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
using System.Threading;
using UPNPLib;

namespace Sonority.UPnP
{
    public class ZonePlayer
    {
        public ZonePlayer(string uniqueDeviceName)
        {
            _device = FindByUDN(uniqueDeviceName);
            Initialize();
        }

        public ZonePlayer(UPnPDevice device)
        {
            // hack/fix to prevent DisconnectedContext MDA due to the upnp callback threads
            // calling CoUninit and nuking this STA on return. create a new instance instead
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                _device = FindByUDN(device.UniqueDeviceName);
            }
            else
            {
                _device = device;
            }

            Initialize();
        }

        private static UPnPDevice FindByUDN(string uniqueDeviceName)
        {
            UPnPDeviceFinder finder = new UPnPDeviceFinderClass();
            return finder.FindByUDN(uniqueDeviceName);
        }

        private void Initialize()
        {
            _alarmClock = _device.Services["urn:upnp-org:serviceId:AlarmClock"];
            _zoneGroupTopology = _device.Services["urn:upnp-org:serviceId:ZoneGroupTopology"];
            _groupManagment = _device.Services["urn:upnp-org:serviceId:GroupManagement"];

        }

        private void DumpServices()
        {
            foreach (UPnPService service in _device.Services)
            {
                Console.WriteLine(service.ServiceTypeIdentifier);
                Console.WriteLine(service.Id);
            }
        }

        public UPnPDevice MediaServer
        {
            get
            {
                if (_mediaServer == null)
                {
                    _mediaServer = _device.Children[String.Format("{0}_MS", _device.UniqueDeviceName)];
                }
                return _mediaServer;
            }
        }

        public UPnPDevice MediaRenderer
        {
            get
            {
                if (_mediaRenderer == null)
                {
                    _mediaRenderer = _device.Children[String.Format("{0}_MR", _device.UniqueDeviceName)];
                }
                return _mediaRenderer;
            }
        }

        public UPnPService AudioIn
        {
            get
            {
                if (_audioIn == null)
                {
                    _audioIn = _device.Services["urn:upnp-org:serviceId:AudioIn"];
                }
                return _audioIn;
            }
        }

        public SystemProperties SystemProperties
        {
            get
            {
                if (_systemProperties == null)
                {
                    _systemProperties = new SystemProperties(_device.Services["urn:upnp-org:serviceId:SystemProperties"]);
                }
                return _systemProperties;
            }
        }
        public DeviceProperties DeviceProperties
        {
            get
            {
                if (_deviceProperties == null)
                {
                    _deviceProperties = new DeviceProperties(_device.Services["urn:upnp-org:serviceId:DeviceProperties"]);
                }
                return _deviceProperties;
            }
        }
        public AVTransport AVTransport
        {
            get
            {
                if (_avTransport == null)
                {
                    _avTransport = new AVTransport(MediaRenderer.Services["urn:upnp-org:serviceId:AVTransport"]);
                }
                return _avTransport;
            }
        }
        public RenderingControl RenderingControl
        {
            get
            {
                if (_renderingControl == null)
                {
                    _renderingControl = new RenderingControl(MediaRenderer.Services["urn:upnp-org:serviceId:RenderingControl"]);
                }
                return _renderingControl;
            }
        }
        public ContentDirectory ContentDirectory
        {
            get
            {
                if (_contentDirectory == null)
                {
                    _contentDirectory = new ContentDirectory(MediaServer.Services["urn:upnp-org:serviceId:ContentDirectory"]);
                }
                return _contentDirectory;
            }
        }

        public string UniqueDeviceName { get { return _device.UniqueDeviceName; } }

        private UPnPDevice _device;
        private UPnPDevice _mediaServer;
        private UPnPDevice _mediaRenderer;
        private UPnPService _audioIn;
        private UPnPService _alarmClock;
        private SystemProperties _systemProperties;
        private UPnPService _zoneGroupTopology;
        private UPnPService _groupManagment;
        private DeviceProperties _deviceProperties;
        private AVTransport _avTransport;
        private RenderingControl _renderingControl;
        private ContentDirectory _contentDirectory;
    }
}
