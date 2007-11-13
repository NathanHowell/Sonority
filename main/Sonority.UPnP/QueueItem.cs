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
using System.Xml;
using System.Xml.XPath;
using System.Web;
using UPNPLib;

namespace Sonority.UPnP
{
    public sealed class QueueItem : IComparable<QueueItem>, IComparable
    {
        public QueueItem(XPathNavigator node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

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

        public string Artist
        {
            get
            {
                return _creator_value;
            }
        }

        public string Album
        {
            get
            {
                return _album_value;
            }
        }

        public string Title
        {
            get
            {
                return _title_value;
            }
        }

        public string ItemID
        {
            get
            {
                return _item_id;
            }
        }

        public int NumericID
        {
            get
            {
                return Convert.ToInt32(_item_id.Substring(_item_parentID.Length + 1));
            }
        }

        public Uri AlbumArt
        {
            get
            {
                if (_albumArtUri_value != null)
                {
                    return new Uri(String.Concat("http://192.168.1.107:1400", HttpUtility.UrlDecode(_albumArtUri_value)));
                }
                else
                {
                    return new Uri("http://mypromusa.com/y/images/red_x.gif");
                }
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
}
