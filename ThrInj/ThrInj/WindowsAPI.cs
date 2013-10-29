using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ThrInj
{
    class WindowsAPI
    {
        #region MessageBox

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr MessageBox(IntPtr hWnd, String lpText, String lpCaption, UInt32 uType);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr TMessageBox(IntPtr hWnd, String lpText, String lpCaption, UInt32 uType);

        #endregion

        #region MessageBoxEx

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr MessageBoxEx(IntPtr hWnd, String lpText, String lpCaption, UInt32 uType, UInt32 LanguageID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr TMessageBoxEx(IntPtr hWnd, String lpText, String lpCaption, UInt32 uType, UInt32 LanguageID);

        #endregion

        #region CreateFile

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCD, UInt32 dwFlags, IntPtr hTemplateFile);

        [UnmanagedFunctionPointer(CallingConvention.StdCall,CharSet = CharSet.Unicode,SetLastError = true)]
        public delegate IntPtr TCreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCD, UInt32 dwFlags, IntPtr hTemplateFile);
        
        #endregion

        #region CreateProcess

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcess(String AppName, String CommandLine, UInt32 ProcessAttr, UInt32 ThreadAttr, bool InheritHandles, UInt32 CreationFlags,
            IntPtr Environment, String CurrDir, STARTUPINFO lpStartupInfo, PROCESS_INFORMATION lpProcessInformation);
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool TCreateProcess(String AppName, String CommandLine, UInt32 ProcessAttr, UInt32 ThreadAttr, bool InheritHandles, UInt32 CreationFlags,
            IntPtr Environment, String CurrDir, STARTUPINFO lpStartupInfo, PROCESS_INFORMATION lpProcessInformation);

        #endregion

        #region BitBlt

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, long dwRop);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public delegate bool TBitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, long dwRop);

        #endregion

        #region SetWindowsHookEx

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetWindowsHookEx(int idHook, IntPtr lpfn, IntPtr hMod, uint dwThreadId);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool TSetWindowsHookEx(int idHook, IntPtr lpfn, IntPtr hMod, uint dwThreadId);

        #endregion

        #region WriteFile

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int WriteFile(IntPtr hFile, [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int WriteFileEx(IntPtr hFile, [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer, uint nNumberOfBytesToWrite, IntPtr lpOverlapped, IntPtr lpCompletionRoutine);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int TWriteFile(IntPtr hFile, [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int TWriteFileEx(IntPtr hFile, [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer, uint nNumberOfBytesToWrite, IntPtr lpOverlapped, IntPtr lpCompletionRoutine);

        #endregion

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern void SetLastError(UInt32 dwErrCode);

        #region Structs
        public struct STARTUPINFO {
            UInt32  cb;
            String lpReserved;
            String lpDesktop;
            String lpTitle;
            UInt32  dwX;
            UInt32  dwY;
            UInt32  dwXSize;
            UInt32  dwYSize;
            UInt32  dwXCountChars;
            UInt32  dwYCountChars;
            UInt32  dwFillAttribute;
            UInt32  dwFlags;
            UInt32   wShowWindow;
            UInt32   cbReserved2;
            Byte lpReserved2;
            IntPtr hStdInput;
            IntPtr hStdOutput;
            IntPtr hStdError;
        };
        
        public struct PROCESS_INFORMATION {
          IntPtr hProcess;
          IntPtr hThread;
          UInt32  dwProcessId;
          UInt32  dwThreadId;
        }
        #endregion
    }
}
