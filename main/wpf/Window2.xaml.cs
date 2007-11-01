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
using System.Windows.Shapes;

namespace wpf
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>

    public partial class Window2 : System.Windows.Window
    {

        public Window2()
        {
            InitializeComponent();
            this.listView1.ItemsSource = disc.Topology;
        }

        Sonority.UPnP.Discover disc = new Sonority.UPnP.Discover();
    }
}