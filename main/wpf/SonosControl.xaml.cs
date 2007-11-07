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
    /// Interaction logic for SonosControl.xaml
    /// </summary>

    public partial class SonosControl : System.Windows.Controls.UserControl
    {
        public SonosControl()
        {
            InitializeComponent();
        }

        void MediaCommands_TogglePlayPause(object sender, ExecutedRoutedEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.SelectedItem;
            ZonePlayer zp = zg.Coordinator;
            zp.AVTransport.PlayPause();
            e.Handled = true;
        }

        void MediaCommands_CanTogglePlayPause(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                FrameworkElement fe = (FrameworkElement)e.OriginalSource;
                ZonePlayer zp = (ZonePlayer)fe.DataContext;
                e.CanExecute = zp.AVTransport.TransportState != TransportState.STOPPED;
                e.Handled = true;
            }
            catch (NullReferenceException)
            {
                // wpf likes to render the button before the zoneplayer properties
                // have filled in, 
            }
        }

        void MediaCommands_NextTrack(object sender, ExecutedRoutedEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.SelectedItem;
            ZonePlayer zp = zg.Coordinator;
            zp.AVTransport.Next();
            e.Handled = true;
        }

        void MediaCommands_CanNextTrack(object sender, CanExecuteRoutedEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.SelectedItem;
            ZonePlayer zp = zg.Coordinator;
            e.CanExecute = zp.AVTransport.CurrentTrack < zp.AVTransport.NumberOfTracks;
            e.Handled = true;
        }

        void MediaCommands_PreviousTrack(object sender, ExecutedRoutedEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.SelectedItem;
            ZonePlayer zp = zg.Coordinator;
            zp.AVTransport.Previous();
            e.Handled = true;
        }

        void MediaCommands_CanPreviousTrack(object sender, CanExecuteRoutedEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.SelectedItem;
            ZonePlayer zp = zg.Coordinator;
            e.CanExecute = zp.AVTransport.CurrentTrack > 0;
            e.Handled = true;
        }

        void MediaCommands_MuteVolume(object sender, ExecutedRoutedEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.SelectedItem;
            ZonePlayer zp = zg.Coordinator;
            zp.RenderingControl.SetMute(Channel.Master, !zp.RenderingControl.Mute[Channel.Master]);
            e.Handled = true;
        }

        void MediaCommands_CanMuteVolume(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void MediaCommands_Select(object sender, ExecutedRoutedEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.SelectedItem;
            ZonePlayer zp = zg.Coordinator;
            zp.RenderingControl.SetMute(Channel.Master, !zp.RenderingControl.Mute[Channel.Master]);
            e.Handled = true;
        }

        void MediaCommands_CanSelect(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void ZoneVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ushort volume = (ushort)Math.Round(e.NewValue);
            Slider s = (Slider)e.Source;
            ZonePlayer zp = (ZonePlayer)s.DataContext;
            s.Delay = 100;
            if (volume != zp.RenderingControl.Volume[Channel.Master])
            {
                zp.RenderingControl.SetVolume(Channel.Master, volume);
            }
            e.Handled = true;
        }
    }
}