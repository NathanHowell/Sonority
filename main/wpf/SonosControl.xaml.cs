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
using System.Xml.XPath;
using Sonority.UPnP;

namespace wpf
{
    class AlbumArtUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            XPathNavigator nav = (XPathNavigator)value;
            XPathNavigator artNav = nav.SelectSingleNode(Sonority.XPath.Expressions.AlbumArt);
            if (artNav == null)
            {
                return null;
            }

            string art = artNav.Value;
            if (String.IsNullOrEmpty(art))
            {
                return null;
            }

            if (Uri.IsWellFormedUriString(art, UriKind.Absolute))
            {
                // Pandora, etc where the full Uri is already given to use
                return new Uri(art);
            }
            else
            {
                string url = art.Substring(0, art.IndexOf('?'));
                string qs = art.Substring(art.IndexOf('?'));
                UriBuilder builder = new UriBuilder("http", "192.168.1.107", 1400, url, qs);
                return builder.Uri;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

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
            SonosControl tc = (SonosControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.DataContext;
            ZonePlayer zp = zg.Coordinator;
            zp.AVTransport.PlayPause();
            e.Handled = true;
        }

        void MediaCommands_CanTogglePlayPause(object sender, CanExecuteRoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            if (fe.DataContext == null)
            {
                return;
            }

            ZonePlayer zp = (ZonePlayer)fe.DataContext;
            if (zp.AVTransport == null)
            {
                return;
            }

            e.CanExecute = zp.AVTransport.TransportState != TransportState.STOPPED;
            e.Handled = true;
        }

        void MediaCommands_NextTrack(object sender, ExecutedRoutedEventArgs e)
        {
            SonosControl tc = (SonosControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.DataContext;
            ZonePlayer zp = zg.Coordinator;
            zp.AVTransport.Next();
            e.Handled = true;
        }

        void MediaCommands_CanNextTrack(object sender, CanExecuteRoutedEventArgs e)
        {
            SonosControl tc = (SonosControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.DataContext;
            ZonePlayer zp = zg.Coordinator;
            e.CanExecute = zp.AVTransport.CurrentTrack < zp.AVTransport.NumberOfTracks;
            e.Handled = true;
        }

        void MediaCommands_PreviousTrack(object sender, ExecutedRoutedEventArgs e)
        {
            SonosControl tc = (SonosControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.DataContext;
            ZonePlayer zp = zg.Coordinator;
            zp.AVTransport.Previous();
            e.Handled = true;
        }

        void MediaCommands_CanPreviousTrack(object sender, CanExecuteRoutedEventArgs e)
        {
            SonosControl tc = (SonosControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.DataContext;
            ZonePlayer zp = zg.Coordinator;
            e.CanExecute = zp.AVTransport.CurrentTrack > 0;
            e.Handled = true;
        }

        void MediaCommands_MuteVolume(object sender, ExecutedRoutedEventArgs e)
        {
            SonosControl tc = (SonosControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.DataContext;
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
            SonosControl tc = (SonosControl)sender;
            ZoneGroup zg = (ZoneGroup)tc.DataContext;
            ZonePlayer zp = zg.Coordinator;
            zp.RenderingControl.SetMute(Channel.Master, !zp.RenderingControl.Mute[Channel.Master]);
            e.Handled = true;
        }

        void MediaCommands_CanSelect(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}