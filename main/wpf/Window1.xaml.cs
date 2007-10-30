using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sonority.UPnP;

namespace wpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {
        public Window1()
        {
            InitializeComponent();

            this.tabControl1.ItemsSource = _discover.ZonePlayers;
            this.Closed += new EventHandler(Window1_Closed);
        }

        void Window1_Closed(object sender, EventArgs e)
        {
            _discover.Dispose();
        }

        void ListDoubleClick(object sender, MouseButtonEventArgs args)
        {
            ListView lv = args.Source as ListView;
            ZonePlayer zp = lv.DataContext as ZonePlayer;

            // Sonos queues are 1 based, ListView index is 0 based
            zp.AVTransport.Seek(SeekMode.TRACK_NR, (lv.SelectedIndex+1).ToString());
            zp.AVTransport.Play();
        }

        void ListKeyDown(object sender, KeyEventArgs args)
        {
            ListView lv = args.Source as ListView;
            ZonePlayer zp = lv.DataContext as ZonePlayer;

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

            if (args.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                switch (args.Key)
                {
                    case Key.F:
                        zp.AVTransport.Next();
                        break;
                    case Key.B:
                        zp.AVTransport.Previous();
                        break;
                    case Key.P:
                        zp.AVTransport.PlayPause();
                        break;
                }
            }
        }

        void ChangeTransportState(object sender, RoutedEventArgs args)
        {
            Button b = sender as Button;
            ZonePlayer zp = b.DataContext as ZonePlayer;

            zp.AVTransport.PlayPause();
        }

        private Discover _discover = new Discover();
    }
}