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
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using System.Web;
using UPNPLib;

namespace Sonority.UPnP
{
    public sealed class QueueItem : IComparable<QueueItem>, IComparable
    {
        public QueueItem(XPathNavigator nav)
        {
            if (nav == null)
            {
                throw new ArgumentNullException("nav");
            }

            _item_id = SelectSingleNode<string>(nav, "@id");
            _item_parentID = SelectSingleNode<string>(nav, "@parentID");
            _item_restricted = SelectSingleNode<bool>(nav, "@restricted");
            _res_protocolInfo = SelectSingleNode<string>(nav, "didl:res/@protocolInfo");
            _res_value = SelectSingleNode<Uri>(nav, "didl:res");
            _albumArtUri_value = SelectSingleNode<string>(nav, "upnp:albumArtURI");
            _title_value = SelectSingleNode<string>(nav, "dc:title");
            _class_value = SelectSingleNode<string>(nav, "upnp:class");
            _creator_value = SelectSingleNode<string>(nav, "dc:creator");
            _album_value = SelectSingleNode<string>(nav, "upnp:album");
        }

        private static T SelectSingleNode<T>(XPathNavigator nav, string xpath)
        {
            XPathNavigator node = nav.SelectSingleNode(xpath, XPath.Globals.Manager);
            if (node == null || String.IsNullOrEmpty(node.Value))
            {
                return default(T);
            }
            else
            {
                return (T)node.ValueAs(typeof(T));
            }
        }

        public static bool operator ==(QueueItem lhs, QueueItem rhs)
        {
            IComparable<QueueItem> qi = lhs;
            return qi.CompareTo(rhs) == 0;
        }

        public static bool operator !=(QueueItem lhs, QueueItem rhs)
        {
            IComparable<QueueItem> qi = lhs;
            return qi.CompareTo(rhs) != 0;
        }

        public static bool operator <(QueueItem lhs, QueueItem rhs)
        {
            IComparable<QueueItem> qi = lhs;
            return qi.CompareTo(rhs) < 0;
        }
        
        public static bool operator >(QueueItem lhs, QueueItem rhs)
        {
            IComparable<QueueItem> qi = lhs;
            return qi.CompareTo(rhs) > 0;
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

        public Uri Res
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

        public string ItemId
        {
            get
            {
                return _item_id;
            }
        }

        public int NumericId
        {
            get
            {
                return Convert.ToInt32(_item_id.Substring(_item_parentID.Length + 1), CultureInfo.InvariantCulture);
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
            return Res.ToString();
        }

        private string _item_id = String.Empty;
        private string _item_parentID = String.Empty;
        private bool _item_restricted;
        private string _res_protocolInfo = String.Empty;
        private Uri _res_value;
        private string _albumArtUri_value = String.Empty;
        private string _title_value = String.Empty;
        private string _class_value = String.Empty;
        private string _creator_value = String.Empty;
        private string _album_value = String.Empty;
    }
}
