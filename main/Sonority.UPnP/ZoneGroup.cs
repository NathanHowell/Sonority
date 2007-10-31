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

namespace Sonority.UPnP
{
    public class ZoneGroup
    {
        public ZoneGroup(XPathNavigator node)
        {
            _coordinator = node.SelectSingleNode("@Coordinator").Value;
            _id = node.SelectSingleNode("@ID").Value;

            foreach (XPathNavigator nav in node.Select("ZoneGroupMember"))
            {
                _members.Add(new ZoneGroupMember(nav));
            }
        }

        public ObservableCollection<ZoneGroupMember> Members
        {
            get
            {
                return _members;
            }
        }

        public string Coordinator
        {
            get
            {
                return _coordinator;
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
        }

        private ObservableCollection<ZoneGroupMember> _members = new ObservableCollection<ZoneGroupMember>();
        private string _coordinator;
        private string _id;
    }
}
