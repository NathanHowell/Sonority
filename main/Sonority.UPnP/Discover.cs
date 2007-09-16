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
using System.ComponentModel;
using UPNPLib;

namespace Sonority.UPnP
{
    public class Discover : IUPnPDeviceFinderCallback, IDisposable, INotifyPropertyChanged
    {
        public Discover()
        {
            findData = finder.CreateAsyncFind("urn:schemas-upnp-org:device:ZonePlayer:1", 0, new DeviceFinderCallback(this));
        }

        ~Discover()
        {
            finder.CancelAsyncFind(findData);
        }

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        UPnPDeviceFinder finder = new UPnPDeviceFinderClass();

        public void Start()
        {
            finder.StartAsyncFind(findData);
        }

        void IUPnPDeviceFinderCallback.DeviceAdded(int lFindData, UPnPDevice pDevice)
        {
            _zonePlayers.Add(new ZonePlayer(pDevice));
            PropertyChanged(this, new PropertyChangedEventArgs("ZonePlayers"));
        }

        void IUPnPDeviceFinderCallback.DeviceRemoved(int lFindData, string bstrUDN)
        {
            _zonePlayers.RemoveAll(delegate(ZonePlayer zp) { return zp.UniqueDeviceName == bstrUDN; });
            PropertyChanged(this, new PropertyChangedEventArgs("ZonePlayers"));
        }

        void IUPnPDeviceFinderCallback.SearchComplete(int lFindData)
        {
            // do something?
        }

        public List<ZonePlayer> ZonePlayers
        {
            get
            {
                return _zonePlayers;
            }
        }

        private List<ZonePlayer> _zonePlayers = new List<ZonePlayer>();
        public event PropertyChangedEventHandler PropertyChanged;

        private int findData;
    }
}
