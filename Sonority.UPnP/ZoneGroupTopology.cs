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
    public enum UnresponsiveDeviceActionType
    {
        Remove,
        VerifyThenRemoveSystemwide,
    }

    public sealed partial class ZoneGroupTopology : UPnPServiceBase
    {
        internal ZoneGroupTopology(UPnPService service) : base(service)
        {
        }

        public string CheckForUpdate(string updateType, bool cachedOnly, string version)
        {
            return UPnP.InvokeAction<string>(_service, "CheckForUpdate", updateType, cachedOnly, version);
        }

        public void BeginSoftwareUpdate(Uri updateUrl, uint flags)
        {
            if (updateUrl == null || updateUrl.IsAbsoluteUri == false)
            {
                throw new ArgumentException("Need absolute uri", "updateUrl");
            }

            UPnP.InvokeAction(_service, "BeginSoftwareUpdate", updateUrl.ToString(), flags);
        }

        // Remove || VerifyThenRemoveSystemwide
        public void ReportUnresponsiveDevice(string deviceUuid, UnresponsiveDeviceActionType desiredAction)
        {
            UPnP.InvokeAction(_service, "ReportUnresponsiveDevice", deviceUuid, desiredAction.ToString());
        }
    }
}
