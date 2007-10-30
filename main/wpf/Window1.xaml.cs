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
        }

        void ListKeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Delete)
            {
                ListView lv = args.Source as ListView;
                ZonePlayer zp = lv.DataContext as ZonePlayer;

                // have to make a copy since there will be a callback 
                // each time the queue is modified
                QueueItem[] delete = new QueueItem[lv.SelectedItems.Count];
                lv.SelectedItems.CopyTo(delete, 0);
                // TODO: reverse numeric sort on queue ID
                Array.Sort(delete, delegate(QueueItem a, QueueItem b) { return String.CompareOrdinal(a.ItemID, b.ItemID) * -1; });

                foreach (QueueItem qi in delete)
                {
                    zp.AVTransport.RemoveTrackFromQueue(qi.ItemID);
                }
            }
        }

        void ChangeTransportState(object sender, RoutedEventArgs args)
        {
            Button b = sender as Button;
            ZonePlayer zp = b.DataContext as ZonePlayer;

            if (zp.AVTransport.TransportState != TransportState.PLAYING)
            {
                zp.AVTransport.Play();
            }
            else
            {
                zp.AVTransport.Pause();
            }
        }

        private Discover _discover = new Discover();
    }
}