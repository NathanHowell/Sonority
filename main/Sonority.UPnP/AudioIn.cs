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
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Windows.Threading;
using UPNPLib;

namespace Sonority.UPnP
{
    public sealed class AudioInputAttributes
    {
        public string CurrentName;
        public string CurrentIcon;
    }

    public sealed class LineInLevel
    {
        public int CurrentLeftLineInLevel;
        public int CurrentRightLineInLevel;
    }

    public sealed partial class AudioIn : UPnPServiceBase
    {
        public AudioIn(UPnPService service) : base(service)
        {
        }

        public string StartTransmissionToGroup(string coordinatorID)
        {
            return UPnP.InvokeAction<string>(_service, "StartTransmissionToGroup", coordinatorID);
        }

        public void StopTransmissionToGroup(string coordinatorID)
        {
            UPnP.InvokeAction(_service, "StopTransmissionToGroup", coordinatorID);
        }

        public void SetAudioInputAttributes(string desiredName, string desiredIcon)
        {
            UPnP.InvokeAction(_service, "SetAudioInputAttributes", desiredName, desiredIcon);
        }

        public AudioInputAttributes GetAudioInputAttributes()
        {
            object[] ret = UPnP.InvokeAction(_service, "GetAudioInputAttributes");
            AudioInputAttributes aia = new AudioInputAttributes();
            aia.CurrentName = Convert.ToString(ret[0], CultureInfo.InvariantCulture);
            aia.CurrentIcon = Convert.ToString(ret[1], CultureInfo.InvariantCulture);
            return aia;
        }

        public void SetLineInLevel(int desiredLeftLineInLevel, int desiredRightLineInLevel)
        {
            UPnP.InvokeAction(_service, "SetLineInLevel", desiredLeftLineInLevel, desiredRightLineInLevel);
        }

        public LineInLevel GetLineInLevel()
        {
            object[] ret = UPnP.InvokeAction(_service, "GetLineInLevel");
            LineInLevel lil = new LineInLevel();
            lil.CurrentLeftLineInLevel = Convert.ToInt32(ret[0], CultureInfo.InvariantCulture);
            lil.CurrentRightLineInLevel = Convert.ToInt32(ret[1], CultureInfo.InvariantCulture);
            return lil;
        }
    }
}
