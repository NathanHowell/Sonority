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
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.XPath;
using Sonority.UPnP;

namespace wpf
{
    /// <summary>
    /// Interaction logic for QueueControl.xaml
    /// </summary>

    public partial class QueueControl : UserControl
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

            // make sure it's playing off the queue before we select a queue track. might be a callback that can be
            // used instead of this explicit call, but this works for now. sort of hacky but it looks like the Sonos client
            // does the same thing... what can you do? :-(
            zp.AVTransport.SetAVTransportUri(zp.QueueUri, "");

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
                case Key.Delete:
                    // have to make a copy since there will be a callback 
                    // each time the queue is modified
                    QueueItem[] delete = new QueueItem[lv.SelectedItems.Count];
                    lv.SelectedItems.CopyTo(delete, 0);

                    // reverse numeric sort on queue ID
                    Array.Sort(delete, delegate(QueueItem a, QueueItem b) { return Comparer<int>.Default.Compare(a.NumericId, b.NumericId) * -1; });

                    // deletes must be synchronous & in order to work properly
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        foreach (QueueItem qi in delete)
                        {
                            zp.AVTransport.RemoveTrackFromQueue(qi.ItemId);
                        }
                    });

                    args.Handled = true;
                    break;
            }
        }
    }
}