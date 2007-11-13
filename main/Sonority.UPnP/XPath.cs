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

namespace Sonority.XPath
{
    internal sealed class SonorityNamespaceManager : XmlNamespaceManager
    {
        public SonorityNamespaceManager(XmlNameTable nameTable) : base(nameTable)
        {
            AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
            AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
            AddNamespace("r", "urn:schemas-rinconnetworks-com:metadata-1-0/");
            AddNamespace("avt", "urn:schemas-upnp-org:metadata-1-0/AVT/");
            AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            AddNamespace("rcs", "urn:schemas-upnp-org:metadata-1-0/RCS/");
        }
    }

    public static class Globals
    {
        private static XmlNameTable Table = new NameTable();
        public static XmlNamespaceManager Manager = new SonorityNamespaceManager(Table);
    }

    public static class Expressions
    {
        public static readonly XPathExpression EventElements = XPathExpression.Compile("/avt:Event/avt:InstanceID[@val='0']/*", XPath.Globals.Manager);
        public static readonly XPathExpression ValueAttributes = XPathExpression.Compile("@val", XPath.Globals.Manager);
        public static readonly XPathExpression ChannelAttributes = XPathExpression.Compile("@channel", XPath.Globals.Manager);

        public static readonly XPathExpression VolumeElements = XPathExpression.Compile("/rcs:Event/rcs:InstanceID[@val='0']/rcs:Volume", XPath.Globals.Manager);
        public static readonly XPathExpression MuteElements = XPathExpression.Compile("/rcs:Event/rcs:InstanceID[@val='0']/rcs:Mute", XPath.Globals.Manager);

        public static readonly XPathExpression ItemsElements = XPathExpression.Compile("/didl:DIDL-Lite/didl:item", XPath.Globals.Manager);
        public static readonly XPathExpression ContainerElements = XPathExpression.Compile("/didl:DIDL-Lite/didl:container", XPath.Globals.Manager);
    }
}
