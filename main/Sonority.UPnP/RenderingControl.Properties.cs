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

namespace Sonority.UPnP
{
    public sealed partial class RenderingControl
    {
        public uint InstanceID { get { return _InstanceID; } }
        public string LastChange { get { return _LastChange; } }
        public Dictionary<Channel, bool> Mute { get { return _Mute; } }
        public Dictionary<Channel, ushort> Volume { get { return _Volume; } }
        public short VolumeDB { get { return _VolumeDB; } }
        public short Bass { get { return _Bass; } }
        public short Treble { get { return _Treble; } }
        public Dictionary<Channel, bool> Loudness { get { return _Loudness; } }
        public bool SupportsOutputFixed { get { return _SupportsOutputFixed; } }
        public bool OutputFixed { get { return _OutputFixed; } }

        private const uint _InstanceID = 0;
        private string _LastChange = String.Empty;
        private Dictionary<Channel, bool> _Mute = new Dictionary<Channel, bool>();
        private Dictionary<Channel, ushort> _Volume = new Dictionary<Channel, ushort>();
        private short _VolumeDB = 0;
        private short _Bass = 0;
        private short _Treble = 0;
        private Dictionary<Channel, bool> _Loudness = new Dictionary<Channel, bool>();
        private bool _SupportsOutputFixed = false;
        private bool _OutputFixed = false;
    }
}
