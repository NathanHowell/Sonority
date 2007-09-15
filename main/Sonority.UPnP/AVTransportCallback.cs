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
using System.IO;
using System.Xml;
using System.Xml.XPath;
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

            XPathDocument doc = new XPathDocument(new StringReader((string)vaValue));
            XPathNavigator nav = doc.CreateNavigator();

            foreach (XPathNavigator node in nav.Select(eventExpression))
            {
                XPathNavigator val = node.SelectSingleNode(valExpression);
                avTransport.OnStateVariableChanged(node.LocalName, val.Value);
            }
        }

        private static readonly XPathExpression eventExpression = XPathExpression.Compile("/avt:Event/avt:InstanceID[@val='0']/*", Namespaces.Manager);
        private static readonly XPathExpression valExpression = XPathExpression.Compile("@val", Namespaces.Manager);
        private WeakReference avTransportRef;
    }
}
