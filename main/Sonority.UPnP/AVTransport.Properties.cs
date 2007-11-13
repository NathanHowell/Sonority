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
    public sealed partial class AVTransport
    {
        public uint InstanceID { get { return _InstanceID; } }
        public TransportState TransportState { get { return _TransportState; } }
        public string TransportStatus { get { return _TransportStatus; } }
        public string PlaybackStorageMedium { get { return _PlaybackStorageMedium; } }
        public string RecordStorageMedium { get { return _RecordStorageMedium; }}
        public string PossiblePlaybackStorageMedia { get { return _PossiblePlaybackStorageMedia; } }
        public string PossibleRecordStorageMedia { get { return _PossibleRecordStorageMedia; } }
        public PlayMode CurrentPlayMode { get { return (PlayMode)Enum.Parse(typeof(PlayMode), _CurrentPlayMode); } }
        public string TransportPlaySpeed { get { return _TransportPlaySpeed; } }
        public string RecordMediumWriteStatus { get { return _RecordMediumWriteStatus; } }
        public string CurrentRecordQualityMode { get { return _CurrentRecordQualityMode; } }
        public string PossibleRecordQualityModes { get { return _PossibleRecordQualityModes; } }
        public uint NumberOfTracks { get { return _NumberOfTracks; } }
        public uint CurrentTrack { get { return _CurrentTrack; } }
        public TimeSpan CurrentTrackDuration { get { return TimeSpan.Parse(_CurrentTrackDuration); } }
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
        public string LastChange { get { return _LastChange; } }

        // for progress bar display binding
        public TimeSpan RelTime { get { return _RelTime; } }
        public TimeSpan TrackDuration { get { return _TrackDuration; } }
        private TimeSpan _RelTime = TimeSpan.MinValue;
        private TimeSpan _TrackDuration = TimeSpan.MinValue;

        private const uint _InstanceID = 0;
        private TransportState _TransportState = TransportState.TRANSITIONING;
        private string _TransportStatus = String.Empty;
        private string _PlaybackStorageMedium = String.Empty;
        private string _RecordStorageMedium = String.Empty;
        private string _PossiblePlaybackStorageMedia = String.Empty;
        private string _PossibleRecordStorageMedia = String.Empty;
        private string _CurrentPlayMode = PlayMode.NORMAL.ToString();
        private string _TransportPlaySpeed = String.Empty;
        private string _RecordMediumWriteStatus = String.Empty;
        private string _CurrentRecordQualityMode = String.Empty;
        private string _PossibleRecordQualityModes = String.Empty;
        private uint _NumberOfTracks = 0u;
        private uint _CurrentTrack = 0u;
        private string _CurrentTrackDuration = String.Empty;
        private string _CurrentMediaDuration = String.Empty;
        private string _CurrentTrackMetaData = String.Empty;
        private string _CurrentSection = String.Empty;
        private string _CurrentTrackURI = String.Empty;
        private string _NextTrackURI = String.Empty;
        private string _NextTrackMetaData = String.Empty;
        private string _EnqueuedTransportURI = String.Empty;
        private string _EnqueuedTransportURIMetaData = String.Empty;
        private string _AVTransportURI = String.Empty;
        private string _AVTransportURIMetaData = String.Empty;
        private string _CurrentTransportActions = String.Empty;
        private string _SleepTimerGeneration = String.Empty;
        private string _AlarmRunning = String.Empty;
        private string _SnoozeRunning = String.Empty;
        private string _RestartPending = String.Empty;
        private string _NextAVTransportURI = String.Empty;
        private string _NextAVTransportURIMetaData = String.Empty;
        private string _LastChange = String.Empty;
    }
}