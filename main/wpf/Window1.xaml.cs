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
            ZonePlayer zp = (ZonePlayer)tc.SelectedItem;
            zp.AVTransport.PlayPause();
        }

        void MediaCommands_CanTogglePlayPause(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void MediaCommands_Next(object sender, ExecutedRoutedEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            ZonePlayer zp = (ZonePlayer)tc.SelectedItem;
            zp.AVTransport.Next();
            e.Handled = true;
        }

        void MediaCommands_CanNext(object sender, CanExecuteRoutedEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            ZonePlayer zp = (ZonePlayer)tc.SelectedItem;
            e.CanExecute = zp.AVTransport.CurrentTrack < zp.AVTransport.NumberOfTracks;
            e.Handled = true;
        }

        void ZoneVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ushort volume = (ushort)Math.Round(e.NewValue);
            Slider s = e.Source as Slider;
            ZonePlayer zp = s.DataContext as ZonePlayer;
            s.Delay = 100;
            if (volume != zp.RenderingControl.Volume[Channel.Master])
            {
                zp.RenderingControl.SetVolume(Channel.Master, volume);
            }
            e.Handled = true;
        }

        void ProgressBar_Initialized(object sender, EventArgs e)
        {
            ProgressBar pb = sender as ProgressBar;
            ZonePlayer zp = pb.DataContext as ZonePlayer;
            zp.AVTransport.PropertyChanged += new PropertyChangedEventHandler(AVTransport_PropertyChanged);
            _avpb[zp.AVTransport] = pb;
        }

        Dictionary<AVTransport, ProgressBar> _avpb = new Dictionary<AVTransport, ProgressBar>();

        void AVTransport_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "TransportState")
            {
                return;
            }

            AVTransport av = sender as AVTransport;
            bool enable = av.TransportState == TransportState.PLAYING;
            ProgressBar pb = _avpb[av];

            pb.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (ThreadStart)delegate
            {
                pb.IsEnabled = enable;
            });
        }

        private Discover _discover = new Discover();
    }
}