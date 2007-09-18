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
    public class SystemProperties
    {
        internal SystemProperties(UPnPService service)
        {
            _service = service;
        }

        // TODO: figure out more state variables
        public string R_AudioInEncodeType = String.Empty;

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

        private UPnPService _service;
    }
}
