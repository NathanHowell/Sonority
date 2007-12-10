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

namespace Sonority.UPnP
{
    public sealed partial class ContentDirectory
    {
        public uint SystemUpdateId { get { return _SystemUpdateID; } }
        public string ContainerUpdateIDs { get { return _ContainerUpdateIDs; } }
        public string ShareListRefreshState { get { return _ShareListRefreshState; } }
        public string ShareIndexInProgress { get { return _ShareIndexInProgress; } }
        public string ShareIndexLastError { get { return _ShareIndexLastError; } }
        public string UserRadioUpdateId { get { return _UserRadioUpdateID; } }
        public string MasterRadioUpdateId { get { return _MasterRadioUpdateID; } }
        public string SavedQueuesUpdateId { get { return _SavedQueuesUpdateID; } }
        public string ShareListUpdateId { get { return _ShareListUpdateID; } }

#pragma warning disable 0649 // Field 'x' is never assigned to, and will always have its default value
        private uint _SystemUpdateID;
        private string _ContainerUpdateIDs = String.Empty;
        private string _ShareListRefreshState = String.Empty;
        private string _ShareIndexInProgress = String.Empty;
        private string _ShareIndexLastError = String.Empty;
        private string _UserRadioUpdateID = String.Empty;
        private string _MasterRadioUpdateID = String.Empty;
        private string _SavedQueuesUpdateID = String.Empty;
        private string _ShareListUpdateID = String.Empty;
#pragma warning restore 0649
    }
}