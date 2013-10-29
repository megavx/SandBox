using EasyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ThrInj_Con;
namespace ThrInj
{
    public class Main : EasyHook.IEntryPoint
    {
        RemoteMon Interface;

        LocalHook MessageBoxExWHook;
        LocalHook MessageBoxExAHook;
        LocalHook MessageBoxWHook;
        LocalHook MessageBoxAHook;
        LocalHook CreateFileHookW;
        LocalHook CreateFileHookA;
        LocalHook CreateProcessHookA;
        LocalHook CreateProcessHookW;
        LocalHook WriteFileHook;
        LocalHook WriteFileHookEx;
        LocalHook SetWindowsHookExWHook;
        LocalHook SetWindowsHookExAHook;
        //LocalHook WriteProcessMemoryHook;
        LocalHook BitBltHook;

        Stack<String> Queue = new Stack<String>();
        static string ChannelName;
        public Main(RemoteHooking.IContext InContext, String InChannelName)
        {

        }
        public void Run(RemoteHooking.IContext InContext, String InChannelName)
        {
            try
            {
                Interface = RemoteHooking.IpcConnectClient<RemoteMon>(InChannelName);
                ChannelName = InChannelName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            try
            {
                MessageBoxExWHook = LocalHook.Create(LocalHook.GetProcAddress("user32.dll", "MessageBoxExW"), new WindowsAPI.TMessageBoxEx(MessageBoxEx_Hooked), this);
                MessageBoxExWHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                MessageBoxExAHook = LocalHook.Create(LocalHook.GetProcAddress("user32.dll", "MessageBoxExA"), new WindowsAPI.TMessageBoxEx(MessageBoxEx_Hooked), this);
                MessageBoxExAHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                MessageBoxWHook = LocalHook.Create(LocalHook.GetProcAddress("user32.dll", "MessageBoxW"), new WindowsAPI.TMessageBox(MessageBox_Hooked), this);
                MessageBoxWHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                MessageBoxAHook = LocalHook.Create(LocalHook.GetProcAddress("user32.dll", "MessageBoxA"), new WindowsAPI.TMessageBox(MessageBox_Hooked), this);
                MessageBoxAHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                CreateFileHookW = LocalHook.Create(LocalHook.GetProcAddress("kernel32.dll", "CreateFileW"), new WindowsAPI.TCreateFile(CreateFile_Hooked), this);
                CreateFileHookW.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                CreateFileHookA = LocalHook.Create(LocalHook.GetProcAddress("kernel32.dll", "CreateFileA"), new WindowsAPI.TCreateFile(CreateFile_Hooked), this);
                CreateFileHookA.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                CreateProcessHookW = LocalHook.Create(LocalHook.GetProcAddress("kernel32.dll", "CreateProcessW"), new WindowsAPI.TCreateProcess(CreateProcess_Hooked), this);
                CreateProcessHookW.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                CreateProcessHookA = LocalHook.Create(LocalHook.GetProcAddress("kernel32.dll", "CreateProcessA"), new WindowsAPI.TCreateProcess(CreateProcess_Hooked), this);
                CreateProcessHookA.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                WriteFileHook = LocalHook.Create(LocalHook.GetProcAddress("kernel32.dll", "WriteFile"), new WindowsAPI.TWriteFile(WriteFile_Hooked), this);
                WriteFileHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                WriteFileHookEx = LocalHook.Create(LocalHook.GetProcAddress("kernel32.dll", "WriteFileEx"), new WindowsAPI.TWriteFileEx(WriteFileEx_Hooked), this);
                WriteFileHookEx.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                SetWindowsHookExWHook = LocalHook.Create(LocalHook.GetProcAddress("user32.dll", "SetWindowsHookExW"), new WindowsAPI.TSetWindowsHookEx(SetWindowsHookEx_Hooked), this);
                SetWindowsHookExWHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                SetWindowsHookExAHook = LocalHook.Create(LocalHook.GetProcAddress("user32.dll", "SetWindowsHookExA"), new WindowsAPI.TSetWindowsHookEx(SetWindowsHookEx_Hooked), this);
                SetWindowsHookExAHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                BitBltHook = LocalHook.Create(LocalHook.GetProcAddress("gdi32.dll", "BitBlt"), new WindowsAPI.TBitBlt(BitBlt_Hooked), this);
                BitBltHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            }
            catch (Exception ExtInfo)
            {
                Interface.ReportException(ExtInfo);
                throw ExtInfo;
            }

            Interface.IsInstalled(RemoteHooking.GetCurrentProcessId());
            RemoteHooking.WakeUpProcess();
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
        

        static IntPtr MessageBoxEx_Hooked(IntPtr hWnd, String lpText, String lpCaption, UInt32 uType, UInt32 LanguageID)
        {
            //Communicate via File-Pipe
            Main callback = (Main)HookRuntimeInfo.Callback;
            if (!callback.Interface.OnMessageBox(lpText, lpCaption))
            {
                WindowsAPI.SetLastError(0x5);
                return IntPtr.Zero;
            }
                // call original API...
            return WindowsAPI.MessageBoxEx(
                    hWnd,
                    lpText,
                    lpCaption,
                    uType,
                    LanguageID
                );
        }

        static IntPtr MessageBox_Hooked(IntPtr hWnd, String lpText, String lpCaption, UInt32 uType)
        {
            //Communicate via File-Pipe
            Main callback = (Main)HookRuntimeInfo.Callback;
            if (!callback.Interface.OnMessageBox(lpText, lpCaption))
            {
                WindowsAPI.SetLastError(0x5);
                return IntPtr.Zero;
            }
            // call original API...
            return WindowsAPI.MessageBox(
                    hWnd,
                    lpText,
                    lpCaption,
                    uType
                );
        }

        static IntPtr CreateFile_Hooked(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCD, UInt32 dwFlags, IntPtr hTemplateFile)
        {
            //Communicate via File-Pipe
            Main callback = (Main)HookRuntimeInfo.Callback;
            if (!callback.Interface.OnCreateFile(lpFileName))
            {
                WindowsAPI.SetLastError(0x5);
                return IntPtr.Zero;
            }
            // call original API...
            IntPtr crte = WindowsAPI.CreateFile(lpFileName,dwDesiredAccess,dwShareMode,lpSecurityAttributes, dwCD, dwFlags, hTemplateFile);
            callback.Interface.UpdateCreateFileHandle(crte, lpFileName);
            return crte;
        }

        static bool CreateProcess_Hooked(String AppName, String CommandLine, UInt32 ProcessAttr, UInt32 ThreadAttr, bool InheritHandles, UInt32 CreationFlags,
            IntPtr Environment, String CurrDir, WindowsAPI.STARTUPINFO lpStartupInfo, WindowsAPI.PROCESS_INFORMATION lpProcessInformation)
        {
            Main callback = (Main)HookRuntimeInfo.Callback;
            bool x = callback.Interface.OnCreateProcess(AppName, CommandLine, CurrDir);
            if (!x)
            {
                WindowsAPI.SetLastError(5);
                return false;
            }
            // call original API...
            return true;
        }

        static bool BitBlt_Hooked(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,int nXSrc, int nYSrc, long dwRop)
        {
            Main callback = (Main)HookRuntimeInfo.Callback;
            bool x = callback.Interface.OnBitBlt(dwRop);
            if (!x)
            {
                WindowsAPI.SetLastError(5);
                return false;
            }
            // call original API...
            return WindowsAPI.BitBlt(hdcDest, nXDest, nYDest, nWidth, nHeight, hdcSrc, nXSrc, nYSrc, dwRop);
        }

        static bool SetWindowsHookEx_Hooked(int idHook, IntPtr lpfn, IntPtr hMod, uint dwThreadId)
        {
            Main callback = (Main)HookRuntimeInfo.Callback;
            bool x = callback.Interface.SetWindowsHookEx(idHook, (int)dwThreadId, (int)hMod);
            if (!x)
            {
                WindowsAPI.SetLastError(5);
                return false;
            }
            // call original API...
            return WindowsAPI.SetWindowsHookEx(idHook, lpfn, hMod, dwThreadId);
        }

        static int WriteFile_Hooked(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped)
        {
            Main callback = (Main)HookRuntimeInfo.Callback;
            bool x = callback.Interface.WriteFile(hFile, lpBuffer, (int)nNumberOfBytesToWrite);
            if (!x)
            {
                WindowsAPI.SetLastError(5);
                lpNumberOfBytesWritten = 0;
                return 0;
            }
            // call original API...
            return WindowsAPI.WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, out lpNumberOfBytesWritten, lpOverlapped);
        }

        static int WriteFileEx_Hooked(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, IntPtr lpOverlapped, IntPtr lpCompletionRoutine)
        {
            Main callback = (Main)HookRuntimeInfo.Callback;
            bool x = callback.Interface.WriteFile(hFile, lpBuffer, (int)nNumberOfBytesToWrite);
            if (!x)
            {
                WindowsAPI.SetLastError(5);
                return 0;
            }
            // call original API...
            return WindowsAPI.WriteFileEx(hFile, lpBuffer, nNumberOfBytesToWrite, lpOverlapped, lpCompletionRoutine);
        }
    }
}
