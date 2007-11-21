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
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Xml;
using System.Xml.XPath;
using UPNPLib;

namespace Sonority.UPnP
{
    public sealed class Discover : DispatcherObject, IUPnPDeviceFinderCallback, IDisposable, INotifyPropertyChanged
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
            GC.SuppressFinalize(this);
            _finder.CancelAsyncFind(_findData);
        }

        void IUPnPDeviceFinderCallback.DeviceAdded(int lFindData, UPnPDevice pDevice)
        {
            Intern(pDevice.UniqueDeviceName);
        }

        private object _internLock = new object();

        public ZonePlayer Intern(string uniqueDeviceName)
        {
            lock (_internLock)
            {
                foreach (ZonePlayer zpi in _zonePlayers)
                {
                    if (String.CompareOrdinal(zpi.UniqueDeviceName, uniqueDeviceName) == 0)
                    {
                        return zpi;
                    }
                }

                ZonePlayer zp = new ZonePlayer(uniqueDeviceName);
                zp.DeviceProperties.PropertyChanged += new PropertyChangedEventHandler(DeviceProperties_PropertyChanged);

                if (_topologyHandled == false)
                {
                    zp.ZoneGroupTopology.PropertyChanged += new PropertyChangedEventHandler(ZoneGroupTopology_PropertyChanged);
                    _topologyHandled = true;
                }

                _zonePlayers.Add(zp);
                PropertyChanged(this, new PropertyChangedEventArgs("ZonePlayers"));
                return zp;
            }
        }

        void ZoneGroupTopology_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ZoneGroupState")
            {
                return;
            }

            ZoneGroupTopology zgt = sender as ZoneGroupTopology;

            XPathDocument doc = new XPathDocument(new StringReader(zgt.ZoneGroupState));
            XPathNavigator nav = doc.CreateNavigator();

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                Dictionary<ZoneGroup, bool> found = new Dictionary<ZoneGroup,bool>();
                foreach (ZoneGroup zg in Topology)
                {
                    found[zg] = false;
                }

                foreach (XPathNavigator node in nav.Select("ZoneGroups/*"))
                {
                    string coordinator = node.SelectSingleNode("@Coordinator").Value;

                    bool newGroup = true;
                    for (int i = 0; i < Topology.Count; ++i)
                    {
                        ZoneGroup zg = Topology[i];

                        if (String.CompareOrdinal(zg.CoordinatorUuid, coordinator) == 0)
                        {
                            newGroup = false;
                            found[zg] = true;

                            Topology.RemoveAt(i);
                            Topology.Insert(i, new ZoneGroup(this, node));
                        }
                    }

                    if (newGroup)
                    {
                        _topology.Add(new ZoneGroup(this, node));
                    }
                }

                foreach (KeyValuePair<ZoneGroup, bool> blah in found)
                {
                    if (blah.Value == false)
                    {
                        _topology.Remove(blah.Key);
                    }
                }
            });
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

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                for (int i = 0; i < zonePlayers.Length; ++i)
                {
                    for (int j = i; j < _zonePlayers.Count; ++j)
                    {
                        if (zonePlayers[i] == _zonePlayers[j])
                        {
                            _zonePlayers.Move(j, i);
                        }
                    }
                }
            });
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

        public ObservableCollection<ZoneGroup> Topology
        {
            get
            {
                return _topology;
            }
        }

        private UPnPDeviceFinder _finder = new UPnPDeviceFinderClass();
        private ObservableCollection<ZonePlayer> _zonePlayers = new ObservableCollection<ZonePlayer>();
        private ObservableCollection<ZoneGroup> _topology = new ObservableCollection<ZoneGroup>();
        private int _findData;
        private bool _topologyHandled = false;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
