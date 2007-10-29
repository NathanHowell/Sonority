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

            this.tabControl1.ItemsSource = disc.ZonePlayers;
            disc.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
            disc.Start();
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("Changed: {0}.{1} -> {2}", sender.GetType().Name, e.PropertyName, sender.GetType().GetProperty(e.PropertyName).GetValue(sender, null));
            Discover disc = (Discover)sender;
            //disc.ZonePlayers[disc.ZonePlayers.Count - 1].ContentDirectory.UpdateQueue();
            //disc.ZonePlayers[disc.ZonePlayers.Count - 1].ContentDirectory.PropertyChanged += new PropertyChangedEventHandler(ContentDirectory_PropertyChanged);
            //disc.ZonePlayers[disc.ZonePlayers.Count - 1].AVTransport.PropertyChanged += new PropertyChangedEventHandler(AVTransport_PropertyChanged);
        }

        void AVTransport_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentTrack")
            {
                Console.WriteLine(tabControl1.Items[0]);
            }
        }

        void ContentDirectory_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ContainerUpdateIDs")
            {
                return;
            }

            ContentDirectory cd = (ContentDirectory)sender;
            if (cd.ContainerUpdateIDs.Contains("Q:0,") == false)
            {
                return;
            }

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate
            {
                //cd.UpdateQueue();
            });
        }

        private Discover disc = new Discover();
    }
}