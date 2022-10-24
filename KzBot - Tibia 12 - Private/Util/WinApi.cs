using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace KzBot
{
    public class WinApi
    {
        private const uint CF_UNICODETEXT = 13U;

        public static string sha1(string strToHash)
        {
            var sha1Obj = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            var bytesToHash = Encoding.ASCII.GetBytes(strToHash);

            bytesToHash = sha1Obj.ComputeHash(bytesToHash);

            string strResult = "";

            foreach (byte b in bytesToHash)
                strResult += b.ToString("x2");

            return strResult;
        }

        public static bool IsNumericType(object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }


        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        public struct PROCESS_INFORMATION
        {
            public uint dwProcessId;
            public uint dwThreadId;
            public IntPtr hProcess;
            public IntPtr hThread;
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        // 矩形结构
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public struct SECURITY_ATTRIBUTES
        {
            public bool bInheritHandle;
            public int length;
            public IntPtr lpSecurityDescriptor;
        }

        public struct STARTUPINFO
        {
            public uint cb;
            public short cbReserved2;
            public uint dwFillAttribute;
            public uint dwFlags;
            public uint dwX;
            public uint dwXCountChars;
            public uint dwXSize;
            public uint dwY;
            public uint dwYCountChars;
            public uint dwYSize;
            public IntPtr hStdError;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public string lpDesktop;
            public string lpReserved;
            public IntPtr lpReserved2;
            public string lpTitle;
            public short wShowWindow;
        }

        [Flags()]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x1),
            SUSPEND_RESUME = (0x2),
            GET_CONTEXT = (0x8),
            SET_CONTEXT = (0x10),
            SET_INFORMATION = (0x20),
            QUERY_INFORMATION = (0x40),
            SET_THREAD_TOKEN = (0x80),
            IMPERSONATE = (0x100),
            DIRECT_IMPERSONATION = (0x200),
            THREAD_ALL = TERMINATE | SUSPEND_RESUME | GET_CONTEXT | SET_CONTEXT | SET_INFORMATION | QUERY_INFORMATION | SET_THREAD_TOKEN | IMPERSONATE | DIRECT_IMPERSONATION
        }

        public enum CONTEXT_FLAGS : uint
        {
            CONTEXT_i386 = 0x10000,
            CONTEXT_i486 = 0x10000,   //  same as i386
            CONTEXT_CONTROL = CONTEXT_i386 | 0x01, // SS:SP, CS:IP, FLAGS, BP
            CONTEXT_INTEGER = CONTEXT_i386 | 0x02, // AX, BX, CX, DX, SI, DI
            CONTEXT_SEGMENTS = CONTEXT_i386 | 0x04, // DS, ES, FS, GS
            CONTEXT_FLOATING_POINT = CONTEXT_i386 | 0x08, // 387 state
            CONTEXT_DEBUG_REGISTERS = CONTEXT_i386 | 0x10, // DB 0-3,6,7
            CONTEXT_EXTENDED_REGISTERS = CONTEXT_i386 | 0x20, // cpu specific extensions
            CONTEXT_FULL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS,
            CONTEXT_ALL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS | CONTEXT_FLOATING_POINT | CONTEXT_DEBUG_REGISTERS | CONTEXT_EXTENDED_REGISTERS
        }

        // x86 float save
        [StructLayout(LayoutKind.Sequential)]
        public struct FLOATING_SAVE_AREA
        {
            public uint ControlWord;
            public uint StatusWord;
            public uint TagWord;
            public uint ErrorOffset;
            public uint ErrorSelector;
            public uint DataOffset;
            public uint DataSelector;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] RegisterArea;
            public uint Cr0NpxState;
        }

        // x86 context structure (not used in this example)
        [StructLayout(LayoutKind.Sequential)]
        public struct CONTEXT
        {
            public uint ContextFlags; //set this to an appropriate value 
                                      // Retrieved by CONTEXT_DEBUG_REGISTERS 
            public uint Dr0;
            public uint Dr1;
            public uint Dr2;
            public uint Dr3;
            public uint Dr6;
            public uint Dr7;
            // Retrieved by CONTEXT_FLOATING_POINT 
            public FLOATING_SAVE_AREA FloatSave;
            // Retrieved by CONTEXT_SEGMENTS 
            public uint SegGs;
            public uint SegFs;
            public uint SegEs;
            public uint SegDs;
            // Retrieved by CONTEXT_INTEGER 
            public uint Edi;
            public uint Esi;
            public uint Ebx;
            public uint Edx;
            public uint Ecx;
            public uint Eax;
            // Retrieved by CONTEXT_CONTROL 
            public uint Ebp;
            public uint Eip;
            public uint SegCs;
            public uint EFlags;
            public uint Esp;
            public uint SegSs;
            // Retrieved by CONTEXT_EXTENDED_REGISTERS 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] ExtendedRegisters;
        }

        // x64 m128a
        [StructLayout(LayoutKind.Sequential)]
        public struct M128A
        {
            public ulong High;
            public long Low;

            public override string ToString()
            {
                return string.Format("High:{0}, Low:{1}", this.High, this.Low);
            }
        }

        // x64 save format
        [StructLayout(LayoutKind.Sequential, Pack = 16)]
        public struct XSAVE_FORMAT64
        {
            public ushort ControlWord;
            public ushort StatusWord;
            public byte TagWord;
            public byte Reserved1;
            public ushort ErrorOpcode;
            public uint ErrorOffset;
            public ushort ErrorSelector;
            public ushort Reserved2;
            public uint DataOffset;
            public ushort DataSelector;
            public ushort Reserved3;
            public uint MxCsr;
            public uint MxCsr_Mask;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public M128A[] FloatRegisters;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public M128A[] XmmRegisters;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
            public byte[] Reserved4;
        }

        // x64 context structure
        [StructLayout(LayoutKind.Sequential, Pack = 16)]
        public struct CONTEXT64
        {
            public ulong P1Home;
            public ulong P2Home;
            public ulong P3Home;
            public ulong P4Home;
            public ulong P5Home;
            public ulong P6Home;

            public CONTEXT_FLAGS ContextFlags;
            public uint MxCsr;

            public ushort SegCs;
            public ushort SegDs;
            public ushort SegEs;
            public ushort SegFs;
            public ushort SegGs;
            public ushort SegSs;
            public uint EFlags;

            public ulong Dr0;
            public ulong Dr1;
            public ulong Dr2;
            public ulong Dr3;
            public ulong Dr6;
            public ulong Dr7;

            public ulong Rax;
            public ulong Rcx;
            public ulong Rdx;
            public ulong Rbx;
            public ulong Rsp;
            public ulong Rbp;
            public ulong Rsi;
            public ulong Rdi;
            public ulong R8;
            public ulong R9;
            public ulong R10;
            public ulong R11;
            public ulong R12;
            public ulong R13;
            public ulong R14;
            public ulong R15;
            public ulong Rip;

            public XSAVE_FORMAT64 DUMMYUNIONNAME;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
            public M128A[] VectorRegister;
            public ulong VectorControl;

            public ulong DebugControl;
            public ulong LastBranchToRip;
            public ulong LastBranchFromRip;
            public ulong LastExceptionToRip;
            public ulong LastExceptionFromRip;
        }

        public const uint CREATE_SUSPENDED = 0x00000004;
        public const int HT_CAPTION = 0x2;
        public const uint HWND_NOTOPMOST = 0xFFFFFFFE;
        public const uint HWND_TOPMOST = 0xFFFFFFFF;
        public const int KEY_DOWN_EVENT = 0x0001;

        //Key down flag
        public const int KEY_UP_EVENT = 0x0002;

        //Key up flag
        public const int KEYEVENTF_KEYUP = 0x02;

        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RELEASE = 0x8000;
        public const uint MEM_RESERVE = 0x2000;
        public const uint PAGE_EXECUTE_READ = 0x20;
        public const uint PAGE_EXECUTE_READWRITE = 0x40;
        public const uint PAGE_READWRITE = 0x4;
        public const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;
        public const int SW_HIDE = 0;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;
        public const int SW_SHOW = 5;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOWNORMAL = 1;
        public const uint SWP_NOMOVE = 0x2;
        public const uint SWP_NOSIZE = 0x1;
        public const uint WM_CHAR = 0x0102;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint WM_LBUTTONDOWN = 0x201;
        public const uint WM_LBUTTONUP = 0x202;
        public const uint WM_MBUTTONDOWN = 0x207;
        public const uint WM_MBUTTONUP = 0x208;
        public const uint WM_MOUSEMOVE = 0x200;
        public const uint WM_MOUSEWHEEL = 0x20A;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const uint WM_RBUTTONDOWN = 0x204;
        public const uint WM_RBUTTONUP = 0x205;
        public const uint WM_SYSKEYDOWN = 0x0104;
        public const uint WM_SYSKEYUP = 0x0105;
        public const uint EM_SETCUEBANNER = 0x1501;

        public const uint MK_LBUTTON = 0x1;
        public const uint MK_MBUTTON = 0x10;
        public const uint MK_RBUTTON = 0x2;

        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadContext(IntPtr hThread, ref CONTEXT lpContext);

        [DllImport("kernel32.dll")]
        public static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
    IntPtr hProcess,
    IntPtr lpBaseAddress,
    [Out] byte[] lpBuffer,
    int dwSize,
    out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            IntPtr lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        /*[DllImport("Gdi32.dll", EntryPoint = nameof(CreateRoundRectRgn))]
        internal static extern IntPtr CreateRoundRectRgn
        (
        int nLeftRect, // x-coordinate of upper-left corner
        int nTopRect, // y-coordinate of upper-left corner
        int nRightRect, // x-coordinate of lower-right corner
        int nBottomRect, // y-coordinate of lower-right corner
        int nWidthEllipse, // height of ellipse
        int nHeightEllipse // width of ellipse
        );*/

        [DllImport("User32.dll")]
        internal static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        internal static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("uxtheme.dll")]
        internal static extern IntPtr SetWindowTheme(IntPtr hWnd, string
        pszSubAppName, string pszSubIdList);

        internal static bool WriteFloat(IntPtr handle, long address, float value)
        {
            return WriteBytes(handle, address, BitConverter.GetBytes(value), sizeof(float));
        }

        internal static bool WriteInt(IntPtr Handle, long address, int value)
        {
            return WriteBytes(Handle, address, BitConverter.GetBytes(value), 4);
        }

        internal static bool WriteInt16(IntPtr Handle, long address, short value)
        {
            return WriteBytes(Handle, address, BitConverter.GetBytes(value), sizeof(short));
        }

        internal static void WriteNops(IntPtr Handle, long address, int nops)
        {
            byte nop = 0x90;
            int j = 0;
            for (int i = 0; i < nops; i++)
            {
                WriteBytes(Handle, address + j, new byte[] { nop }, 1);
                j++;
            }
        }

        internal static bool WriteUInt32(IntPtr Handle, long address, uint value)
        {
            return WriteBytes(Handle, address, BitConverter.GetBytes(value), sizeof(uint));
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseClipboard();

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CreateProcess(string imageName,
                string cmdLine,
                IntPtr lpProcessAttributes,
                IntPtr lpThreadAttributes,
                bool boolInheritHandles,
                uint dwCreationFlags,
                IntPtr lpEnvironment,
                string lpszCurrentDir,
                ref STARTUPINFO si,
                out PROCESS_INFORMATION pi);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess,
           IntPtr lpThreadAttributes, uint dwStackSize, IntPtr
           lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern HandleRef FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hWnd, bool invert);

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        public static uint getAddress(IntPtr Handle, uint pointer, uint[] offset)
        {
            byte[] bytes = new byte[24];
            uint address = ReadPointer(Handle, pointer) + offset[0];
            if (offset.Length > 1)
            {
                for (int i = 1; i < offset.Length; i++)
                {
                    address = ReadPointer(Handle, address) + offset[i];
                }
            }

            return address;
        }

        [DllImport("user32.dll")]
        public static extern bool GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        [DllImport("user32.dll")]
        public static extern bool GetAsyncKeyState(int vKeyCode);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder className, int maxCharCount);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        public static string GetText()
        {
            if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
            {
                return null;
            }

            try
            {
                if (!OpenClipboard(IntPtr.Zero))
                {
                    return null;
                }

                IntPtr handle = GetClipboardData(CF_UNICODETEXT);
                if (handle == IntPtr.Zero)
                {
                    return null;
                }

                IntPtr pointer = IntPtr.Zero;

                try
                {
                    pointer = GlobalLock(handle);
                    if (pointer == IntPtr.Zero)
                    {
                        return null;
                    }

                    int size = GlobalSize(handle);
                    byte[] buff = new byte[size];

                    Marshal.Copy(pointer, buff, 0, size);

                    return Encoding.Unicode.GetString(buff).TrimEnd('\0');
                }
                finally
                {
                    if (pointer != IntPtr.Zero)
                    {
                        GlobalUnlock(handle);
                    }
                }
            }
            finally
            {
                CloseClipboard();
            }
        }

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern int GlobalSize(IntPtr hMem);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        public static bool IsValidAddress(Process process, uint addr)
        {
            if (addr >= 0x7FFFFFFF || addr < process.MainModule.BaseAddress.ToInt32())
            {
                return false;
            }

            IntPtr none;
            byte[] tmpBuffer = new byte[sizeof(uint)];
            try
            {
                ReadProcessMemory(process.Handle, (IntPtr)addr, tmpBuffer, tmpBuffer.Length, out none);
            }
            catch (Exception)
            {
                return false;
            }

            return tmpBuffer[0] > 0 && none.ToInt32() > 0;
        }

        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static int MakeLParam(int LoWord, int HiWord)
        {
            return ((HiWord << 16) | (LoWord & 0xffff));
        }

        //the same function but with another name =D
        // just for understand the code better.
        public static int MakeWParam(int LoWord, int HiWord)
        {
            return ((HiWord << 16) | (LoWord & 0xffff));
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        public static byte ReadByte(IntPtr Handle, long Address)
        {
            byte[] buffer = new byte[1];
            IntPtr intPtr;
            ReadProcessMemory(Handle, new IntPtr(Address), buffer, 1, out intPtr);
            return buffer[0];
        }

        public static byte[] ReadBytes(IntPtr Handle, long Address, uint BytesToRead)
        {
            byte[] buffer = new byte[BytesToRead];
            IntPtr intPtr;
            ReadProcessMemory(Handle, new IntPtr(Address), buffer, (int)BytesToRead, out intPtr);
            return buffer;
        }

        public static double ReadDouble(IntPtr Handle, long address)
        {
            return BitConverter.ToDouble(ReadBytes(Handle, address, 8), 0);
        }

        public static double ReadDoubleOffset(IntPtr Handle, uint pointer, uint[] offset)
        {
            uint address = ReadPointer(Handle, pointer) + offset[0];
            if (offset.Length > 1)
            {
                for (int i = 1; i < offset.Length; i++)
                {
                    address = ReadPointer(Handle, address) + offset[i];
                }
            }

            return ReadDouble(Handle, address);
        }

        public static double ReadDoubleOffset(IntPtr Handle, uint pointer, uint offset)
        {
            uint address = ReadPointer(Handle, pointer) + offset;
            return ReadDouble(Handle, address);
        }

        public static float ReadFloat(IntPtr Handle, long address)
        {
            return BitConverter.ToSingle(ReadBytes(Handle, address, sizeof(float)), 0);
        }

        public static int ReadInt16(IntPtr Handle, uint Address)
        {
            return BitConverter.ToInt16(ReadBytes(Handle, Address, sizeof(short)), 0);
        }

        public static int ReadInt32(IntPtr Handle, uint Address)
        {
            return BitConverter.ToInt32(ReadBytes(Handle, Address, 4), 0);
        }

        public static long ReadInt64(IntPtr Handle, uint Address)
        {
            return BitConverter.ToInt64(ReadBytes(Handle, Address, 8), 0);
        }

        public static byte ReadOffsetByte(IntPtr Handle, uint pointer, uint[] offset)
        {
            byte[] bytes = new byte[sizeof(int)];
            uint address = 0;
            for (int i = 0; i < offset.Length; i++)
            {
                address = ReadPointer(Handle, (i == 0) ? pointer : address) + offset[i];
            }

            return ReadByte(Handle, address);
        }

        public static byte ReadOffsetByte(IntPtr Handle, uint pointer, uint offset)
        {
            byte[] bytes = new byte[sizeof(int)];
            uint address = ReadPointer(Handle, pointer) + offset;

            return ReadByte(Handle, address);
        }

        public static int ReadOffsetInt16(IntPtr Handle, uint pointer, uint[] offset)
        {
            byte[] bytes = new byte[sizeof(int)];
            uint address = 0;
            for (int i = 0; i < offset.Length; i++)
            {
                address = ReadPointer(Handle, (i == 0) ? pointer : address) + offset[i];
            }

            IntPtr intPtr;
            ReadProcessMemory(Handle, (IntPtr)address, bytes, sizeof(int), out intPtr);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static int ReadOffsetInt16(IntPtr Handle, uint pointer, uint offset)
        {
            byte[] bytes = new byte[sizeof(int)];
            uint address = ReadPointer(Handle, pointer) + offset;

            IntPtr intPtr;
            ReadProcessMemory(Handle, (IntPtr)address, bytes, sizeof(int), out intPtr);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static int ReadOffsetInt32(IntPtr Handle, uint pointer, uint[] offset)
        {
            byte[] bytes = new byte[sizeof(int)];
            uint address = 0;
            for (int i = 0; i < offset.Length; i++)
            {
                address = ReadPointer(Handle, (i == 0) ? pointer : address) + offset[i];
            }

            IntPtr intPtr;
            ReadProcessMemory(Handle, (IntPtr)address, bytes, sizeof(int), out intPtr);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static int ReadOffsetInt32(IntPtr Handle, uint pointer, uint offset)
        {
            byte[] bytes = new byte[sizeof(int)];
            uint address = ReadPointer(Handle, pointer) + offset;

            IntPtr intPtr;
            ReadProcessMemory(Handle, (IntPtr)address, bytes, sizeof(int), out intPtr);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static long ReadOffsetInt64(IntPtr Handle, uint pointer, uint[] offset)
        {
            byte[] bytes = new byte[sizeof(long)];
            uint address = 0;
            for (int i = 0; i < offset.Length; i++)
            {
                address = ReadPointer(Handle, (i == 0) ? pointer : address) + offset[i];
            }

            IntPtr intPtr;
            ReadProcessMemory(Handle, (IntPtr)address, bytes, sizeof(long), out intPtr);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static string ReadOffsetString(IntPtr Handle, uint pointer, uint[] offset, uint length = 32)
        {
            byte[] bytes = new byte[length * 2];
            uint address = ReadPointer(Handle, pointer) + offset[0];
            if (address == 0)
            {
                return "";
            }

            if (offset.Length > 1)
            {
                for (int i = 1; i < offset.Length; i++)
                {
                    address = ReadPointer(Handle, address) + offset[i];
                    if (address == 0)
                    {
                        return "";
                    }
                }
            }

            if (address == 0 || address >= 0x7FFFFFFF)
            {
                return "";
            }

            IntPtr intPtr;
            ReadProcessMemory(Handle, (IntPtr)address, bytes, (int)length * 2, out intPtr);
            if (bytes[0] == 0)
            {
                return "";
            }

            return ASCIIEncoding.Unicode.GetString(bytes);
        }

        public static string ReadOffsetString(IntPtr Handle, uint pointer, uint offset, uint length = 32)
        {
            byte[] bytes = new byte[length * 2];
            uint address = ReadPointer(Handle, pointer) + offset;

            if (address == 0 || address >= 0x7FFFFFFF)
            {
                return "";  
            }

            IntPtr intPtr;
            ReadProcessMemory(Handle, (IntPtr)address, bytes, (int)length * 2, out intPtr);
            if (bytes.Length <= 0 || bytes[0] == 0)
            {
                return "";
            }

            return ASCIIEncoding.Unicode.GetString(bytes);
        }

        public static uint ReadOffsetStringAddress(IntPtr Handle, uint pointer, uint[] offset)
        {
            uint address = ReadPointer(Handle, pointer) + offset[0];
            if (offset.Length > 1)
            {
                for (int i = 1; i < offset.Length; i++)
                {
                    address = ReadPointer(Handle, address) + offset[i];
                }
            }

            return address;
        }

        public static uint ReadOffsetUInt32(IntPtr Handle, uint pointer, uint[] offset)
        {
            byte[] bytes = new byte[sizeof(uint)];
            uint address = 0;
            for (int i = 0; i < offset.Length; i++)
            {
                address = ReadPointer(Handle, (i == 0) ? pointer : address) + offset[i];
            }

            IntPtr intPtr;
            ReadProcessMemory(Handle, (IntPtr)address, bytes, sizeof(uint), out intPtr);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static uint ReadPointer(IntPtr Handle, uint pointer)
        {
            try
            {
                byte[] bytes = new byte[32];
                if (pointer >= 0xFFFFFFFF || pointer == 0)
                {
                    return 0;
                }

                IntPtr intPtr;
                ReadProcessMemory(Handle, (IntPtr)pointer, bytes, sizeof(uint), out intPtr);
                return BitConverter.ToUInt32(bytes, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex: " + ex.ToString());
            }
            return 0;
        }

        public static string ReadString(IntPtr Handle, long Address, uint length = 32)
        {
            return ASCIIEncoding.Unicode.GetString(ReadBytes(Handle, Address, length * 2)).Split('\0')[0];
        }

        public static ushort ReadUInt16(IntPtr Handle, long address)
        {
            return BitConverter.ToUInt16(ReadBytes(Handle, address, 2), 0);
        }

        public static uint ReadUInt32(IntPtr Handle, long address)
        {
            return BitConverter.ToUInt32(ReadBytes(Handle, address, sizeof(uint)), 0);
        }

        public static uint ReadUInt32(IntPtr Handle, uint pointer, uint[] offset)
        {
            uint address = ReadPointer(Handle, pointer) + offset[0];
            if (offset.Length > 1)
            {
                for (int i = 1; i < offset.Length; i++)
                {
                    address = ReadPointer(Handle, address) + offset[i];
                }
            }
            return BitConverter.ToUInt32(ReadBytes(Handle, address, 4), 0);
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(
             IntPtr hWndChild,      // handle to window
             IntPtr hWndNewParent   // new parent window
           );

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, uint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern void SetWindowText(IntPtr hWnd, string str);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmd);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize,
           MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress,
            IntPtr dwSize, uint flNewProtect, ref uint lpflOldProtect);

        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WaitForSingleObject(IntPtr Handle, uint Wait);

        public static bool WriteByte(IntPtr Handle, long address, byte val)
        {
            try
            {
                IntPtr bytesWritten;
                int result = WinApi.WriteProcessMemory(Handle, new IntPtr(address), new byte[] { val }, 1, out bytesWritten);
                return result != 0;
            }
            catch { return false; }
        }

        public static bool WriteBytes(IntPtr Handle, long address, byte[] bytes, uint length)
        {
            try
            {
                IntPtr bytesWritten;
                int result = WinApi.WriteProcessMemory(Handle, new IntPtr(address), bytes, length, out bytesWritten);
                return result != 0;
            }
            catch { return false; }
        }

        public static void WriteMem(Process p, int address, long v)
        {
            IntPtr hProc = OpenProcess(ProcessAccessFlags.All, false, p.Id);
            byte[] val = new byte[] { (byte)v };

            int wtf = 0;
            WriteProcessMemory(hProc, new IntPtr(address), val, (uint)val.LongLength, out wtf);

            WinApi.CloseHandle(hProc);
        }

        public static bool WriteOffsetByte(IntPtr Handle, uint pointer, uint[] offset, byte value)
        {
            uint address = ReadPointer(Handle, pointer) + offset[0];
            if (offset.Length > 1)
            {
                for (int i = 1; i < offset.Length; i++)
                {
                    address = ReadPointer(Handle, address) + offset[i];
                }
            }

            return WriteByte(Handle, address, value);
        }

        public static bool WriteOffsetByte(IntPtr Handle, uint pointer, uint offset, byte value)
        {
            uint address = ReadPointer(Handle, pointer) + offset;

            return WriteByte(Handle, address, value);
        }

        public static bool WriteOffsetInt(IntPtr Handle, uint pointer, uint[] offset, int value)
        {
            uint address = ReadPointer(Handle, pointer) + offset[0];
            if (offset.Length > 1)
            {
                for (int i = 1; i < offset.Length; i++)
                {
                    address = ReadPointer(Handle, address) + offset[i];
                }
            }

            return WriteBytes(Handle, address, BitConverter.GetBytes(value), sizeof(int));
        }

        public static bool WriteOffsetInt(IntPtr Handle, uint pointer, uint offset, int value)
        {
            uint address = ReadPointer(Handle, pointer) + offset;

            return WriteBytes(Handle, address, BitConverter.GetBytes(value), sizeof(int));
        }

        public static bool WriteOffsetString(IntPtr Handle, uint pointer, uint[] offset, string str)
        {
            uint address = ReadPointer(Handle, pointer) + offset[0];
            if (offset.Length > 1)
            {
                for (int i = 1; i < offset.Length; i++)
                {
                    address = ReadPointer(Handle, address) + offset[i];
                }
            }
            str += '\0';
            byte[] bytes = System.Text.ASCIIEncoding.Default.GetBytes(str);
            return WriteBytes(Handle, address, bytes, (uint)bytes.Length);
        }

        [DllImport("kernel32.dll")]
        public static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            [In, Out] byte[] buffer, uint size, out IntPtr lpNumberOfBytesWritten);

        public static bool WriteString(IntPtr Handle, long address, string str)
        {
            str += '\0';
            byte[] bytes = System.Text.ASCIIEncoding.Default.GetBytes(str);
            return WriteBytes(Handle, address, bytes, (uint)bytes.Length);
        }
    }
}
