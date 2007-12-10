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
    public sealed partial class GroupManagement : UPnPServiceBase
    {
        internal GroupManagement(UPnPService service) : base(service)
        {
        }

        public void AddMember(string memberId)
        {
            UPnP.InvokeAction(_service, "AddMember", memberId);
            // TODO: out[0] == CurrentTransportSettings
            // TODO: out[1] == GroupUUIDJoined
        }

        public void RemoveMember(string memberId)
        {
            UPnP.InvokeAction(_service, "RemoveMember", memberId);
        }

        public void ReportTrackBufferingResult(string memberId, int resultCode)
        {
            UPnP.InvokeAction(_service, "ReportTrackBufferingResult", memberId, resultCode);
        }
    }
}
