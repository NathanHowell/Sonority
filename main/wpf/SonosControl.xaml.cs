//
// Copyright (c) 2007 Sonority
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

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