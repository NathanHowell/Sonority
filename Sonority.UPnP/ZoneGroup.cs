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
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Threading;
using UPNPLib;
using System.ComponentModel;

namespace Sonority.UPnP
{
    public sealed class ZoneGroup : INotifyPropertyChanged
    {
        public ZoneGroup(Discover disc, XPathNavigator node)
        {
            _coordinator = node.SelectSingleNode("@Coordinator").Value;
            _id = node.SelectSingleNode("@ID").Value;

            foreach (XPathNavigator nav in node.Select("ZoneGroupMember"))
            {
                ZonePlayer zp = disc.Intern(String.Concat("uuid:", nav.SelectSingleNode("@UUID").Value));
                _members.Add(zp);
            }

            _coordinatorZone = disc.Intern(String.Concat("uuid:", _coordinator));
            _members.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_members_CollectionChanged);
        }

        void _members_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("Members"));
        }

        public ObservableCollection<ZonePlayer> Members
        {
            get
            {
                return _members;
            }
        }

        public ZonePlayer Coordinator
        {
            get
            {
                return _coordinatorZone;
            }
        }

        public string CoordinatorUuid
        {
            get
            {
                return _coordinator;
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        private ObservableCollection<ZonePlayer> _members = new ObservableCollection<ZonePlayer>();
        private ZonePlayer _coordinatorZone;
        private string _coordinator;
        private string _id;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
