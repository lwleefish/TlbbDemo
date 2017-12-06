namespace Demo.Utils
{
    public static class WindowsMessageValue
    {
        // FindWindowEx 时 对话框的 lpClassName
        public static readonly string DialogClassName = "#32770";

        public static readonly string ComboBoxClassName = "ComboBox";

        public static readonly string ComboBoxEx32ClassName = "ComboBoxEx32";

        public static readonly string ButtonClassName = "Button";

        public static readonly string LabelClassName = "Static";

        public static readonly string EditClassName = "Edit";

        public static readonly string ListViewClassName = "SysListView32";

        // 获取 控件当前文本的消息
        public static readonly int WM_GETTEXT = 0x000D;

        public static readonly int WM_SETTEXT = 0x000C;

        //combobox 将 选中项 改为 传入的 字符串
        public static readonly int CB_SELECTSTRING = 0x014D;

        public static readonly int CB_SETCURSEL = 0x014E;

        public static readonly int CB_GETCOUNT = 0x0146;

        public static readonly int CB_GETLBTEXT = 0x0148;

        public static readonly int CB_GETCURSEL = 0x0147;


        public static readonly int LVM_FIRST = 0x1000;
        //public static readonly int LVM_GETITEMCOUNT = LVM_FIRST + 4; 
        public static readonly int LVM_GETHEADER = LVM_FIRST + 31;

        //combobox 选中项 修改后，要发送 CBN_SELCHANGE 消息，combobox才会响应 change事件
        public static readonly int CBN_SELCHANGE = 1;

        public static readonly int WM_COMMAND = 0x0111;

        //鼠标点击消息
        public static readonly int BM_CLICK = 0x00F5;

        public static readonly int MOUSEEVENTF_WHEEL = 0x0800;//滚轮操作

        //移动鼠标 
        public static readonly int MOUSEEVENTF_MOVE = 0x0001;
        //模拟鼠标左键按下 
        public static readonly int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //模拟鼠标左键抬起 
        public static readonly int MOUSEEVENTF_LEFTUP = 0x0004;
        //模拟鼠标右键按下 
        public static readonly int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        public static readonly int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下 
        public static readonly int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起 
        public static readonly int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //标示是否采用绝对坐标 
        public static readonly int MOUSEEVENTF_ABSOLUTE = 0x8000;


        public static readonly int SW_SHOWNORMAL = 1;
        public static readonly int SW_SHOW = 5;
        public static readonly int SW_MAXIMIZE = 3;

        public static readonly int LB_SETCURSEL = 0x0186;

        public static readonly int LVM_GETITEMCOUNT = 0x1004;

        public static readonly int HDM_GETITEMCOUNT = 0x1200;

        public static readonly int LVM_SETITEMSTATE = 0x102B;

        public static readonly int LVM_GETITEMW = 0x1075;
        public static readonly int LVM_GETITEMTEXTW = 0x1073;
        public static readonly int LVM_GETITEMTEXTA = 0x102d;

        public static readonly int WM_LBUTTONDOWN = 0x201;
        public static readonly int WM_LBUTTONUP = 0x0202;
        //public static readonly int LVM_GETITEMTEXT = LVM_FIRST + 45;

    }

}
