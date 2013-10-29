using EasyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;

namespace ThrInj_Con
{
    class Program
    {
        public static String ChannelName = null;
        public static string currdir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        static void Main(string[] args)
        {
            try
            {
                
                /*Config.Register(
                    "ThrInj",
                    currdir + "ThrInj_Con.exe",
                    currdir + "ThrInj.dll");*/
                //args = new string[] { "TestApp.exe" };
                RemoteHooking.IpcCreateServer<RemoteMon>(
                     ref ChannelName, WellKnownObjectMode.Singleton);
                int processid;
                
                //RemoteHooking.CreateAndInject(args[0], "", 0, currdir + "ThrInj.dll", currdir + "ThrInj.dll", out processid, ChannelName);
                ProcessStartInfo f = new ProcessStartInfo();
                f.FileName = args[0];
                Process x = Process.Start(f);
                RemoteHooking.Inject(x.Id, InjectionOptions.DoNotRequireStrongName, currdir + "ThrInj.dll", currdir + "ThrInj.dll", new Object[] { ChannelName });
            }
            catch (Exception ExtInfo)
            {
                Console.WriteLine("There was an error while connecting " +
                                  "to target:\r\n{0}", ExtInfo.ToString());
                Console.ReadLine();
            }

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
    public class RemoteMon : MarshalByRefObject
    {
        public void IsInstalled(int InClientPID)
        {
            Console.WriteLine("Successfully injected into PID {0}", InClientPID);
        }

        public void ReportException(Exception InInfo)
        {
            Console.WriteLine("Exception!: {0}", InInfo.ToString());
        }

        public bool OnMessageBox(string text, string caption)
        {
            //ask user for interaction.
            Console.WriteLine("Trying to open messagebox '{0}' '{1}', allow?", text, caption);
            string x = Console.ReadLine();
            if (x == "y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool CreateFil_R = false;
        public void UpdateCreateFileHandle(IntPtr handle, string Filename)
        {
            FileHandles.Add(handle.ToInt32(), Filename);
        }
        public bool OnCreateFile(string filename)
        {
            if (CreateFil_R == true) return true;
            //ask user for interaction.
            Console.WriteLine("Process is Trying to Open the File '{0}', allow?", filename);
            string x = Console.ReadLine();
            if (x == "y")
            {
                return true;
            }
            else if (x == "r")
            {
                CreateFil_R = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool OnCreateProcess(string appname, string commandline, string environment)
        {
            //ask user for interaction.
            Console.WriteLine("The process is trying to start a new process. This process will be sandboxed if Allowed. FilePath: {0}, allow?", (appname == commandline ? appname : "\"" + appname + "\", CommandLine: \"" + commandline + "\""));
            string x = Console.ReadLine();
            if (x == "y")
            {
                ProcessStartInfo b = new ProcessStartInfo();
                b.Arguments = commandline;
                b.FileName = appname;
                b.WorkingDirectory = environment;
                Process w = Process.Start(b);
                RemoteHooking.Inject(w.Id, InjectionOptions.DoNotRequireStrongName, Program.currdir + "ThrInj.dll", Program.currdir + "ThrInj.dll", new Object[] { Program.ChannelName });
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool bitblt = false;
        public bool OnBitBlt(long dwRop)
        {
            if ((ulong)dwRop != 0x00CC0020 && (ulong)dwRop != 0x4) return true;
            if (bitblt) return true;
            //ask user for interaction.
            Console.WriteLine("The process is trying to copy the screen with BitBlt->SRCCOPY/CAPTUREBLT, allow?");
            string x = Console.ReadLine();
            if (x == "y")
            {
                return true;
            }
            else if (x == "r")
            {
                bitblt = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool writefile = false;
        public static SortedList<int, string> FileHandles;
        public bool WriteFile(IntPtr hFile, byte[] buff, int bytestowrite)
        {
            if (writefile) return true;
            //ask user for interaction.
            string FilePath = "";
            if(FileHandles.ContainsKey(hFile.ToInt32()))
                FilePath = FileHandles[hFile.ToInt32()];
            Console.WriteLine("The process is trying to write to a file. \n{0}FileHandle: {1}\nSize: {2}\nData: {3}", (FilePath != "" ? "FilePath: " + FilePath + "\n" : ""), hFile.ToInt32(), bytestowrite, Encoding.Default.GetString(buff));
            string x = Console.ReadLine();
            if (x == "y")
            {
                return true;
            }
            else if (x == "r")
            {
                writefile = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetWindowsHookEx(int IdHook, int ThreadID, int hModule)
        {
            string IdHookStr = "UNKNOWN";
            switch (IdHook)
            {
                case -1: IdHookStr = "WH_MSGFILTER: Window Message Hook"; break;
                case 0: IdHookStr = "WH_JOURNALRECORD: Window Message Hook"; break;
                case 1: IdHookStr = "WH_JOURNALPLAYBACK: Window Message Hook"; break;
                case 2: IdHookStr = "WH_KEYBOARD: Hotkeys/Keylogger"; break;
                case 3: IdHookStr = "WH_GETMESSAGE: Window Message Hook"; break;
                case 4: IdHookStr = "WH_CALLWNDPROC: Window Message Hook"; break;
                case 5: IdHookStr = "WH_CBT: Window Message Hook"; break;
                case 6: IdHookStr = "WH_SYSMSGFILTER: Window Message Hook"; break;
                case 7: IdHookStr = "WH_MOUSE: Mouse Hook"; break;
                case 9: IdHookStr = "WH_DEBUG: Window Debug Message Hook"; break;
                case 10: IdHookStr = "WH_SHELL: Window Notification Hook"; break;
                case 11: IdHookStr = "WH_FOREGROUNDIDLE: Window Foreground Hook"; break;
                case 12: IdHookStr = "WH_CALLWNDPROCRET: Window Message Hook"; break;
                case 13: IdHookStr = "WH_KEYBOARD_LL: Hotkeys/Keylogger"; break;
            }
            Console.WriteLine("The process is trying to hook \"{0}\". Allow?", IdHookStr);
            string x = Console.ReadLine();
            if (x == "y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
