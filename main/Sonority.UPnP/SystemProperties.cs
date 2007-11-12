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
using System.Reflection;
using System.Text;
using UPNPLib;

namespace Sonority.UPnP
{
    public partial class SystemProperties : UPnPServiceBase, UPnPService
    {
        internal SystemProperties(UPnPService service) : base(service)
        {
            StateVariables.Initialize(this, this);
        }

        void SetString(string name, string value)
        {
            UPnP.InvokeAction(_service, "SetString", name, value);
        }

        string GetString(string name)
        {
            return UPnP.InvokeAction<String>(_service, "GetString", name);
        }

        // methods we don't care about:
        // ProvisionTrialAccount
        // ProvisionCredentialedTrialAccount
        // MigrateTrialAccount
        // AddAccount
        // RemoveAccount
        // EditAccountPassword
        // CreateStubAccounts

        object IUPnPService.QueryStateVariable(string bstrVariableName)
        {
            return this.GetString(bstrVariableName);
        }

        #region IUPnPService forwarders
        void IUPnPService.AddCallback(object pUnkCallback)
        {
            _service.AddCallback(pUnkCallback);
        }

        string IUPnPService.Id
        {
            get { return _service.Id; }
        }

        object IUPnPService.InvokeAction(string bstrActionName, object vInActionArgs, ref object pvOutActionArgs)
        {
            return _service.InvokeAction(bstrActionName, vInActionArgs, ref pvOutActionArgs);
        }

        int IUPnPService.LastTransportStatus
        {
            get { return _service.LastTransportStatus; }
        }

        string IUPnPService.ServiceTypeIdentifier
        {
            get { return _service.ServiceTypeIdentifier; }
        }
        #endregion
    }
}
