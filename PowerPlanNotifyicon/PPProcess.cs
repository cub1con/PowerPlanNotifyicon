using System;
using System.Diagnostics;
using System.Text;

namespace PowerPlanNotifyicon
{
    class PPProcess
    {
        public static string PowerPlanExec(string argument)
        {

            Process proc = new Process();
            proc.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(850);
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.FileName = "powercfg";
            proc.StartInfo.Arguments = argument;
            proc.Start();
            proc.WaitForExit();


            return Convert.ToString(proc.StandardOutput.ReadToEnd());
        }
    }
}
