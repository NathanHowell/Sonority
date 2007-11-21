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
    class AreObjectsEqual : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return Convert.ToUInt32(values[0]) == Convert.ToUInt32(values[1]);
            }
            catch (InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
            }
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    class TrackMetadataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            XPathNavigator nav = (XPathNavigator)value;

            return String.Format("Artist: {0} / Album: {1} / Track: {2}",
                nav.SelectSingleNode(Sonority.XPath.Expressions.Creator).Value,
                nav.SelectSingleNode(Sonority.XPath.Expressions.Album).Value,
                nav.SelectSingleNode(Sonority.XPath.Expressions.Title).Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    /// <summary>
    /// Interaction logic for QueueControl.xaml
    /// </summary>

    public partial class QueueControl : System.Windows.Controls.UserControl
    {
        public QueueControl()
        {
            InitializeComponent();
        }

        void ListDoubleClick(object sender, MouseButtonEventArgs args)
        {
            ListView lv = (ListView)args.Source;
            ZoneGroup zg = (ZoneGroup)lv.DataContext;
            ZonePlayer zp = zg.Coordinator;

            // Sonos queues are 1 based, ListView index is 0 based
            zp.AVTransport.Seek(SeekMode.TRACK_NR, (lv.SelectedIndex + 1).ToString());
            zp.AVTransport.Play();
        }

        void WindowKeyDown(object sender, KeyEventArgs args)
        {
            ListView lv = (ListView)args.Source;
            ZoneGroup zg = (ZoneGroup)lv.DataContext;
            ZonePlayer zp = zg.Coordinator;

            switch (args.Key)
            {
                case Key.Return:
                    zp.AVTransport.Seek(SeekMode.TRACK_NR, (lv.SelectedIndex + 1).ToString());
                    zp.AVTransport.Play();
                    break;
                case Key.MediaNextTrack:
                    zp.AVTransport.Next();
                    break;
                case Key.MediaPlayPause:
                    zp.AVTransport.PlayPause();
                    break;
                case Key.MediaPreviousTrack:
                    zp.AVTransport.Previous();
                    break;
                case Key.MediaStop:
                    zp.AVTransport.Stop();
                    break;
                case Key.VolumeMute:
                    zp.RenderingControl.SetMute(Channel.Master, !zp.RenderingControl.Mute[Channel.Master]);
                    break;
                case Key.VolumeUp:
                    zp.RenderingControl.SetRelativeVolume(Channel.Master, 5);
                    break;
                case Key.VolumeDown:
                    zp.RenderingControl.SetRelativeVolume(Channel.Master, -5);
                    break;
                case Key.Delete:
                    // have to make a copy since there will be a callback 
                    // each time the queue is modified
                    QueueItem[] delete = new QueueItem[lv.SelectedItems.Count];
                    lv.SelectedItems.CopyTo(delete, 0);
                    // TODO: reverse numeric sort on queue ID
                    System.Collections.Comparer cmp = new System.Collections.Comparer(System.Globalization.CultureInfo.InvariantCulture);
                    Array.Sort(delete, delegate(QueueItem a, QueueItem b) { return cmp.Compare(a.NumericID, b.NumericID) * -1; });

                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(delegate
                    {
                        foreach (QueueItem qi in delete)
                        {
                            zp.AVTransport.RemoveTrackFromQueue(qi.ItemID);
                        }
                    }, null);

                    break;
            }

        }

        void ChangeTransportState(object sender, RoutedEventArgs args)
        {
            Button tc = args.OriginalSource as Button;
            ZonePlayer zp = tc.DataContext as ZonePlayer;

            zp.AVTransport.PlayPause();
        }
    }
}