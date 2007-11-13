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
    public sealed class MediaInfo
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

    public sealed class TransportInfo
    {
        public TransportState CurrentTransportState;
        public string CurrentTransportStatus;
        public string CurrentSpeed;
    }

    public sealed class PositionInfo
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

    public sealed class DeviceCapabilities
    {
        public string PlayMedia;
        public string RecMedia;
        public string ReqQualityModes;
    }

    public sealed class TransportSettings
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

    public enum PlayMode
    {
        NORMAL,
        REPEAT_ALL,
        SHUFFLE_NOREPEAT,
        SHUFFLE,
    }

    // http://x.x.x.x:1400/xml/AVTransport1.xml
    // PAUSED_PLAYBACK does not show up on the list but Sonos uses this state
    public enum TransportState
    {
        STOPPED,
        PLAYING,
        PAUSED_PLAYING,
        PAUSED_PLAYBACK = PAUSED_PLAYING,
        TRANSITIONING,
    }

    public sealed partial class AVTransport : UPnPServiceBase, IDisposable
    {
        public AVTransport(UPnPService service) : base(service)
        {
            _service.AddCallback(new AVTransportCallback(this));
            PropertyChanged += new PropertyChangedEventHandler(AVTransport_PropertyChanged);
            PropertyChanged += new PropertyChangedEventHandler(TransportStateChanged);
            _timer = new Timer(new TimerCallback(this.OnTimerFired), null, 1000, 1000);
        }

        void TransportStateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "TransportState")
            {
                return;
            }

            if (this.TransportState == TransportState.PLAYING)
            {
                _timer.Change(1000, 1000);
            }
            else
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        void AVTransport_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "LastChange")
            {
                return;
            }

            PositionInfo pi = GetPositionInfo();
            IUPnPServiceCallback x = this;
            x.StateVariableChanged(_service, "TrackDuration", TimeSpan.Parse(pi.TrackDuration));
            x.StateVariableChanged(_service, "RelTime", (_timeSpanBase = TimeSpan.Parse(pi.RelTime)));
            _baseTime = DateTime.Now;
        }

        private void OnTimerFired(object state)
        {
            if (this.TransportState == TransportState.PLAYING)
            {
                IUPnPServiceCallback x = this;
                x.StateVariableChanged(_service, "RelTime", _timeSpanBase.Add(DateTime.Now - _baseTime));
            }
        }

        private TimeSpan _timeSpanBase;
        private DateTime _baseTime;
        private Timer _timer;

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
            mi.NrTracks = Convert.ToUInt32(outArray[0], CultureInfo.InvariantCulture);
            mi.MediaDuration = Convert.ToString(outArray[1], CultureInfo.InvariantCulture);
            mi.CurrentUri = Convert.ToString(outArray[2], CultureInfo.InvariantCulture);
            mi.CurrentUriMetadata = Convert.ToString(outArray[3], CultureInfo.InvariantCulture);
            mi.NextUri = Convert.ToString(outArray[4], CultureInfo.InvariantCulture);
            mi.NextUriMetadata = Convert.ToString(outArray[5], CultureInfo.InvariantCulture);
            mi.PlayMedium = Convert.ToString(outArray[6], CultureInfo.InvariantCulture);
            mi.RecordMedium = Convert.ToString(outArray[7], CultureInfo.InvariantCulture);
            mi.WriteStatus = Convert.ToString(outArray[8], CultureInfo.InvariantCulture);
            return mi;
        }

        // required
        public TransportInfo GetTransportInfo()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetTransportInfo", InstanceID);

            TransportInfo ti = new TransportInfo();
            ti.CurrentTransportState = (TransportState)Enum.Parse(typeof(TransportState), Convert.ToString(outArray[0], CultureInfo.InvariantCulture), true);
            ti.CurrentTransportStatus = Convert.ToString(outArray[1], CultureInfo.InvariantCulture);
            ti.CurrentSpeed = Convert.ToString(outArray[2], CultureInfo.InvariantCulture);
            return ti;
        }

        // required
        public PositionInfo GetPositionInfo()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetPositionInfo", InstanceID);

            PositionInfo pi = new PositionInfo();
            pi.Track = Convert.ToString(outArray[0], CultureInfo.InvariantCulture);
            pi.TrackDuration = Convert.ToString(outArray[1], CultureInfo.InvariantCulture);
            pi.TrackMetaData = Convert.ToString(outArray[2], CultureInfo.InvariantCulture);
            pi.TrackUri = Convert.ToString(outArray[3], CultureInfo.InvariantCulture);
            pi.RelTime = Convert.ToString(outArray[4], CultureInfo.InvariantCulture);
            pi.AbsTime = Convert.ToString(outArray[5], CultureInfo.InvariantCulture);
            pi.RelCount = Convert.ToString(outArray[6], CultureInfo.InvariantCulture);
            pi.AbsCount = Convert.ToString(outArray[7], CultureInfo.InvariantCulture);
            return pi;
        }

        // required
        public DeviceCapabilities GetDeviceCapabilities()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetDeviceCapabilities", InstanceID);

            DeviceCapabilities dc = new DeviceCapabilities();
            dc.PlayMedia = Convert.ToString(outArray[0], CultureInfo.InvariantCulture);
            dc.RecMedia = Convert.ToString(outArray[1], CultureInfo.InvariantCulture);
            dc.ReqQualityModes = Convert.ToString(outArray[2], CultureInfo.InvariantCulture);
            return dc;
        }

        // required
        public TransportSettings GetTransportSettings()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetTransportSettings", InstanceID);

            TransportSettings ts = new TransportSettings();
            ts.PlayMode = Convert.ToString(outArray[0], CultureInfo.InvariantCulture);
            ts.RecQualityMode = Convert.ToString(outArray[1], CultureInfo.InvariantCulture);
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
            if (this.NumberOfTracks > 0)
            {
                UPnP.InvokeAction(_service, "Play", InstanceID, speed);
            }
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
            if (CurrentTrack < NumberOfTracks)
            {
                UPnP.InvokeAction(_service, "Next", InstanceID);
            }
        }

        // required
        public void Previous()
        {
            if (CurrentTrack > 1)
            {
               UPnP.InvokeAction(_service, "Previous", InstanceID);
            }
        }

        // optional
        public void SetPlayMode(PlayMode newPlayMode)
        {
            UPnP.InvokeAction(_service, "SetPlayMode", InstanceID, newPlayMode.ToString());
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

        public void AddURIToQueue(string enqueuedURI, string enqueuedURIMetaData, uint desiredFirstTrackNumberEnqueued, bool enqueueAsNext)
        {
            UPnP.InvokeAction(_service, "AddURIToQueue", InstanceID, enqueuedURI, enqueuedURIMetaData, desiredFirstTrackNumberEnqueued, enqueueAsNext);
            // out[0] == FirstTrackNumberEnqueued
            // out[1] == NumTracksAdded
            // out[2] == NewQueueLength
        }

        public void ReorderTracksInQueue(uint startingIndex, uint numberOfTracks, uint insertBefore)
        {
            UPnP.InvokeAction(_service, "ReorderTracksInQueue", InstanceID, startingIndex, numberOfTracks, insertBefore);
        }

        public void RemoveAllTracksFromQueue()
        {
            UPnP.InvokeAction(_service, "RemoveAllTracksFromQueue", InstanceID);
        }

        public void RemoveTrackFromQueue(string objectID)
        {
            UPnP.InvokeAction(_service, "RemoveTrackFromQueue", InstanceID, objectID);
        }

        void IDisposable.Dispose()
        {
            _timer.Dispose();
        }
    }
}
