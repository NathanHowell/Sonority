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
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Sonority.UPnP;

namespace wpf
{
    /// <summary>
    /// Interaction logic for VolumeControl.xaml
    /// </summary>

    public partial class VolumeControl : UserControl, IDisposable
    {
        public VolumeControl()
        {
            InitializeComponent();
            _setVolumeTimer = new Timer(new TimerCallback(SetVolumeCallback));

            _disc = ((wpf.App)Application.Current).Discover;
        }

        void IDisposable.Dispose()
        {
            _setVolumeTimer.Dispose();
            GC.SuppressFinalize(this);
        }

        private void SetVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ushort volume = (ushort)Math.Round(e.NewValue);
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            ZonePlayer zp = (ZonePlayer)fe.DataContext;

            _volumes[zp.UniqueDeviceName] = volume;
            _setVolumeTimer.Change(50, 0);
            e.Handled = true;
        }

        private void SetVolumeCallback(object state)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                foreach (KeyValuePair<string, ushort> volume in _volumes)
                {
                    ZonePlayer zp = _disc.Intern(volume.Key);
                    if (volume.Value != zp.RenderingControl.Volume[Channel.Master])
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            zp.RenderingControl.SetVolume(Channel.Master, volume.Value);
                        });
                    }
                }

                _volumes.Clear();
            });
        }

        private Discover _disc;
        private Dictionary<string, ushort> _volumes = new Dictionary<string, ushort>();
        private Timer _setVolumeTimer;
    }
}