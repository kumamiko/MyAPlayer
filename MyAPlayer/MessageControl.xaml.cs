using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyAPlayer
{
    /// <summary>
    /// MessageShow.xaml 的交互逻辑
    /// </summary>
    public sealed partial class MessageControl : UserControl
    {
        public MessageControl()
        {
            this.InitializeComponent();
        }

        public void ShowMsg(string content)
        {
            txt_GG.Text = content;
            Storyboard sb = grid_GG.FindResource("ShowAnimation") as Storyboard;
            sb.Begin();
        }
    }
}