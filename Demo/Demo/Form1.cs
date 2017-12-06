using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.IO;
using System.Windows.Forms;
using OpenCvSharp;
using Demo.Utils;
using System.Threading;
using System.Drawing;
using Demo.Model;
using System.Drawing.Imaging;

namespace Demo
{
    public partial class Form1 : Form
    {
        SynchronizationContext ui_context;
        Model.Rect g_rect = new Model.Rect(); //模拟器
        string path = AppDomain.CurrentDomain.BaseDirectory;
        Thread Task_Thr = null;
        bool g_needSearch = true;
        bool run = true;
        private int[,] transaction_data = new int[25, 25];
        KeyboardHook k_hook = new KeyboardHook();
        Random _r = new Random();
        public Form1()
        {
            InitializeComponent();
#if DEBUG
            gb_test.Visible = true;
#endif
            ui_context = SynchronizationContext.Current;
            setField();
            getClientPosition();
            getTransactionData();
            k_hook.KeyDownEvent += new KeyEventHandler(hook_KeyDown);//钩住键按下
            
        }
        //3.判断输入键值（实现KeyDown事件）
        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.S && (int)ModifierKeys == (int)Keys.Alt)
            {
                run = false;
                ui_context.Send(t => { btn_begin.Text = "开始"; }, null);
                //MessageBox.Show("停止！");
            }
        }
        private void Test_Click(object sender, EventArgs e)
        {
            //bool b =getShotAndCalDev(goodsRect.Left + g_rect.Left, goodsRect.Top + g_rect.Top, goodsRect.Right - goodsRect.Left, goodsRect.Bottom - goodsRect.Top);
            //rtb_Log.AppendText("success" + b.ToString());
            Config config = new Config();
            config.transaction = new Position() { X = tb_transaction_x.Text.Trim(), Y = tb_transaction_x.Text.Trim() };
            config.search = new Position() { X = tb_search_x.Text.Trim(), Y = tb_search_y.Text.Trim() };
            config.inputText = new Position() { X = tb_input_x.Text.Trim(), Y = tb_input_y.Text.Trim() };
            config.goodsName = tb_goodsName.Text.Trim();
            config.searchBtn = new Position() { X = tb_searchBtn_x.Text.Trim(), Y = tb_searchBtn_y.Text.Trim() };
            config.goodsBtn = new Position() { X = tb_goods_x.Text.Trim(), Y = tb_goods_y.Text.Trim() };
            config.FirstGoods = new Position() { X = tb_firstGood_x.Text.Trim(), Y = tb_firstGood_y.Text.Trim() };
            config.BuyBtn = new Position() { X = tb_buy_x.Text.Trim(), Y = tb_buy_y.Text.Trim() };
            config.offset = new Position() { X = tb_offsetx.Text.Trim(), Y = tb_offsety.Text.Trim() };
            config.goodsrect = goodsRect;
            config.transactionRect = transactionRect;
            SerializeHelper.XMLSerialize(config, Path.Combine(path, "config.xml"));
        }
        private bool getShotAndCalDev(int x, int y, int cutWidth, int cutHeight)
        {
            if (!string.IsNullOrEmpty(tb_offsetx.Text.Trim()))
            {
                x = x - Convert.ToInt32(tb_offsetx.Text.Trim());
            }
            if (!string.IsNullOrEmpty(tb_offsety.Text.Trim()))
            {
                y = y - Convert.ToInt32(tb_offsety.Text.Trim());
            }
            Graphics g = null;
            int deviation = 0;
            try
            {
                Bitmap baseImage = new Bitmap(cutWidth, cutHeight);
                g = Graphics.FromImage(baseImage);
                g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(cutWidth, cutHeight));
                baseImage.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constant.PicName));
                BitmapData data = baseImage.LockBits(new Rectangle(0, 0, baseImage.Width, baseImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                //System.Collections.Queue queue = new System.Collections.Queue();

                int tmp = 0;
                unsafe
                {
                    byte* ptr = (byte*)(data.Scan0);
                    int width = data.Width <= 150 ? data.Width : 150;
                    int height = data.Height <= 60 ? data.Height : 60;
                    for (int i = 0; i < data.Height; i++)//60
                    {
                        for (int j = 0; j < width; j++)//150
                        {
                            int bule = *ptr;  //117
                            int green = *(ptr + 1);  //142
                            int red = *(ptr + 2);  //165
                            if (Math.Abs(bule - 117) > 20 || Math.Abs(green - 142) > 20 || Math.Abs(red - 165) > 20)
                            {
                                tmp++; //单行偏差点个数
                            }
                            ptr += 3; //
                        }
                        if (tmp >= 30) deviation++; //单行偏差点大于50
                         ptr += data.Stride - data.Width * 3;
                        if (data.Width > 150)
                        {
                            ptr += (data.Width - 150) * 3;
                        }
                        if (deviation > 40)
                            break;
                    }
                }
                baseImage.UnlockBits(data);
            }
            catch { }
            finally
            {
                g?.Dispose();
            }
            return deviation > 40;
        }

        private int getDeviation()
        {
            Mat goodsPic = Cv2.ImRead(Path.Combine(path, Constant.PicName), ImreadModes.Color);
            int deviation = 0;
            System.Collections.Queue queue = new System.Collections.Queue();
            for (int j = 0; j < goodsPic.Height; j += 3)
            {
                queue.Clear();
                for (int i = 0; i < Width; i += 3)
                {
                    Vec3b color = goodsPic.Get<Vec3b>(j, i);
                    int b = color.Item0;  //117
                    int g = color.Item1;  //142
                    int r = color.Item2;  //165
                    if (Math.Abs(b - 117) > 20 || Math.Abs(g - 142) > 20 || Math.Abs(r - 165) > 20)
                    {
                        queue.Enqueue(i);
                    }
                    if (queue.Count > 5)
                    {
                        deviation++;
                        break;
                    }
                }
                if (deviation > 5)
                {
                    break;
                }
            }
            return deviation;
        }
        private void setField()
        {
            Config config = null;
            try
            {
                config = (Config)SerializeHelper.XMLDeSerialize<Config>(typeof(Config), Path.Combine(path, "config.xml"));
                if (config != null)
                {
                    tb_goodsName.Text = config.goodsName;
                    tb_input_x.Text = config.inputText?.X;
                    tb_input_y.Text = config.inputText?.Y;
                    tb_searchBtn_x.Text = config.searchBtn?.X;
                    tb_searchBtn_y.Text = config.searchBtn?.Y;
                    tb_search_x.Text = config.search?.X;
                    tb_search_y.Text = config.search?.Y;
                    tb_goods_x.Text = config.goodsBtn?.X;
                    tb_goods_y.Text = config.goodsBtn?.Y;
                    tb_firstGood_x.Text = config.FirstGoods?.X;
                    tb_firstGood_y.Text = config.FirstGoods?.Y;
                    tb_buy_x.Text = config.BuyBtn?.X;
                    tb_buy_y.Text = config.BuyBtn?.Y;
                    tb_transaction_x.Text = config.transaction?.X;
                    tb_transaction_y.Text = config.transaction?.Y;
                    tb_offsetx.Text = config.offset?.X;
                    tb_offsety.Text = config.offset?.Y;
                    goodsRect = config.goodsrect;
                    transactionRect = config.transactionRect;
                }
            }
            catch
            {
                rtb_Log.Text = "error";
            }
        }
        private void getClientPosition()
        {
            IntPtr hwnd = Win32API.FindWindow("TXGuiFoundation", Constant.Pc_Title);
            if (hwnd == IntPtr.Zero)
            {
                hwnd = Win32API.FindWindow("TXGuiFoundation", Constant.Mo_Title);
            }
            if (hwnd == IntPtr.Zero)
            {
                hwnd = Win32API.FindWindow("TXGuiFoundation", Constant.Pc_Title_standard);
            }
            if (hwnd == IntPtr.Zero)
            {
                MessageBox.Show("SB, 没开模拟器吧?");
                return;
            }
            //置顶
            Win32API.ShowWindow(hwnd, WindowsMessageValue.SW_SHOWNORMAL);
            Win32API.SetForegroundWindow(hwnd);
            Win32API.GetWindowRect(hwnd, out g_rect);

            //rtb_Log.Text = g_rect.getRectStr();
        }
        public System.Drawing.Point getWebControlOffSet(Control contral)
        {
            System.Drawing.Point screenPoint = new System.Drawing.Point(0, 0);
            ui_context.Send(t => { screenPoint = contral.PointToScreen(contral.Location); }, null);
            return screenPoint;
        }
        private void changeNum(Mat src, Mat img)
        {
            //for (int i = 800; i < 860; i++) 
            //{
            //    for (int j = 755; j < 842; j++)
            //    {
            //        src.Set(j, i, img.At<Vec3b>(j - 755, i - 800));
            //    }
            //}
            //Cv2.ImWrite("bbbb.jpg", src);
        }
        //private void readPdf(string file)
        //{
        //    PdfReader pdfReader = new PdfReader(file);
        //    string text = PdfTextExtractor.GetTextFromPage(pdfReader, 6);
        //    try { pdfReader.Close(); }
        //    catch { }
        //    rtb_Log.Text = text;
        //}

        //private void Form1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    ui_context.Send(t => {
        //        rtb_Log.Text = "x:" + e.X + " y:" + e.Y;
        //    }, null);
        //}

        private void bt_0_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Space space = null;
            switch (btn.Name)
            {
                case "btn_transaction":
                    space = new Space(tb_transaction_x, tb_transaction_y);
                    break;
                case "btn_search":
                    space = new Space(tb_search_x, tb_search_y);
                    break;
                case "btn_input":
                    space = new Space(tb_input_x, tb_input_y);
                    break;
                case "btn_searchBtn":
                    space = new Space(tb_searchBtn_x, tb_searchBtn_y);
                    break;
                case "btn_goods":
                    space = new Space(tb_goods_x, tb_goods_y);
                    break;
                case "btn_firstGood":
                    space = new Space(tb_firstGood_x, tb_firstGood_y);
                    break;
                case "btn_buy":
                    space = new Space(tb_buy_x, tb_buy_y);
                    break;
                default:
                    return;
            }
            getClientPosition();
            Thread.Sleep(50);
            space.setPoint = setPoint;
            space.ShowDialog();
        }
        Model.Rect goodsRect = new Model.Rect();
        Model.Rect transactionRect = new Model.Rect();
        public void setPoint(TextBox tbx, TextBox tby, Model.Rect rect)
        {
            try
            {
                if (inRect(g_rect, rect))
                {
                    tbx.Text = ((rect.Left + rect.Right) / 2 - g_rect.Left).ToString();
                    tby.Text = ((rect.Top + rect.Bottom) / 2 - g_rect.Top).ToString();
                    if (tbx.Tag.Equals("goods"))
                    {
                        goodsRect = new Model.Rect(rect.Left - g_rect.Left, rect.Top - g_rect.Top, rect.Right - g_rect.Left, rect.Bottom - g_rect.Top);
                        //getShot(goodsRect.Left + g_rect.Left, goodsRect.Top + g_rect.Top, goodsRect.Right - goodsRect.Left, goodsRect.Bottom - goodsRect.Top);
                    }
                    if (tbx.Tag.Equals("transaction"))
                    {
                        transactionRect = new Model.Rect(rect.Left - g_rect.Left, rect.Top - g_rect.Top, rect.Right - g_rect.Left, rect.Bottom - g_rect.Top);
                        new Thread(tr => {
                            Thread.Sleep(50);
                            getShot(transactionRect.Left + g_rect.Left, transactionRect.Top + g_rect.Top, transactionRect.Right - transactionRect.Left, transactionRect.Bottom - transactionRect.Top, "transaction.jpg");
                            getTransactionData(true);
                        }).Start();
                    }
                }
            }
            catch { }
        }

        private bool getTransactionData(bool save = false)
        {
            int diff = 0;
            try
            {
                string file = "";
                bool isSaved = false;
                if (!save && transaction_data.Length > 0)
                {
                    isSaved = true;
                }
                if (isSaved)
                {
                    file = Path.Combine(path, Constant.PicName);
                }
                else
                {
                    file = Path.Combine(path, "transaction.jpg");
                    if (!File.Exists(file))
                    {
                        return false;
                    }
                }
                Bitmap bmp = (Bitmap)Image.FromFile(file);
                bmp.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constant.PicName));
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* ptr = (byte*)(data.Scan0);
                    int width = data.Width <= Constant.calWidth ? data.Width : Constant.calWidth;
                    int height = data.Height <= Constant.calWidth ? data.Height : Constant.calWidth;
                    int red = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            red = *(ptr + 2);//b g r
                            if (!isSaved)
                            {
                                transaction_data[j, i] = red;
                            }
                            else if (!red.Equals(transaction_data[j, i]))
                            {
                                diff++;
                            }
                            ptr += 3;
                        }
                        ptr += data.Stride - data.Width * 3;
                        if (data.Width > 25)
                        {
                            ptr += (data.Width - 25) * 3;
                        }
                    }
                }
                bmp.UnlockBits(data);
            }
            catch { }
            return diff < 50;
        }
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="content"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private bool inRect(Model.Rect content, Model.Rect rect)
        {
            return (rect.Left >= content.Left && rect.Top >= content.Top && rect.Right <= content.Right && rect.Bottom <= content.Bottom);
        }
        /// <summary>
        /// 截图
        /// </summary>
        private void getShot(int x, int y, int cutWidth, int cutHeight, string filename = Constant.PicName)
        {
            if (!string.IsNullOrEmpty(tb_offsetx.Text.Trim()))
            {
                x = x - Convert.ToInt32(tb_offsetx.Text.Trim());
            }
            if (!string.IsNullOrEmpty(tb_offsety.Text.Trim()))
            {
                y = y - Convert.ToInt32(tb_offsety.Text.Trim());
            }
            //try
            //{
                Bitmap baseImage = new Bitmap(cutWidth, cutHeight);
                Graphics g = Graphics.FromImage(baseImage);
                g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(cutWidth, cutHeight));
                baseImage.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
                g.Dispose();
            //}
            //catch {
                
            //}
        }
        private void btn_testClick_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            ClickField(btn.Name);
        }

        private void ClickField(string name)
        {
            int x = 0, y = 0;
            switch (name)
            {
                case "btn_transactionBtn":
                    x = Convert.ToInt32(double.Parse(tb_transaction_x.Text.Trim()));
                    y = Convert.ToInt32(double.Parse(tb_transaction_y.Text.Trim()));
                    break;
                case "btn_searchClick":
                    x = Convert.ToInt32(double.Parse(tb_search_x.Text.Trim()));
                    y = Convert.ToInt32(double.Parse(tb_search_y.Text.Trim()));
                    break;
                case "btn_inputClick":
                    x = Convert.ToInt32(double.Parse(tb_input_x.Text.Trim()));
                    y = Convert.ToInt32(double.Parse(tb_input_y.Text.Trim()));
                    break;
                case "btn_searchBtnClick":
                    x = Convert.ToInt32(double.Parse(tb_searchBtn_x.Text.Trim()));
                    y = Convert.ToInt32(double.Parse(tb_searchBtn_y.Text.Trim()));
                    break;
                case "btn_goodsClick":
                    x = Convert.ToInt32(double.Parse(tb_goods_x.Text.Trim()));
                    y = Convert.ToInt32(double.Parse(tb_goods_y.Text.Trim()));
                    break;
                case "btn_firstGoodBtn":
                    x = Convert.ToInt32(double.Parse(tb_firstGood_x.Text.Trim()));
                    y = Convert.ToInt32(double.Parse(tb_firstGood_y.Text.Trim()));
                    break;
                //case "btn_goodsTextClick":
                //    x = Convert.ToInt32(double.Parse(tb_input_x.Text.Trim()));
                //    y = Convert.ToInt32(double.Parse(tb_input_y.Text.Trim()));
                //    break;
                case "btn_buyClick":
                    x = Convert.ToInt32(double.Parse(tb_buy_x.Text.Trim()));
                    y = Convert.ToInt32(double.Parse(tb_buy_y.Text.Trim()));
                    break;
            }
            getClientPosition();
            Win32API.MouseClick(g_rect.Left + x, g_rect.Top + y);
            if (name.Equals("btn_inputClick"))
            {
                setInput();
            }
        }
        /// <summary>
        /// 输入
        /// </summary>
        private void setInput()
        {
            ui_context.Send(t => { Clipboard.SetText(tb_goodsName.Text.Trim()); }, null);
            Thread.Sleep(200);
            Win32API.keybd_event(Convert.ToInt32(Keys.ControlKey), 0, 0, 0);
            Win32API.keybd_event(Convert.ToInt32(Keys.V), 0, 0, 0);
            Thread.Sleep(50);
            Win32API.keybd_event(Convert.ToInt32(Keys.ControlKey), 0, 0x02, 0);
            Win32API.keybd_event(Convert.ToInt32(Keys.V), 0, 0x02, 0);
        }

        private void btn_begin_Click(object sender, EventArgs e)
        {
            run = true;
            Task_Thr = new Thread(DoTask);
            k_hook.Start();//安装键盘钩子
            Task_Thr.IsBackground = true;
            Task_Thr.Start();
            btn_begin.Text = "结束(Alt+S)";
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        private void DoTask()
        {
            
            while (run)
            {
                if (needClickTransaction())
                {
                    ClickField("btn_transactionBtn");
                }
                Thread.Sleep((int)nud_thr.Value + _r.Next(50));
                //Thread.Sleep(3000);
                ClickField("btn_searchClick");
                Thread.Sleep(500);
                if (g_needSearch)
                {
                    ClickField("btn_inputClick");
                    Thread.Sleep(500);
                    ClickField("btn_searchBtnClick");
                    Thread.Sleep(200);
                    ClickField("btn_searchBtnClick");
                    Thread.Sleep(100);
                }
                ClickField("btn_goodsClick");
                Thread.Sleep(500);
                bool hasGoods = false;
                hasGoods = getShotAndCalDev(goodsRect.Left + g_rect.Left, goodsRect.Top + g_rect.Top, goodsRect.Right - goodsRect.Left, goodsRect.Bottom - goodsRect.Top);
                //Thread.Sleep(50);
                //int dev = getDeviation();
                if (!hasGoods)
                {
                    //Thread.Sleep(Constant.Sleep1Seconds);
                    continue;
                }
                ClickField("btn_firstGoodBtn");
                Thread.Sleep(200);
                ClickField("btn_buyClick");
            }
        }

        private bool needClickTransaction()
        {
            getShot(transactionRect.Left + g_rect.Left, transactionRect.Top + g_rect.Top, transactionRect.Right - transactionRect.Left, transactionRect.Bottom - transactionRect.Top);
            return getTransactionData();
        }
        /// <summary>
        /// 搜索选项事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_needSearch_CheckedChanged(object sender, EventArgs e)
        {
            g_needSearch = cb_needSearch.Checked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_t_Click(object sender, EventArgs e)
        {
            //getShot(transactionRect.Left + g_rect.Left, transactionRect.Top + g_rect.Top, transactionRect.Right - transactionRect.Left, transactionRect.Bottom - transactionRect.Top);
            bool b = getTransactionData();
        }

        int offset_x = 0, offset_y;

        private void btn_offset_Click(object sender, EventArgs e)
        {

        }

        private void tb_offsetx_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.KeyChar != 45 && e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
