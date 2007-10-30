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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using UPNPLib;

namespace Sonority.UPnP
{
    public class Discover : DispatcherObject, IUPnPDeviceFinderCallback, IDisposable, INotifyPropertyChanged
    {
        public Discover()
        {
            _findData = _finder.CreateAsyncFind("urn:schemas-upnp-org:device:ZonePlayer:1", 0, new DeviceFinderCallback(this));
            _finder.StartAsyncFind(_findData);
        }

        ~Discover()
        {
        }

        public void Dispose()
        {
            _finder.CancelAsyncFind(_findData);
        }

        void IUPnPDeviceFinderCallback.DeviceAdded(int lFindData, UPnPDevice pDevice)
        {
            ZonePlayer zp = new ZonePlayer(pDevice);
            zp.DeviceProperties.PropertyChanged += new PropertyChangedEventHandler(DeviceProperties_PropertyChanged);
            _zonePlayers.Add(zp);
            PropertyChanged(this, new PropertyChangedEventArgs("ZonePlayers"));
        }

        void DeviceProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ZoneName")
            {
                return;
            }

            ZonePlayer[] zonePlayers = new ZonePlayer[_zonePlayers.Count];
            _zonePlayers.CopyTo(zonePlayers, 0);
            Array.Sort(zonePlayers, delegate(ZonePlayer a, ZonePlayer b) { return String.CompareOrdinal(a.DeviceProperties.ZoneName, b.DeviceProperties.ZoneName); });

            for (int i = 0; i < zonePlayers.Length; ++i)
            {
                for (int j = i; j < _zonePlayers.Count; ++j)
                {
                    if (zonePlayers[i] == _zonePlayers[j])
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                        {
                            _zonePlayers.Move(j, i);
                        });
                    }
                }
            }
        }

        void IUPnPDeviceFinderCallback.DeviceRemoved(int lFindData, string bstrUDN)
        {
            ZonePlayer found = null;
            foreach (ZonePlayer zp in _zonePlayers)
            {
                if (zp.UniqueDeviceName == bstrUDN)
                {
                    found = zp;
                    break;
                }
            }

            if (found != null)
            {
                _zonePlayers.Remove(found);
            }

            PropertyChanged(this, new PropertyChangedEventArgs("ZonePlayers"));
        }

        void IUPnPDeviceFinderCallback.SearchComplete(int lFindData)
        {
            // do something?
        }

        public ObservableCollection<ZonePlayer> ZonePlayers
        {
            get
            {
                return _zonePlayers;
            }
        }

        private UPnPDeviceFinder _finder = new UPnPDeviceFinderClass();
        private ObservableCollection<ZonePlayer> _zonePlayers = new ObservableCollection<ZonePlayer>();
        private int _findData;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
