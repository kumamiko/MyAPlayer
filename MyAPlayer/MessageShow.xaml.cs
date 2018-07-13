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
    public sealed partial class MessageShow : UserControl
    {
        public MessageShow(string content, int showTime, bool display)
        {
            this.InitializeComponent();

            Show(content, showTime, display);
        }

        public async void Show(string content, int showTime, bool display)
        {
            if (!display) return;
            grid_GG.Visibility = Visibility.Visible;
            DoubleAnimation dLoginFadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.2)));
            grid_GG.BeginAnimation(UIElement.OpacityProperty, dLoginFadeIn);
            txt_GG.Text = content;
            await Task.Delay(showTime/2);
            DoubleAnimation dLoginFadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.2)));
            grid_GG.BeginAnimation(UIElement.OpacityProperty, dLoginFadeOut);
            await Task.Delay(showTime/2);
            Storyboard_Completed(null,null);
        }

        private void Storyboard_Completed(object sender, object e)
        {
            grid_GG.Visibility = Visibility.Collapsed;
        }
    }
}