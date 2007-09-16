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
    public partial class AVTransport
    {
        public uint InstanceID { get { return _InstanceID; } }
        public string TransportState { get { return _TransportState; } }
        public string TransportStatus { get { return _TransportStatus; } }
        public string PlaybackStorageMedium { get { return _PlaybackStorageMedium; } }
        public string RecordStorageMedium { get { return _RecordStorageMedium; }}
        public string PossiblePlaybackStorageMedia { get { return _PossiblePlaybackStorageMedia; } }
        public string PossibleRecordStorageMedia { get { return _PossibleRecordStorageMedia; } }
        public string CurrentPlayMode { get { return _CurrentPlayMode; } }
        public string TransportPlaySpeed { get { return _TransportPlaySpeed; } }
        public string RecordMediumWriteStatus { get { return _RecordMediumWriteStatus; } }
        public string CurrentRecordQualityMode { get { return _CurrentRecordQualityMode; } }
        public string PossibleRecordQualityModes { get { return _PossibleRecordQualityModes; } }
        public uint NumberOfTracks { get { return _NumberOfTracks; } }
        public uint CurrentTrack { get { return _CurrentTrack; } }
        public string CurrentTrackDuration { get { return _CurrentTrackDuration; } }
        public string CurrentMediaDuration { get { return _CurrentMediaDuration; } }
        public string CurrentTrackMetaData { get { return _CurrentTrackMetaData; } }
        public string CurrentSection { get { return _CurrentSection; } }
        public string CurrentTrackURI { get { return _CurrentTrackURI; } }
        public string NextTrackURI { get { return _NextTrackURI; } }
        public string NextTrackMetaData { get { return _NextTrackMetaData; } }
        public string EnqueuedTransportURI { get { return _EnqueuedTransportURI; } }
        public string EnqueuedTransportURIMetaData { get { return _EnqueuedTransportURIMetaData; } }
        public string AVTransportURI { get { return _AVTransportURI; } }
        public string AVTransportURIMetaData { get { return _AVTransportURIMetaData; } }
        public string CurrentTransportActions { get { return _CurrentTransportActions; } }
        public string SleepTimerGeneration { get { return _SleepTimerGeneration; } }
        public string AlarmRunning { get { return _AlarmRunning; } }
        public string SnoozeRunning { get { return _SnoozeRunning; } }
        public string RestartPending { get { return _RestartPending; } }
        public string NextAVTransportURI { get { return _NextAVTransportURI; } }
        public string NextAVTransportURIMetaData { get { return _NextAVTransportURIMetaData; } }

        internal const uint _InstanceID = 0;
        internal string _TransportState = String.Empty;
        internal string _TransportStatus = String.Empty;
        internal string _PlaybackStorageMedium = String.Empty;
        internal string _RecordStorageMedium = String.Empty;
        internal string _PossiblePlaybackStorageMedia = String.Empty;
        internal string _PossibleRecordStorageMedia = String.Empty;
        internal string _CurrentPlayMode = String.Empty;
        internal string _TransportPlaySpeed = String.Empty;
        internal string _RecordMediumWriteStatus = String.Empty;
        internal string _CurrentRecordQualityMode = String.Empty;
        internal string _PossibleRecordQualityModes = String.Empty;
        internal uint _NumberOfTracks = 0u;
        internal uint _CurrentTrack = 0u;
        internal string _CurrentTrackDuration = String.Empty;
        internal string _CurrentMediaDuration = String.Empty;
        internal string _CurrentTrackMetaData = String.Empty;
        internal string _CurrentSection = String.Empty;
        internal string _CurrentTrackURI = String.Empty;
        internal string _NextTrackURI = String.Empty;
        internal string _NextTrackMetaData = String.Empty;
        internal string _EnqueuedTransportURI = String.Empty;
        internal string _EnqueuedTransportURIMetaData = String.Empty;
        internal string _AVTransportURI = String.Empty;
        internal string _AVTransportURIMetaData = String.Empty;
        internal string _CurrentTransportActions = String.Empty;
        internal string _SleepTimerGeneration = String.Empty;
        internal string _AlarmRunning = String.Empty;
        internal string _SnoozeRunning = String.Empty;
        internal string _RestartPending = String.Empty;
        internal string _NextAVTransportURI = String.Empty;
        internal string _NextAVTransportURIMetaData = String.Empty;
    }
}