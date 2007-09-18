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
using UPNPLib;

namespace Sonority.UPnP
{
    public class ZonePlayer
    {
        public ZonePlayer(string uniqueDeviceName)
        {
            UPnPDeviceFinder finder = new UPnPDeviceFinderClass();
            _device = finder.FindByUDN(uniqueDeviceName);
            Initialize();
        }

        public ZonePlayer(UPnPDevice device)
        {
            _device = device;
            Initialize();
        }

        // lazy construction is better but do it this way until the MDA ContextDisconnect crash gets tracked down
        private void Initialize()
        {
            _mediaServer = _device.Children[String.Format("{0}_MS", _device.UniqueDeviceName)];
            _mediaRenderer = _device.Children[String.Format("{0}_MR", _device.UniqueDeviceName)];

            _audioIn = _device.Services["urn:upnp-org:serviceId:AudioIn"];
            _alarmClock = _device.Services["urn:upnp-org:serviceId:AlarmClock"];
            _deviceProperties = new DeviceProperties(_device.Services["urn:upnp-org:serviceId:DeviceProperties"]);
            _systemProperties = new SystemProperties(_device.Services["urn:upnp-org:serviceId:SystemProperties"]);
            _zoneGroupTopology = _device.Services["urn:upnp-org:serviceId:ZoneGroupTopology"];
            _groupManagment = _device.Services["urn:upnp-org:serviceId:GroupManagement"];

            _avTransport = new AVTransport(MediaRenderer.Services["urn:upnp-org:serviceId:AVTransport"]);
            _renderingControl = new RenderingControl(MediaRenderer.Services["urn:upnp-org:serviceId:RenderingControl"]);
            _contentDirectory = new ContentDirectory(MediaServer.Services["urn:upnp-org:serviceId:ContentDirectory"]);
        }

        private void DumpServices()
        {
            foreach (UPnPService service in _device.Services)
            {
                Console.WriteLine(service.ServiceTypeIdentifier);
                Console.WriteLine(service.Id);
            }
        }

        public UPnPDevice MediaServer { get { return _mediaServer; } }
        public UPnPDevice MediaRenderer { get { return _mediaRenderer; } }
        public UPnPService AudioIn { get { return _audioIn; } }
        public SystemProperties SystemProperties { get { return _systemProperties; } }
        public DeviceProperties DeviceProperties { get { return _deviceProperties; } }
        public AVTransport AVTransport { get { return _avTransport; } }
        public RenderingControl RenderingControl { get { return _renderingControl; } }
        public ContentDirectory ContentDirectory { get { return _contentDirectory; } }
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
