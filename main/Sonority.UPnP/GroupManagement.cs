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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using UPNPLib;

namespace Sonority.UPnP
{
    public class GroupManagement : DispatcherObject, IUPnPServiceCallback, INotifyPropertyChanged
    {
        internal GroupManagement(UPnPService service)
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

        public void AddMember(string memberID)
        {
            UPnP.InvokeAction(_service, "AddMember", memberID);
            // TODO: out[0] == CurrentTransportSettings
            // TODO: out[1] == GroupUUIDJoined
        }

        public void RemoveMember(string memberID)
        {
            UPnP.InvokeAction(_service, "RemoveMember", memberID);
        }

        public void ReportTrackBufferingResult(string memberID, int resultCode)
        {
            UPnP.InvokeAction(_service, "ReportTrackBufferingResult", memberID, resultCode);
        }

        public bool GroupCoordinatorIsLocal
        {
            get
            {
                return _GroupCoordinatorIsLocal;
            }
        }

        public string LocalGroupUUID
        {
            get
            {
                return _LocalGroupUUID;
            }
        }

        bool _GroupCoordinatorIsLocal = false;
        string _LocalGroupUUID = String.Empty;

        private UPnPService _service;
    }
}
