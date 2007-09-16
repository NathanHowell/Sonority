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
using System.Text;
using System.Xml;
using System.Xml.XPath;
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
        public string CurrentTransportState;
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

    public class AVTransport : INotifyPropertyChanged
    {
        public AVTransport(UPnPService service)
        {
            avTransportService = service;
            avTransportService.AddCallback(new AVTransportCallback(this));
        }

        internal void OnStateVariableChanged(string stateVariable, object value)
        {
            System.Reflection.FieldInfo fi = this.GetType().GetField(stateVariable);
            if (fi == null)
            {
                Console.Error.WriteLine("Field not found: {0}", stateVariable);
                return;
            }

            fi.SetValue(this, Convert.ChangeType(value, fi.FieldType));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(stateVariable));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public readonly uint InstanceID = 0;
        public string TransportState = String.Empty;
        public string TransportStatus = String.Empty;
        public string PlaybackStorageMedium = String.Empty;
        public string RecordStorageMedium = String.Empty;
        public string PossiblePlaybackStorageMedia = String.Empty;
        public string PossibleRecordStorageMedia = String.Empty;
        public string CurrentPlayMode = String.Empty;
        public string TransportPlaySpeed = String.Empty;
        public string RecordMediumWriteStatus = String.Empty;
        public string CurrentRecordQualityMode = String.Empty;
        public string PossibleRecordQualityModes = String.Empty;
        public uint NumberOfTracks = 0u;
        public uint CurrentTrack = 0u;
        public string CurrentTrackDuration = String.Empty;
        public string CurrentMediaDuration = String.Empty;
        public string CurrentTrackMetaData = String.Empty;
        public string CurrentSection = String.Empty;
        public string CurrentTrackURI = String.Empty;
        public string NextTrackURI = String.Empty;
        public string NextTrackMetaData = String.Empty;
        public string EnqueuedTransportURI = String.Empty;
        public string EnqueuedTransportURIMetaData = String.Empty;
        public string AVTransportURI = String.Empty;
        public string AVTransportURIMetaData = String.Empty;
        public string CurrentTransportActions = String.Empty;
        public string SleepTimerGeneration = String.Empty;
        public string AlarmRunning = String.Empty;
        public string SnoozeRunning = String.Empty;
        public string RestartPending = String.Empty;
        public string NextAVTransportURI = String.Empty;
        public string NextAVTransportURIMetaData = String.Empty;

        private string QueryStateString(string variableName)
        {
            return Convert.ToString(avTransportService.QueryStateVariable(variableName));
        }

        private uint QueryStateUInt32(string variableName)
        {
            return Convert.ToUInt32(avTransportService.QueryStateVariable(variableName));
        }

        // required
        public void SetAVTransportUri(string currentUri, string currentUriMetadata)
        {
            Array inArgs = new object[] { InstanceID, currentUri, currentUriMetadata };
            object outArgs = null;
            object results = avTransportService.InvokeAction("SetAVTransportURI", inArgs, ref outArgs);
        }

        // optional
        public void SetNextAVTransportUri(string currentUri, string currentUriMetadata)
        {
            Array inArgs = new object[] { InstanceID, currentUri, currentUriMetadata };
            object outArgs = null;
            object results = avTransportService.InvokeAction("SetNextAVTransportURI", inArgs, ref outArgs);
        }

        // required
        public MediaInfo GetMediaInfo()
        {
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("GetMediaInfo", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;

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
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("GetTransportInfo", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;

            TransportInfo ti = new TransportInfo();
            ti.CurrentTransportState = Convert.ToString(outArray[0]);
            ti.CurrentTransportStatus = Convert.ToString(outArray[1]);
            ti.CurrentSpeed = Convert.ToString(outArray[2]);
            return ti;
        }

        // required
        public PositionInfo GetPositionInfo()
        {
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("GetPositionInfo", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;

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
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("GetDeviceCapabilities", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;

            DeviceCapabilities dc = new DeviceCapabilities();
            dc.PlayMedia = Convert.ToString(outArray[0]);
            dc.RecMedia = Convert.ToString(outArray[1]);
            dc.ReqQualityModes = Convert.ToString(outArray[2]);
            return dc;
        }

        // required
        public TransportSettings GetTransportSettings()
        {
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("GetTransportSettings", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;

            TransportSettings ts = new TransportSettings();
            ts.PlayMode = Convert.ToString(outArray[0]);
            ts.RecQualityMode = Convert.ToString(outArray[1]);
            return ts;
        }

        // required
        public void Stop()
        {
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("Stop", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;
        }

        // required
        public void Play(string speed)
        {
            Array inArgs = new object[] { InstanceID, speed };
            object outArgs = null;
            object results = avTransportService.InvokeAction("Play", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;
        }

        // optional
        public void Pause()
        {
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("Pause", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;
        }

        // optional
        public void Record()
        {
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("Record", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;
        }

        // required
        public void Seek(string unit, string target)
        {
            Array inArgs = new object[] { InstanceID, unit, target };
            object outArgs = null;
            object results = avTransportService.InvokeAction("Seek", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;
        }

        // required
        public void Next()
        {
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("Next", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;
        }

        // required
        public void Previous()
        {
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("Previous", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;
        }

        // optional
        public void SetPlayMode(string newPlayMode)
        {
            Array inArgs = new object[] { InstanceID, newPlayMode };
            object outArgs = null;
            object results = avTransportService.InvokeAction("SetPlayMode", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;
        }

        // optional
        public void SetRecordQualityMode()
        {
            throw new NotImplementedException();
        }

        // optional
        public string GetCurrentTransportActions()
        {
            Array inArgs = new object[] { InstanceID };
            object outArgs = null;
            object results = avTransportService.InvokeAction("GetCurrentTransportActions", inArgs, ref outArgs);
            object[] outArray = (object[])outArgs;
            return Convert.ToString(outArray[0]);
        }

        private UPnPService avTransportService;
    }
}
