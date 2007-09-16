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
        }

        public ZonePlayer(UPnPDevice device)
        {
            _device = device;
        }

        private UPnPDevice _device;

        public UPnPDevice MediaServer
        {
            get
            {
                return _device.Children[String.Format("{0}_MS", _device.UniqueDeviceName)];
            }
        }

        public UPnPDevice MediaRenderer
        {
            get
            {
                return _device.Children[String.Format("{0}_MR", _device.UniqueDeviceName)];
            }
        }

        public UPnPService AudioIn
        {
            get
            {
                return _device.Services["urn:upnp-org:serviceId:AudioIn"];
            }
        }

        public UPnPService DeviceProperties
        {
            get
            {
                return _device.Services["urn:upnp-org:serviceId:DeviceProperties"];
            }
        }

        public AVTransport AVTransport
        {
            get
            {
                return new AVTransport(MediaRenderer.Services["urn:upnp-org:serviceId:AVTransport"]);
            }
        }

        public UPnPService RenderingControl
        {
            get
            {
                return MediaRenderer.Services["urn:upnp-org:serviceId:RenderingControl"];
            }
        }

        public ContentDirectory ContentDirectory
        {
            get
            {
                return new ContentDirectory(MediaServer.Services["urn:upnp-org:serviceId:ContentDirectory"]);
            }
        }

        public string UniqueDeviceName
        {
            get
            {
                return _device.UniqueDeviceName;
            }
        }
    }
}
