using System.Drawing;
using System.Windows.Forms;

namespace Demo
{
    public delegate void SetPoint(TextBox tbx, TextBox tby, Model.Rect rect);
    public partial class Space : Form
    {
        public SetPoint setPoint;
        bool g_beginPaint = false;
        Point g_startPoint = new Point(0, 0);
        Point g_endPoint = new Point(0, 0);
        Graphics g = null;
        Rectangle g_rect = new Rectangle(0, 0, 0, 0);
        TextBox tb_x, tb_y;
        public Space(TextBox tbx, TextBox tby)
        {
            InitializeComponent();
            TopMost = true;
            tb_x = tbx;
            tb_y = tby;
            Opacity = 0.4;
            //TransparencyKey = Color.Red;
            //BackColor = Color.Red;
            //FormBorderStyle 设置为 None
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            g = CreateGraphics();
            hideButton();
        }

        private void showButton()
        {
            btn_OK.Visible = true;
            btn_OK.Enabled = true;
            btn_OK.Location = g_endPoint;
            btn_cancel.Visible = true;
            btn_cancel.Enabled = true;
            btn_cancel.Location = new Point(g_endPoint.X - btn_cancel.Width - 5, g_endPoint.Y);
        }
        private void hideButton()
        {
            btn_OK.Location = new Point(0, 0);
            btn_cancel.Location = new Point(0, 0);
            btn_OK.Visible = false;
            btn_OK.Enabled = false;
            btn_cancel.Visible = false;
            btn_cancel.Enabled = false;
        }
        private void Space_MouseDown(object sender, MouseEventArgs e)
        {
            g.Clear(Color.White);
            g_startPoint = e.Location;
            g_beginPaint = true;
            hideButton();
        }

        private void Space_MouseUp(object sender, MouseEventArgs e)
        {
            g_endPoint = e.Location;
            
            Rectangle rect = new Rectangle(g_startPoint.X, g_startPoint.Y, g_endPoint.X - g_startPoint.X, g_endPoint.Y - g_startPoint.Y);//定义矩形,参数为起点横纵坐标以及其长和宽
            if (isRectangle(g_startPoint, g_endPoint))
            {
                Pen pen = new Pen(Color.Red);
                pen.Width = 2;
                showButton(); //先显示按钮防止重绘时 矩形被清空
                g.Clear(Color.White);
                g.DrawRectangle(pen, rect);
                g_rect = rect;
            }
            g_beginPaint = false;
        }
        private bool isRectangle(Point p1, Point p2)
        {
            return (System.Math.Abs(p1.X - p2.X) > 1 && System.Math.Abs(p1.Y - p2.Y) > 1);
        }
        private void Space_MouseMove(object sender, MouseEventArgs e)
        {
            if (g_beginPaint)
            {
                g_endPoint = e.Location;
                Rectangle rect = new Rectangle(g_startPoint.X, g_startPoint.Y, g_endPoint.X - g_startPoint.X, g_endPoint.Y - g_startPoint.Y);//定义矩形,参数为起点横纵坐标以及其长和宽
                if (isRectangle(g_startPoint, g_endPoint))
                {
                    Pen pen = new Pen(Color.Red);
                    pen.Width = 2;
                    g.Clear(Color.White);
                    g.DrawRectangle(pen, rect);
                    g_rect = rect;
                }
            }
        }

        private void btn_OK_Click(object sender, System.EventArgs e)
        {
            Model.Rect rect = new Model.Rect();
            try
            {
                rect.Top = g_rect.Top;
                rect.Left = g_rect.Left;
                rect.Right = g_rect.Right;
                rect.Bottom = g_rect.Bottom;
                setPoint?.Invoke(tb_x, tb_y, rect);
            }
            catch { }
            g?.Dispose();
            Close();
        }

        private void btn_cancel_Click(object sender, System.EventArgs e)
        {
            hideButton();
            g.Clear(Color.White);
        }
    }
}
