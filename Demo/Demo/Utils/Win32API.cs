using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Demo.Utils
{
    internal class Win32API
    {
        /// <summary>
        //提升进程访问控制权限
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }
        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_SHUTDOWN_NAME = "SeDebugPrivilege";

        [DllImport("user32.dll")]
        public static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);
        public static uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        public static int HWND_BROADCAST = 0xffff;
        public static string en_US = "00000409";
        public static uint KLF_ACTIVATE = 1;
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);
        ///
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hwnd, Win32API.WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int smIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLong64(IntPtr hWnd, int nIndex);

        [DllImport("User32")]
        public extern static void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);
        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);
        [DllImport("User32")]
        public extern static bool GetCursorPos(out POINT p);
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public enum MouseEventFlags
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            Wheel = 0x0800,
            Absolute = 0x8000
        }
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
            {
                return Win32API.GetWindowLong64(hWnd, nIndex);
            }
            return Win32API.GetWindowLong32(hWnd, nIndex);
        }


        [DllImport("User32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, out Model.Rect lpRect);//获取窗口坐标
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out int processId);

        [DllImport("user32.dll")]
        public static extern bool IsChild(IntPtr parentHwnd, IntPtr hwnd);

        public static void MouseClick(int x = -1, int y = -1)
        {
            if (x < 0 || y < 0)
            {
                x = Control.MousePosition.X;
                y = Control.MousePosition.Y;
            }
            Win32API.INPUT iNPUT = new Win32API.INPUT
            {
                Type = 0u
            };
            iNPUT.Type = 0u;
            iNPUT.Data.Mouse.MouseData = 0u;
            iNPUT.Data.Mouse.Time = 0u;
            iNPUT.Data.Mouse.ExtraInfo = IntPtr.Zero;
            iNPUT.Data.Mouse.X = x * 65535 / Screen.PrimaryScreen.Bounds.Width;
            iNPUT.Data.Mouse.Y = y * 65535 / Screen.PrimaryScreen.Bounds.Height;
            iNPUT.Data.Mouse.Flags = 32769u;
            Win32API.INPUT[] inputs = new Win32API.INPUT[]
            {
                iNPUT
            };
            Win32API.SendInput(1u, inputs, Marshal.SizeOf(typeof(Win32API.INPUT)));
            iNPUT.Data.Mouse.Flags = 32770u;
            Win32API.INPUT[] inputs2 = new Win32API.INPUT[]
            {
                iNPUT
            };
            Win32API.SendInput(1u, inputs2, Marshal.SizeOf(typeof(Win32API.INPUT)));
            Thread.Sleep(10);
            iNPUT.Data.Mouse.Flags = 32772u;
            Win32API.INPUT[] inputs3 = new Win32API.INPUT[]
            {
                iNPUT
            };
            Win32API.SendInput(1u, inputs3, Marshal.SizeOf(typeof(Win32API.INPUT)));
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void keybd_event(int bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, Win32API.INPUT[] inputs, int sizeOfInputStructure);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string sParam);
        
        [DllImport("user32.dll")]
        protected static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, StringBuilder lParam);
        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public static void SetTransparent(IntPtr handle, byte alpha)
        {
            Win32API.SetWindowLongPtr(handle, -20, (IntPtr)(Win32API.GetWindowLongPtr(handle, -20).ToInt32() | 524288));
            Win32API.SetLayeredWindowAttributes(handle, 0u, alpha, 2u);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return Win32API.SetWindowLong64(hWnd, nIndex, dwNewLong);
            }
            return Win32API.SetWindowLong32(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref Win32API.WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, Win32API.WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int processId);


        public static uint PROCESS_VM_OPERATION = 0x0008;//允许函数VirtualProtectEx使用此句柄修改进程的虚拟内存  
        public static uint PROCESS_VM_READ = 0x0010;//允许函数访问权限  
        public static uint PROCESS_VM_WRITE = 0x0020;//允许函数写入权限

        [DllImport("kernel32.dll")]//在指定进程的虚拟空间保留或提交内存区域
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
    uint dwSize, uint flAllocationType, uint flProtect);

        public static uint MEM_RESERVE = 0x2000;
        public static uint MEM_COMMIT = 0x1000;
        public static uint PAGE_READWRITE = 4;
        public static int LVIF_TEXT = 0x0001;
        public static int LVIS_FOCUSED = 1;
        public static int LVIS_SELECTED = 2;
        public const int LVM_FIRST = 0x1000;
        public const int LVM_SETITEMSTATE = LVM_FIRST + 43;
        public const int LVM_GETITEMW = LVM_FIRST + 75;
        public const uint MEM_RELEASE = 0x8000;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;

        [DllImport("kernel32.dll")]//将数据写入内存中  
        public static extern bool WriteProcessMemory(
                                    IntPtr hProcess,//由OpenProcess返回的进程句柄  
                                    IntPtr lpBaseAddress, //要写的内存首地址,再写入之前,此函数将先检查目标地址是否可用,并能容纳待写入的数据  
                                    IntPtr lpBuffer, //指向要写的数据的指针  
                                    int nSize, //要写入的字节数  
                                    ref uint vNumberOfBytesRead
);
        [DllImport("kernel32.dll")]//在其它进程中释放申请的虚拟内存空间  
        public static extern bool VirtualFreeEx(
                            IntPtr hProcess,//目标进程的句柄,该句柄必须拥有PROCESS_VM_OPERATION的权限  
                            IntPtr lpAddress,//指向要释放的虚拟内存空间首地址的指针  
                            uint dwSize,
                            uint dwFreeType//释放类型  
);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32.dll")]//从指定内存中读取字节集数据  
        public static extern bool ReadProcessMemory(
                                    IntPtr hProcess, //被读取者的进程句柄  
                                    IntPtr lpBaseAddress,//开始读取的内存地址  
                                    IntPtr lpBuffer, //数据存储变量  
                                    int nSize, //要写入多少字节  
                                    ref uint vNumberOfBytesRead//读取长度  
);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern bool BitBlt(
            IntPtr hdcDest, // 目的 DC的句柄
            int nXDest,
            int nYDest,
            int nWidth,
            int nHeight,
            IntPtr hdcSrc, // 源DC的句柄
            int nXSrc,
            int nYSrc,
            System.Int32 dwRop // 光栅的处置数值
        );
        internal struct HARDWAREINPUT
        {
            public uint Msg;
            public ushort ParamL;
            public ushort ParamH;
        }

        internal struct INPUT
        {
            public uint Type;
            public Win32API.MOUSEKEYBDHARDWAREINPUT Data;
        }

        internal struct KEYBDINPUT
        {
            public ushort Vk;

            public ushort Scan;

            public uint Flags;

            public uint Time;

            public IntPtr ExtraInfo;
        }

        internal struct MOUSEINPUT
        {
            public int X;

            public int Y;

            public uint MouseData;

            public uint Flags;

            public uint Time;

            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public Win32API.HARDWAREINPUT Hardware;

            [FieldOffset(0)]
            public Win32API.KEYBDINPUT Keyboard;

            [FieldOffset(0)]
            public Win32API.MOUSEINPUT Mouse;
        }

        public delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);


        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        public static void OnClickButton(IntPtr vpnHWnd, IntPtr BtnHWnd)
        {
            Model.Rect rect = new Model.Rect();
            GetWindowRect(BtnHWnd, out rect);
            ShowWindow(vpnHWnd, WindowsMessageValue.SW_SHOWNORMAL);
            SetForegroundWindow(vpnHWnd);
            Thread.Sleep(100);
            int x = Convert.ToInt32((rect.Left + rect.Right) / 2);
            int y = Convert.ToInt32((rect.Top + rect.Bottom) / 2);
            MouseClick(x, y);
            
        }

    }
}
