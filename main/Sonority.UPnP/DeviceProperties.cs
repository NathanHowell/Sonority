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
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Threading;
using UPNPLib;

namespace Sonority.UPnP
{
    public class DeviceProperties : DispatcherObject, IUPnPServiceCallback, INotifyPropertyChanged
    {
        internal DeviceProperties(UPnPService service)
        {
            _service = service;
            StateVariables.Initialize(this, service);
            service.AddCallback(new ServiceCallback(this));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        void IUPnPServiceCallback.ServiceInstanceDied(UPnPService pus)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IUPnPServiceCallback.StateVariableChanged(UPnPService pus, string stateVariable, object value)
        {
            StateVariables.Changed(this, pus, stateVariable, value);
            PropertyChanged(this, new PropertyChangedEventArgs(stateVariable));
        }

        public string ZoneName { get { return _ZoneName; } }
        public string Icon { get { return _Icon; } }
        public bool Invisible { get { return _Invisible; } }
        public string SettingsReplicationState { get { return _SettingsReplicationState; } }

        internal string _ZoneName = String.Empty;
        internal string _Icon = String.Empty;
        internal bool _Invisible = false;
        internal string _SettingsReplicationState = String.Empty;

        private UPnPService _service;
    }
}