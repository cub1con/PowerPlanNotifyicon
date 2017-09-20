using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PowerPlanNotifyicon
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new NotifyContext());
        }

        public class NotifyContext : ApplicationContext
        {
            private static NotifyIcon NotifyTrayIcon;

            public NotifyContext()
            {
                NotifyTrayIcon = new NotifyIcon()
                {
                    Icon = GenerateIcon(GetActiveScheme().name, Color.Transparent),
                    //Icon = Properties.Resources.ToolIcon,
                    ContextMenuStrip = new ContextMenuStrip(),
                    Visible = true,
                };

                NotifyTrayIcon.ContextMenuStrip.Items.AddRange(GetAllSchemes().Select(x => new ToolStripMenuItem(x.name, GenerateBitmap(x.name), (o, e) => { SetScheme(x); })).ToArray());
                NotifyTrayIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
                {
                    new ToolStripSeparator(),
                    //new ToolStripMenuItem("Actual PowerPlan",  GenerateBitmap("P", Color.FromArgb(128,128,128)),  ti_actual),
                    new ToolStripMenuItem("PowerOptions",  GenerateBitmap("O", Color.FromArgb(128,128,128)),  Ti_PowerOptions),
                    new ToolStripMenuItem("About", GenerateBitmap("?", Color.FromArgb(128,128,128)), Ti_About),
                    new ToolStripMenuItem("Exit",  GenerateBitmap("X", Color.FromArgb(128,128,128)), Ti_Exit)
                });  
                SetIconText(GetActiveScheme().name);
            }

            private void SetIconText(string SchemeName)
            {
                NotifyTrayIcon.Text = "PowerPlan " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                    Environment.NewLine + SchemeName;
            }

            private void SetScheme(Scheme scheme)
            {
                PPProcess.PowerPlanExec(" -s " + scheme.guid);
                NotifyTrayIcon.Icon = GenerateIcon(scheme.name, Color.Transparent);
                SetIconText(scheme.name);
            }

            private Scheme[] GetAllSchemes()
            {
                var str = PPProcess.PowerPlanExec("/L");
                List<Scheme> schemes = new List<Scheme>();
                using (MemoryStream mem = new MemoryStream(Encoding.GetEncoding(850).GetBytes(str)))
                {
                    using (StreamReader reader = new StreamReader(mem, Encoding.GetEncoding(850)))
                    {
                        string line = null;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains("GUID"))
                                schemes.Add(new Scheme()
                                {
                                    name = line.Split('(')[1].Split(')')[0].Trim(),
                                    guid = line.Split(':')[1].Split('(')[0].Trim()
                                });
                        }
                    }
                }
                return schemes.ToArray();
            }

            struct Scheme
            {
                public string name;
                public string guid;
            }

            void Ti_Exit(object sender, EventArgs e)
            {
                NotifyTrayIcon.Visible = false;
                Environment.Exit(0);
            }

            public static void Ti_About(object sender, EventArgs e)
            {
                MessageBox.Show("Created by Cubicon" + Environment.NewLine + "Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }

            //public static void ti_actual(object sender, EventArgs e) // GET SCHEME NAME
            //{
            //    MessageBox.Show(PPProcess.PowerPlanExec("-getactivescheme").Split('(')[1].Split(')')[0].Trim());
            //}

            private static Icon GenerateIcon(string name)
            {
                return GenerateIcon(name, Color.FromArgb(32, 32, 32));
            }
            private static Icon GenerateIcon(string name, Color background)
            {
                return Icon.FromHandle(GenerateBitmap(name, background).GetHicon());
            }

            private static Bitmap GenerateBitmap(string name)
            {
                return GenerateBitmap(name, Color.FromArgb(32, 32, 32));
            }
            private static Bitmap GenerateBitmap(string name, Color background)
            {
                Bitmap bmp = new Bitmap(16, 16);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    using (SolidBrush b = new SolidBrush(background))
                        g.FillRectangle(b, 0, 0, bmp.Width, bmp.Height);
                    //g.FillRectangle(b, 1, 1, bmp.Width - 2, bmp.Height - 2);

                    using (Font fnt = new Font("Arial", 10/*, FontStyle.Bold*/))
                    {
                        var sz = g.MeasureString(name[0].ToString(), fnt);
                        g.DrawString(name[0].ToString(), fnt, Brushes.White, bmp.Width / 2 - sz.Width / 2, bmp.Height / 2 - sz.Height / 2);
                    }
                }
                return bmp;
            }

            private static Scheme GetActiveScheme()   // GET SCHEME GUID
            {
                var line = PPProcess.PowerPlanExec("-getactivescheme");
                return new Scheme()
                {
                    name = line.Split('(')[1].Split(')')[0].Trim(),
                    guid = line.Split(':')[1].Split('(')[0].Trim()
                };
            }

            public static void Ti_PowerOptions(object sender, EventArgs e)
            {
                Process.Start(fileName: "control", arguments: "/name Microsoft.PowerOptions");
            }

        }

    }

}
