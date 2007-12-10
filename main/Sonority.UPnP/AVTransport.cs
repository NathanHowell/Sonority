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
using System.Diagnostics.CodeAnalysis;
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
        public uint NrTracks { get { return _nrTracks; } }
        public string MediaDuration { get { return _mediaDuration; } }
        public Uri CurrentUri { get { return _currentUri; } }
        public string CurrentUriMetadata { get { return _currentUriMetadata; } }
        public Uri NextUri { get { return _nextUri; } }
        public string NextUriMetadata { get { return _nextUriMetadata; } }
        public string PlayMedium { get { return _playMedium; } }
        public string RecordMedium { get { return _recordMedium; } }
        public string WriteStatus { get { return _writeStatus; } }

        internal uint _nrTracks;
        internal string _mediaDuration;
        internal Uri _currentUri;
        internal string _currentUriMetadata;
        internal Uri _nextUri;
        internal string _nextUriMetadata;
        internal string _playMedium;
        internal string _recordMedium;
        internal string _writeStatus;
    }

    public sealed class TransportInfo
    {
        public TransportState CurrentTransportState { get { return _currentTransportState; } }
        public string CurrentTransportStatus { get { return _currentTransportStatus; } }
        public string CurrentSpeed { get { return _currentSpeed; } }

        internal TransportState _currentTransportState;
        internal string _currentTransportStatus;
        internal string _currentSpeed;
    }

    public sealed class PositionInfo
    {
        public string Track { get { return _track; } }
        public TimeSpan? TrackDuration { get { return _trackDuration; } }
        public string TrackMetaData { get { return _trackMetaData; } }
        public Uri TrackUri { get { return _trackUri; } }
        public TimeSpan? RelTime { get { return _relTime; } }
        public TimeSpan? AbsTime { get { return _absTime; } }
        public string RelCount { get { return _relCount; } }
        public string AbsCount { get { return _absCount; } }

        internal string _track;
        internal TimeSpan? _trackDuration;
        internal string _trackMetaData;
        internal Uri _trackUri;
        internal TimeSpan? _relTime;
        internal TimeSpan? _absTime;
        internal string _relCount;
        internal string _absCount;
    }

    public sealed class DeviceCapabilities
    {
        public string PlayMedia { get { return _playMedia; } }
        public string RecMedia { get { return _recMedia; } }
        public string ReqQualityModes { get { return _reqQualityModes; } }

        internal string _playMedia;
        internal string _recMedia;
        internal string _reqQualityModes;
    }

    public sealed class TransportSettings
    {
        public string PlayMode { get { return _playMode; } }
        public string RecQualityMode { get { return _recQualityMode; } }

        internal string _playMode;
        internal string _recQualityMode;
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

            if (pi.TrackDuration.HasValue)
            {
                x.StateVariableChanged(_service, "TrackDuration", pi.TrackDuration.Value);
            }
            if (pi.RelTime.HasValue)
            {
                x.StateVariableChanged(_service, "RelTime", (_timeSpanBase = pi.RelTime.Value));
            }
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
        public void SetAVTransportUri(Uri currentUri, string currentUriMetadata)
        {
            if (currentUri == null)
            {
                throw new ArgumentException("currentUri is required", "currentUri");
            }

            if (AVTransportUri != currentUri || String.CompareOrdinal(_AVTransportURIMetaData, currentUriMetadata) != 0)
            {
                UPnP.InvokeAction(_service, "SetAVTransportURI", InstanceId, currentUri.ToString(), currentUriMetadata);
                _TransportState = TransportState.TRANSITIONING;
            }
        }

        // optional
        public void SetNextAVTransportUri(Uri currentUri, string currentUriMetadata)
        {
            if (currentUri == null)
            {
                throw new ArgumentException("currentUri is required", "currentUri");
            }

            if (NextAVTransportUri != currentUri || String.CompareOrdinal(_NextAVTransportURIMetaData, currentUriMetadata) != 0)
            {
                UPnP.InvokeAction(_service, "SetNextAVTransportUri", InstanceId, currentUri.ToString(), currentUriMetadata);
            }
        }

        // required
        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public MediaInfo GetMediaInfo()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetMediaInfo", InstanceId);

            MediaInfo mi = new MediaInfo();
            mi._nrTracks = Convert.ToUInt32(outArray[0], CultureInfo.InvariantCulture);
            mi._mediaDuration = Convert.ToString(outArray[1], CultureInfo.InvariantCulture);
            mi._currentUri = CreateUri(Convert.ToString(outArray[2], CultureInfo.InvariantCulture));
            mi._currentUriMetadata = Convert.ToString(outArray[3], CultureInfo.InvariantCulture);
            mi._nextUri = CreateUri(Convert.ToString(outArray[4], CultureInfo.InvariantCulture));
            mi._nextUriMetadata = Convert.ToString(outArray[5], CultureInfo.InvariantCulture);
            mi._playMedium = Convert.ToString(outArray[6], CultureInfo.InvariantCulture);
            mi._recordMedium = Convert.ToString(outArray[7], CultureInfo.InvariantCulture);
            mi._writeStatus = Convert.ToString(outArray[8], CultureInfo.InvariantCulture);
            return mi;
        }

        // required
        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public TransportInfo GetTransportInfo()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetTransportInfo", InstanceId);

            TransportInfo ti = new TransportInfo();
            ti._currentTransportState = (TransportState)Enum.Parse(typeof(TransportState), Convert.ToString(outArray[0], CultureInfo.InvariantCulture), true);
            ti._currentTransportStatus = Convert.ToString(outArray[1], CultureInfo.InvariantCulture);
            ti._currentSpeed = Convert.ToString(outArray[2], CultureInfo.InvariantCulture);
            return ti;
        }

        // required
        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public PositionInfo GetPositionInfo()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetPositionInfo", InstanceId);

            PositionInfo pi = new PositionInfo();
            pi._track = Convert.ToString(outArray[0], CultureInfo.InvariantCulture);
            pi._trackDuration = CreateTimeSpan(Convert.ToString(outArray[1], CultureInfo.InvariantCulture));
            pi._trackMetaData = Convert.ToString(outArray[2], CultureInfo.InvariantCulture);
            pi._trackUri = CreateUri(Convert.ToString(outArray[3], CultureInfo.InvariantCulture));
            pi._relTime = CreateTimeSpan(Convert.ToString(outArray[4], CultureInfo.InvariantCulture));
            pi._absTime = CreateTimeSpan(Convert.ToString(outArray[5], CultureInfo.InvariantCulture));
            pi._relCount = Convert.ToString(outArray[6], CultureInfo.InvariantCulture);
            pi._absCount = Convert.ToString(outArray[7], CultureInfo.InvariantCulture);
            return pi;
        }

        private static TimeSpan? CreateTimeSpan(string s)
        {
            TimeSpan timeSpan;
            if (TimeSpan.TryParse(s, out timeSpan))
            {
                return timeSpan;
            }
            else
            {
                return null;
            }
        }

        // required
        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public DeviceCapabilities GetDeviceCapabilities()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetDeviceCapabilities", InstanceId);

            DeviceCapabilities dc = new DeviceCapabilities();
            dc._playMedia = Convert.ToString(outArray[0], CultureInfo.InvariantCulture);
            dc._recMedia = Convert.ToString(outArray[1], CultureInfo.InvariantCulture);
            dc._reqQualityModes = Convert.ToString(outArray[2], CultureInfo.InvariantCulture);
            return dc;
        }

        // required
        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public TransportSettings GetTransportSettings()
        {
            object[] outArray = UPnP.InvokeAction(_service, "GetTransportSettings", InstanceId);

            TransportSettings ts = new TransportSettings();
            ts._playMode = Convert.ToString(outArray[0], CultureInfo.InvariantCulture);
            ts._recQualityMode = Convert.ToString(outArray[1], CultureInfo.InvariantCulture);
            return ts;
        }

        // required
        public void Stop()
        {
            if (TransportState != TransportState.STOPPED)
            {
                UPnP.InvokeAction(_service, "Stop", InstanceId);
            }
        }

        // required
        public void Play()
        {
            const string speed = "1";
            if (this.NumberOfTracks > 0 && TransportState != TransportState.PLAYING)
            {
                UPnP.InvokeAction(_service, "Play", InstanceId, speed);
            }
        }

        // optional
        public void Pause()
        {
            if (TransportState != TransportState.PAUSED_PLAYBACK)
            {
                UPnP.InvokeAction(_service, "Pause", InstanceId);
            }
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
            UPnP.InvokeAction(_service, "Record", InstanceId);
        }

        // required
        public void Seek(SeekMode unit, string target)
        {
            UPnP.InvokeAction(_service, "Seek", InstanceId, unit.ToString(), target);
        }

        // required
        public void Next()
        {
            if (CurrentTrack < NumberOfTracks)
            {
                UPnP.InvokeAction(_service, "Next", InstanceId);
            }
        }

        // required
        public void Previous()
        {
            if (CurrentTrack > 1)
            {
               UPnP.InvokeAction(_service, "Previous", InstanceId);
            }
        }

        // optional
        public void SetPlayMode(PlayMode newPlayMode)
        {
            UPnP.InvokeAction(_service, "SetPlayMode", InstanceId, newPlayMode.ToString());
        }

        // optional
        public void SetRecordQualityMode()
        {
            throw new NotImplementedException();
        }

        // optional
        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public string GetCurrentTransportActions()
        {
            return UPnP.InvokeAction<String>(_service, "GetCurrentTransportActions", InstanceId);
        }

        public void AddUriToQueue(Uri enqueuedUri, string enqueuedUriMetaData, uint desiredFirstTrackNumberEnqueued, bool enqueueAsNext)
        {
            if (enqueuedUri == null)
            {
                throw new ArgumentException("enqueuedUri required", "enqueuedUri");
            }

            UPnP.InvokeAction(_service, "AddURIToQueue", InstanceId, enqueuedUri.ToString(), enqueuedUriMetaData, desiredFirstTrackNumberEnqueued, enqueueAsNext);
            // out[0] == FirstTrackNumberEnqueued
            // out[1] == NumTracksAdded
            // out[2] == NewQueueLength
        }

        public void ReorderTracksInQueue(uint startingIndex, uint numberOfTracks, uint insertBefore)
        {
            UPnP.AsyncInvokeAction(_service, "ReorderTracksInQueue", InstanceId, startingIndex, numberOfTracks, insertBefore);
        }

        public void RemoveAllTracksFromQueue()
        {
            UPnP.AsyncInvokeAction(_service, "RemoveAllTracksFromQueue", InstanceId);
        }

        public void RemoveTrackFromQueue(string objectId)
        {
            UPnP.InvokeAction(_service, "RemoveTrackFromQueue", InstanceId, objectId);
        }

        private static Uri CreateUri(string uri)
        {
            if (String.IsNullOrEmpty(uri))
            {
                return null;
            }

            return new Uri(uri);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
