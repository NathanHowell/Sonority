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
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Windows.Threading;
using UPNPLib;

namespace Sonority.UPnP
{
    public class MediaInfo
    {
        public uint NrTracks;
        public string MediaDuration;
        public string CurrentUri;
        public string CurrentUriMetadata;
        public string NextUri;
        public string NextUriMetadata;
        public string PlayMedium;
        public string RecordMedium;
        public string WriteStatus;
    }

    public class TransportInfo
    {
        public TransportState CurrentTransportState;
        public string CurrentTransportStatus;
        public string CurrentSpeed;
    }

    public class PositionInfo
    {
        public string Track;
        public string TrackDuration;
        public string TrackMetaData;
        public string TrackUri;
        public string RelTime;
        public string AbsTime;
        public string RelCount;
        public string AbsCount;
    }

    public class DeviceCapabilities
    {
        public string PlayMedia;
        public string RecMedia;
        public string ReqQualityModes;
    }

    public class TransportSettings
    {
        public string PlayMode;
        public string RecQualityMode;
    }

    public enum SeekMode
    {
        TRACK_NR,
        REL_TIME,
        SECTION,
    }

    // http://x.x.x.x:1400/xml/AVTransport1.xml
    // PAUSED_PLAYBACK does not show up on the list but Sonos uses this state
    public enum TransportState
    {
        STOPPED,
        PLAYING,
        PAUSED_PLAYBACK,
        PAUSED_PLAYING,
        TRANSITIONING,
    }

    public partial class AVTransport : UPnPServiceBase
    {
        public AVTransport(UPnPService service) : base(service)
        {
            _service.AddCallback(new AVTransportCallback(this));
        }

        // required
        public void SetAVTransportUri(string currentUri, string currentUriMetadata)
        {
            UPnP.InvokeAction(_service, "SetAVTransportURI", InstanceID, currentUri, currentUriMetadata);
        }

        // optional
        public void SetNextAVTransportUri(string currentUri, string currentUriMetadata)
        {
            UPnP.InvokeAction(_service, "SetNextAVTransportUri", InstanceID, currentUri, currentUriMetadata);
        }

        // required
        public MediaInfo GetMediaInfo()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetMediaInfo", InstanceID);

            MediaInfo mi = new MediaInfo();
            mi.NrTracks = Convert.ToUInt32(outArray[0]);
            mi.MediaDuration = Convert.ToString(outArray[1]);
            mi.CurrentUri = Convert.ToString(outArray[2]);
            mi.CurrentUriMetadata = Convert.ToString(outArray[3]);
            mi.NextUri = Convert.ToString(outArray[4]);
            mi.NextUriMetadata = Convert.ToString(outArray[5]);
            mi.PlayMedium = Convert.ToString(outArray[6]);
            mi.RecordMedium = Convert.ToString(outArray[7]);
            mi.WriteStatus = Convert.ToString(outArray[8]);
            return mi;
        }

        // required
        public TransportInfo GetTransportInfo()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetTransportInfo", InstanceID);

            TransportInfo ti = new TransportInfo();
            ti.CurrentTransportState = (TransportState)Enum.Parse(typeof(TransportState), Convert.ToString(outArray[0]), true);
            ti.CurrentTransportStatus = Convert.ToString(outArray[1]);
            ti.CurrentSpeed = Convert.ToString(outArray[2]);
            return ti;
        }

        // required
        public PositionInfo GetPositionInfo()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetPositionInfo", InstanceID);

            PositionInfo pi = new PositionInfo();
            pi.Track = Convert.ToString(outArray[0]);
            pi.TrackDuration = Convert.ToString(outArray[1]);
            pi.TrackMetaData = Convert.ToString(outArray[2]);
            pi.TrackUri = Convert.ToString(outArray[3]);
            pi.RelTime = Convert.ToString(outArray[4]);
            pi.AbsTime = Convert.ToString(outArray[5]);
            pi.RelCount = Convert.ToString(outArray[6]);
            pi.AbsCount = Convert.ToString(outArray[7]);
            return pi;
        }

        // required
        public DeviceCapabilities GetDeviceCapabilities()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetDeviceCapabilities", InstanceID);

            DeviceCapabilities dc = new DeviceCapabilities();
            dc.PlayMedia = Convert.ToString(outArray[0]);
            dc.RecMedia = Convert.ToString(outArray[1]);
            dc.ReqQualityModes = Convert.ToString(outArray[2]);
            return dc;
        }

        // required
        public TransportSettings GetTransportSettings()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetTransportSettings", InstanceID);

            TransportSettings ts = new TransportSettings();
            ts.PlayMode = Convert.ToString(outArray[0]);
            ts.RecQualityMode = Convert.ToString(outArray[1]);
            return ts;
        }

        // required
        public void Stop()
        {
            UPnP.InvokeAction(_service, "Stop", InstanceID);
        }

        // required
        public void Play()
        {
            const string speed = "1";
            UPnP.InvokeAction(_service, "Play", InstanceID, speed);
        }

        // optional
        public void Pause()
        {
            UPnP.InvokeAction(_service, "Pause", InstanceID);
        }

        public void PlayPause()
        {
            if (TransportState != TransportState.PLAYING)
            {
                Play();
            }
            else
            {
                Pause();
            }
        }

        // optional
        public void Record()
        {
            UPnP.InvokeAction(_service, "Record", InstanceID);
        }

        // required
        public void Seek(SeekMode unit, string target)
        {
            UPnP.InvokeAction(_service, "Seek", InstanceID, unit.ToString(), target);
        }

        // required
        public void Next()
        {
            UPnP.InvokeAction(_service, "Next", InstanceID);
        }

        // required
        public void Previous()
        {
            UPnP.InvokeAction(_service, "Previous", InstanceID);
        }

        // optional
        public void SetPlayMode(string newPlayMode)
        {
            UPnP.InvokeAction(_service, "SetPlayMode", InstanceID);
        }

        // optional
        public void SetRecordQualityMode()
        {
            throw new NotImplementedException();
        }

        // optional
        public string GetCurrentTransportActions()
        {
            return UPnP.InvokeAction<String>(_service, "GetCurrentTransportActions", InstanceID);
        }

        // undocumented?
        public void RemoveTrackFromQueue(string objectID)
        {
            UPnP.InvokeAction(_service, "RemoveTrackFromQueue", InstanceID, objectID);
        }
    }
}
