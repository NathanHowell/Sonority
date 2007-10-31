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
using System.Xml;
using System.Xml.XPath;

namespace Sonority.UPnP
{
    public class ZoneGroupMember
    {
        public ZoneGroupMember(XPathNavigator node)
        {
            _uuid = node.SelectSingleNode("@UUID").Value;
            _location = node.SelectSingleNode("@Location").Value;
            _zoneName = node.SelectSingleNode("@ZoneName").Value;
            _icon = node.SelectSingleNode("@Icon").Value;
            _softwareVersion = node.SelectSingleNode("@SoftwareVersion").Value;
            _bootSeq = node.SelectSingleNode("@BootSeq").Value;
        }

        public ZonePlayer GetZonePlayer()
        {
            return new ZonePlayer(String.Format("uuid:{0}", _uuid));
        }

        private string _uuid;
        private string _location;
        private string _zoneName;
        private string _icon;
        private string _softwareVersion;
        private string _bootSeq;
    }
}
