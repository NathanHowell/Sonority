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
using UPNPLib;

namespace Sonority.UPnP
{
    internal class AVTransportCallback : IUPnPServiceCallback
    {
        public AVTransportCallback(AVTransport outer)
        {
            avTransportRef = new WeakReference(outer);
        }

        public void ServiceInstanceDied(UPnPService pus)
        {
            AVTransport avTransport = (AVTransport)avTransportRef.Target;
            if (avTransport == null)
            {
                return;
            }
        }

        public void StateVariableChanged(UPnPService pus, string pcwszStateVarName, object vaValue)
        {
            AVTransport avTransport = (AVTransport)avTransportRef.Target;
            if (avTransport == null)
            {
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml((string)vaValue);

            XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
            nm.AddNamespace("avt", "urn:schemas-upnp-org:metadata-1-0/AVT/");
            nm.AddNamespace("r", "urn:schemas-rinconnetworks-com:metadata-1-0/");

            foreach (XmlNode node in doc.SelectNodes(String.Format("/avt:Event/avt:InstanceID[@val='{0}']/*", avTransport.InstanceID), nm))
            {
                avTransport.OnStateVariableChanged(node.LocalName, node.SelectSingleNode("@val").Value);
            }
        }

        private WeakReference avTransportRef;
    }
}
