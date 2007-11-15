using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sonority.UPnP;

namespace wpf
{
    /// <summary>
    /// Interaction logic for VolumeControl.xaml
    /// </summary>

    public partial class VolumeControl : System.Windows.Controls.UserControl
    {
        public VolumeControl()
        {
            InitializeComponent();
        }

        void SetVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ushort volume = (ushort)Math.Round(e.NewValue);
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            ZonePlayer zp = (ZonePlayer)fe.DataContext;
            if (volume != zp.RenderingControl.Volume[Channel.Master])
            {
                zp.RenderingControl.SetVolume(Channel.Master, volume);
                e.Handled = true;
            }
        }
    }
}