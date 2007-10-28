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
using UPNPLib;

namespace Sonority.UPnP
{
    public class Discover : IUPnPDeviceFinderCallback, IDisposable, INotifyPropertyChanged, IEnumerable<ZonePlayer>
    {
        public Discover()
        {
            _findData = _finder.CreateAsyncFind("urn:schemas-upnp-org:device:ZonePlayer:1", 0, new DeviceFinderCallback(this));
        }

        ~Discover()
        {
            _finder.CancelAsyncFind(_findData);
        }

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Start()
        {
            _finder.StartAsyncFind(_findData);
        }

        void IUPnPDeviceFinderCallback.DeviceAdded(int lFindData, UPnPDevice pDevice)
        {
            _zonePlayers.Add(new ZonePlayer(pDevice));
            PropertyChanged(this, new PropertyChangedEventArgs("ZonePlayers"));
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

        IEnumerator<ZonePlayer> IEnumerable<ZonePlayer>.GetEnumerator()
        {
            foreach (ZonePlayer zp in _zonePlayers)
            {
                yield return zp;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<ZonePlayer> e = this;
            return e.GetEnumerator();
        }
    }
}
