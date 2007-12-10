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
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using System.Xml.XPath;
using UPNPLib;

namespace Sonority.UPnP
{
    public enum Channel
    {
        Master,
        LF,
        RF,
    }

    public enum RampType
    {
        SLEEP_TIMER_RAMP_TYPE,
        ALARM_RAMP_TYPE,
    }

    public sealed partial class RenderingControl : UPnPServiceBase
    {
        internal RenderingControl(UPnPService service) : base(service)
        {
            _Loudness[Channel.Master] = false;
            foreach (Channel c in Enum.GetValues(typeof(Channel)))
            {
                _Mute[c] = false;
                _Volume[c] = 0;
            }
            this.PropertyChanged += new PropertyChangedEventHandler(RenderingControl_PropertyChanged);
        }

        void RenderingControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "LastChange")
            {
                return;
            }

            /* <Event xmlns=\"urn:schemas-upnp-org:metadata-1-0/RCS/\">
             * <InstanceId val=\"0\">
             * <Volume channel=\"Master\" val=\"59\"/>
             * <Volume channel=\"LF\" val=\"100\"/>
             * <Volume channel=\"RF\" val=\"100\"/>
             * <Mute channel=\"Master\" val=\"0\"/>
             * <Mute channel=\"LF\" val=\"0\"/>
             * <Mute channel=\"RF\" val=\"0\"/>
             * <Bass val=\"0\"/>
             * <Treble val=\"0\"/>
             * <Loudness channel=\"Master\" val=\"0\"/>
             * <OutputFixed val=\"0\"/>
             * <PresetNameList>FactoryDefaults</PresetNameList>
             * </InstanceId>
             * </Event>
             */

            XPathDocument doc = new XPathDocument(new StringReader(LastChange));
            XPathNavigator nav = doc.CreateNavigator();

            foreach (XPathNavigator node in nav.Select(XPath.Expressions.RenderingChannelElements))
            {
                FieldInfo fi = GetField(node);
                IDictionary dict = (IDictionary)fi.GetValue(this);
                Type[] dictionaryTypes = fi.FieldType.GetGenericArguments();

                string channelString = node.SelectSingleNode(XPath.Expressions.ChannelAttributes).Value;
                Channel channel = (Channel)Enum.Parse(typeof(Channel), channelString, true);

                object value = node.SelectSingleNode(XPath.Expressions.ValueAttributes).ValueAs(dictionaryTypes[1]);

                if (Object.Equals(dict[channel], value) == false)
                {
                    dict[channel] = value;
                    RaisePropertyChangedEvent(new PropertyChangedEventArgs(node.LocalName));
                    RaisePropertyChangedEvent(new PropertyChangedEventArgs(String.Format(CultureInfo.InvariantCulture, "{0}[{1}]", node.LocalName, channel)));
                }
            }

            foreach (XPathNavigator node in nav.Select(XPath.Expressions.RenderingValueElements))
            {
                FieldInfo fi = GetField(node);
                object oldValue = fi.GetValue(this);
                object newValue = node.SelectSingleNode(XPath.Expressions.ValueAttributes).ValueAs(fi.FieldType);

                if (Object.Equals(oldValue, newValue) == false)
                {
                    fi.SetValue(this, newValue);
                    RaisePropertyChangedEvent(new PropertyChangedEventArgs(node.LocalName));
                }
            }

            foreach (XPathNavigator node in nav.Select(XPath.Expressions.RenderingNoAttributeElements))
            {
                FieldInfo fi = GetField(node);
                object oldValue = fi.GetValue(this);
                object newValue = node.ValueAs(fi.FieldType);

                if (Object.Equals(oldValue, newValue) == false)
                {
                    fi.SetValue(this, newValue);
                    RaisePropertyChangedEvent(new PropertyChangedEventArgs(node.LocalName));
                }
            }
        }

        private FieldInfo GetField(XPathNavigator node)
        {
            return this.GetType().GetField(String.Format(CultureInfo.InvariantCulture, "_{0}", node.LocalName), BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public bool GetMute(Channel channel)
        {
            return UPnP.InvokeAction<bool>(_service, "GetMute", InstanceId, channel.ToString());
        }

        public void SetMute(Channel channel, bool desiredMute)
        {
            UPnP.InvokeAction(_service, "SetMute", InstanceId, channel.ToString(), desiredMute);
        }

        public ushort GetVolume(Channel channel)
        {
            return UPnP.InvokeAction<ushort>(_service, "GetVolume", InstanceId, channel.ToString());
        }

        public void SetVolume(Channel channel, ushort desiredVolume)
        {
            UPnP.InvokeAction(_service, "SetVolume", InstanceId, channel.ToString(), desiredVolume);
        }

        public ushort SetRelativeVolume(Channel channel, int adjustment)
        {
            return UPnP.InvokeAction<ushort>(_service, "SetRelativeVolume", InstanceId, channel.ToString(), adjustment);
        }

        public short GetVolumeDB(Channel channel)
        {
            return UPnP.InvokeAction<short>(_service, "GetVolumeDB", InstanceId, channel.ToString());
        }

        public void SetVolumeDB(Channel channel, short desiredVolume)
        {
            UPnP.InvokeAction(_service, "SetVolumeDB", InstanceId, channel.ToString(), desiredVolume);
        }

        public void GetVolumeDBRange(Channel channel)
        {
            // TODO: return MinValue/MaxValue
            object[] results = UPnP.InvokeAction(_service, "GetVolumeDBRange", InstanceId, channel.ToString());
        }

        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public short GetBass()
        {
            return UPnP.InvokeAction<short>(_service, "GetBass", InstanceId);
        }

        public void SetBass(int desiredBass)
        {
            UPnP.InvokeAction(_service, "SetBass", InstanceId, desiredBass);
        }

        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public short GetTreble()
        {
            return UPnP.InvokeAction<short>(_service, "GetTreble", InstanceId);
        }

        public void SetTreble(int desiredTreble)
        {
            UPnP.InvokeAction(_service, "SetTreble", InstanceId, desiredTreble);
        }

        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public bool GetLoudness(Channel channel)
        {
            return UPnP.InvokeAction<bool>(_service, "GetLoudness", InstanceId, channel.ToString());
        }

        public void SetLoudness(Channel channel, bool desiredLoudness)
        {
            UPnP.InvokeAction(_service, "SetLoudness", InstanceId, channel.ToString(), desiredLoudness);
        }

        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public bool GetSupportsOutputFixed()
        {
            return UPnP.InvokeAction<bool>(_service, "GetSupportsOutputFixed", InstanceId);
        }

        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Modeling UPnP APIs, this is a method not a property")]
        public bool GetOutputFixed()
        {
            return UPnP.InvokeAction<bool>(_service, "GetOutputFixed", InstanceId);
        }

        public void SetOutputFixed(bool desiredFixed)
        {
            UPnP.InvokeAction(_service, "SetOutputFixed", InstanceId, desiredFixed);
        }

        public uint RampToVolume(Channel channel, RampType rampType, short desiredVolume)
        {
            return UPnP.InvokeAction<uint>(_service, "RampToVolume", InstanceId, channel.ToString(), rampType.ToString(), desiredVolume);
        }

        public void RestoreVolumePriorToRamp(Channel channel)
        {
            UPnP.InvokeAction(_service, "RestoreVolumePriorToRamp", InstanceId, channel.ToString());
        }
    }
}