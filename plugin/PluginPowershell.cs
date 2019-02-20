using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Rainmeter;

namespace PluginPowershell
{
    internal class Measure
    {
        Process commandPromptProcess;
        StreamWriter commandPromptWriter;
        public delegate void UpdateTextCallback(string text);

        internal Measure()
        {
        }

        internal void Reload(Rainmeter.API api, ref double maxValue)
        {
        }

        internal double Update()
        {
            if (!hasStarted)
            {
                hasStarted = true;
                init();
            }

            return 0.0;
        }

        internal void ClearWindow()
        {
            commandPromptWriter.Close();
            commandPromptProcess.Close();
        }

        internal void ClearText()
        {
            output = "";
            lines = 0;
        }

        internal void SetLineMax(int max)
        {
            lineMax = max;
        }

        internal string GetString()
        {
            return output;
        }

        internal void RunCMD(string input)
        {
            commandPromptWriter.WriteLine(input);
        }

        internal void init()
        {
            // reference: https://www.codeproject.com/articles/17410/command-prompt-gadget
            commandPromptProcess = new Process();
            commandPromptProcess.StartInfo.FileName = "powershell.exe";
            commandPromptProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            commandPromptProcess.StartInfo.CreateNoWindow = true;
            commandPromptProcess.StartInfo.UseShellExecute = false;
            commandPromptProcess.StartInfo.RedirectStandardOutput = true;
            StringBuilder sortOutput = new StringBuilder("");
            
            commandPromptProcess.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            commandPromptProcess.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            commandPromptProcess.StartInfo.RedirectStandardInput = true;
            
            commandPromptProcess.Start();
            
            commandPromptWriter = commandPromptProcess.StandardInput;
            commandPromptWriter.WriteLine(@"cd c:\");
            commandPromptProcess.BeginOutputReadLine();
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                output += outLine.Data + "\n";

                if (lines > lineMax)
                {
                    int index = output.IndexOf("\n");
                    output = output.Substring(index + 1);
                }
                else
                {
                    lines++;
                }
            }
        }

        private string output = "";
        private int lines = 0;
        private int lineMax = 73;

        private bool hasStarted = false;
    }

    public static class Plugin
    {
        static IntPtr StringBuffer = IntPtr.Zero;

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.ClearWindow();

            GCHandle.FromIntPtr(data).Free();

            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;

            API api = (API)rm;
            int max = api.ReadInt("LineMax", -1);

            if (max > -1)
                measure.SetLineMax(max);

            measure.Reload(new Rainmeter.API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            return measure.Update();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }

            string stringValue = measure.GetString();
            if (stringValue != null)
            {
                StringBuffer = Marshal.StringToHGlobalUni(stringValue);
            }

            return StringBuffer;
        }

        [DllExport]
        public static void ExecuteBang(IntPtr data, [MarshalAs(UnmanagedType.LPWStr)] string args)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.RunCMD(args);
            
            if (args.ToLower() == "clear")
            {
                measure.ClearText();
            }

            if (args.ToLower() == "exit")
            {
                measure.ClearText();
                measure.init();
            }
        }
    }
}
