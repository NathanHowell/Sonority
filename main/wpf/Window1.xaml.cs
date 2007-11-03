using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
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

            this.tabControl1.ItemsSource = _discover.Topology;
            this.Closed += new EventHandler(Window1_Closed);


        }

        void Window1_Closed(object sender, EventArgs e)
        {
            _discover.Dispose();
        }

        void ListDoubleClick(object sender, MouseButtonEventArgs args)
        {
            ListView lv = (ListView)args.Source;
            ZoneGroup zg = (ZoneGroup)lv.DataContext;
            ZonePlayer zp = zg.Coordinator;

            // Sonos queues are 1 based, ListView index is 0 based
            zp.AVTransport.Seek(SeekMode.TRACK_NR, (lv.SelectedIndex+1).ToString());
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
                TabControl tc = (TabControl)sender;
                ZoneGroup zg = (ZoneGroup)tc.SelectedItem;
                ZonePlayer zp = zg.Coordinator;
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

        private Discover _discover = new Discover();
    }
}