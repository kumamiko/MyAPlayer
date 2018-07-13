using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyAPlayer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Process process = Process.GetCurrentProcess();

            foreach (Process p in Process.GetProcessesByName(process.ProcessName))
            {
                if (p.Id != process.Id)
                {
                    //关闭第二个启动的程序
                    //MessageBox.Show("您的程序已经启动！");
                    Application.Current.Shutdown();
                    return;
                }
            }
            base.OnStartup(e);
        }
    }
}
