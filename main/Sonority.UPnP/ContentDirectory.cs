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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Threading;
using System.Xml;
using System.Xml.XPath;
using UPNPLib;

namespace Sonority.UPnP
{
    public enum BrowseFlags
    {
        BrowseDirectChildren,
        BrowseMetadata,
    }

    public class QueueItem : IComparable<QueueItem>, IComparable
    {
        public QueueItem(XPathNavigator node)
        {
            _item_id = node.SelectSingleNode("@id", XPath.Globals.Manager).Value;
            _item_parentID = node.SelectSingleNode("@parentID", XPath.Globals.Manager).Value;
            _item_restricted = node.SelectSingleNode("@restricted", XPath.Globals.Manager).ValueAsBoolean;
            _res_protocolInfo = node.SelectSingleNode("didl:res/@protocolInfo", XPath.Globals.Manager).Value;
            _res_value = node.SelectSingleNode("didl:res", XPath.Globals.Manager).Value;
            _albumArtUri_value = node.SelectSingleNode("upnp:albumArtURI", XPath.Globals.Manager).Value;
            _title_value = node.SelectSingleNode("dc:title", XPath.Globals.Manager).Value;
            _class_value = node.SelectSingleNode("upnp:class", XPath.Globals.Manager).Value;
            _creator_value = node.SelectSingleNode("dc:creator", XPath.Globals.Manager).Value;
            _album_value = node.SelectSingleNode("upnp:album", XPath.Globals.Manager).Value;
        }

        int IComparable<QueueItem>.CompareTo(QueueItem other)
        {
            return String.Compare(_item_id, other._item_id);
        }

        int IComparable.CompareTo(object obj)
        {
            IComparable<QueueItem> that = this;
            return that.CompareTo((QueueItem)obj);
        }

        public override bool Equals(object obj)
        {
            IComparable that = this;
            return that.CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return _item_id.GetHashCode();
        }

        public string Res
        {
            get
            {
                return _res_value;
            }
        }

        public string Creator
        {
            get
            {
                return _creator_value;
            }
        }

        public string ItemID
        {
            get
            {
                return _item_id;
            }
        }

        public override string ToString()
        {
            return System.Web.HttpUtility.UrlDecode(Res);
        }

        private string _item_id;
        private string _item_parentID;
        private bool _item_restricted;
        private string _res_protocolInfo;
        private string _res_value;
        private string _albumArtUri_value;
        private string _title_value;
        private string _class_value;
        private string _creator_value;
        private string _album_value;
    }

    public partial class ContentDirectory : DispatcherObject, IUPnPServiceCallback, INotifyPropertyChanged
    {
        internal ContentDirectory(UPnPService service)
        {
            _service = service;
            StateVariables.Initialize(this, service);
            service.AddCallback(new ServiceCallback(this));
        }

        void IUPnPServiceCallback.ServiceInstanceDied(UPnPService pus)
        {
            // ignore for now
        }

        void IUPnPServiceCallback.StateVariableChanged(UPnPService pus, string stateVariable, object value)
        {
            StateVariables.Changed(this, pus, stateVariable, value);
            PropertyChanged(this, new PropertyChangedEventArgs(stateVariable));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string GetSearchCapabilities()
        {
            return UPnP.InvokeAction<String>(_service, "GetSearchCapabilities");
        }

        public string GetSortCapabilities()
        {
            return UPnP.InvokeAction<String>(_service, "GetSortCapabilities");
        }

        public string GetSystemUpdateID()
        {
            return UPnP.InvokeAction<String>(_service, "GetSystemUpdateID");
        }

        public string GetAlbumArtistDisplayOption()
        {
            return UPnP.InvokeAction<String>(_service, "GetAlbumArtistDisplayOption");
        }

        public string GetLastIndexChange()
        {
            return UPnP.InvokeAction<String>(_service, "GetLastIndexChange");
        }

        public IEnumerable<XPathNavigator> Browse(string objectId, BrowseFlags browseFlag, string filter, string sortCriteria)
        {
            object[] inArgs = { objectId, browseFlag.ToString(), filter, 0u, 256u * 4, sortCriteria };
            object[] outArray = { "", 0u, uint.MaxValue, 0u };

            for (uint i = 0; i < (uint)outArray[2]; i += (uint)outArray[1])
            {
                inArgs.SetValue(i, 3); // starting index
                inArgs.SetValue(Math.Min((uint)inArgs.GetValue(4), (uint)outArray[2] - (uint)outArray[1]), 4); // items to retrieve
                outArray = UPnP.InvokeAction(_service, "Browse", inArgs);

            	string resultXml = Convert.ToString(outArray.GetValue(0));
	            uint numberReturned = Convert.ToUInt32(outArray.GetValue(1));
	            uint totalMatches = Convert.ToUInt32(outArray.GetValue(2));
                uint updateId = Convert.ToUInt32(outArray.GetValue(3));

                XPathDocument doc = new XPathDocument(new StringReader(resultXml));
                XPathNavigator nav = doc.CreateNavigator();

                foreach (XPathNavigator node in nav.Select(XPath.Expressions.ContainerElements))
                {
                    yield return node;
                }

                foreach (XPathNavigator node in nav.Select(XPath.Expressions.ItemsElements))
                {
                    yield return node;
                }
            }
        }

        // returns (objectid, result)
        public object[] CreateObject(string containerId, string elements)
        {
            return UPnP.InvokeAction(_service, "CreateObject", containerId, elements);
        }

        public void UpdateObject(string objectId, string containerId, string newTagValue)
        {
            UPnP.InvokeAction(_service, "UpdateObject", objectId, containerId, newTagValue);
        }

        public void DestroyObject(string objectId)
        {
            UPnP.InvokeAction(_service, "DestroyObject", objectId);
        }

        public void RefreshRadioStationList()
        {
            UPnP.InvokeAction(_service, "RefreshRadioStationList");
        }

        public void RefreshShareList()
        {
            UPnP.InvokeAction(_service, "RefreshShareList");
        }

        public void RefreshShareIndex(string albumArtistDisplayOption)
        {
            UPnP.InvokeAction(_service, "RefreshShareIndex");
        }

        public void Search()
        {
            throw new NotImplementedException();
        }

        private UPnPService _service;
    }
}
