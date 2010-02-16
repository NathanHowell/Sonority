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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using System.Web;
using Sonority.UPnP;
using System.Windows.Threading;
using UPNPLib;

namespace Sonority.Test
{
    class Program
    {
        static Discover _disc = new Discover();

        static void ThreadFunc()
        {
            string masterBedroom = "uuid:RINCON_000E5810848C01400";
            ZonePlayer zonePlayer = new ZonePlayer(masterBedroom);
            Console.WriteLine(zonePlayer.DeviceProperties.Icon);
            //UPnPDevice mediaServer = zonePlayer.MediaServer;
            //UPnPDevice mediaRenderer = zonePlayer.MediaRenderer;
            //AudioIn audioIn = zonePlayer.AudioIn;
            DeviceProperties deviceProperties = zonePlayer.DeviceProperties;
            RenderingControl renderingControl = zonePlayer.RenderingControl;
            AVTransport avTransport = zonePlayer.AVTransport;
            ContentDirectory contentDirectory = zonePlayer.ContentDirectory;
            GroupManagement groupManagement = zonePlayer.GroupManagement;
            //ZoneGroupTopology zoneGroupTopology = zonePlayer.ZoneGroupTopology;
            //ConnectionManager connectionManager = zonePlayer.ConnectionManager;

            foreach (XPathNavigator node in contentDirectory.Browse("0", BrowseFlags.BrowseDirectChildren, "*", ""))
            {
                Console.WriteLine(node.OuterXml);

#if FOO
                string path = HttpUtility.UrlDecode(node.SelectSingleNode("@id", Sonority.XPath.Globals.Manager).Value);
                path = path.Replace(@"S://", @"\\");
                path = path.Replace('/', '\\');
                string destination = Path.Combine(@"c:\temp\nuevo", Path.GetFileName(path));
                Console.WriteLine("{0}", path);

                File.Copy(path, destination);
#endif
            }

            foreach (QueueItem node in zonePlayer.Queue)
            {
                Console.WriteLine(node);
            }
            zonePlayer.Queue.ToString();

            avTransport.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
            contentDirectory.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
            renderingControl.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);

#if FOO
            using (foo bar = new foo())
            {
                // check updateIDs first
                Queue<string> queue = new Queue<string>();
                queue.Enqueue("S:");
                queue.Enqueue("0");

                while (queue.Count > 0)
                {
                    string currentObject = queue.Dequeue();
                    // box is down, ignore for now.
                    if (currentObject.StartsWith("S://13CFB6DD7F03432", StringComparison.OrdinalIgnoreCase))
                        continue;

                    foreach (XPathNavigator node in contentDirectory.Browse(currentObject, BrowseFlags.BrowseDirectChildren, "*", ""))
                    {
                        if (node.LocalName == "container")
                        {
                            // recurse into container
                            queue.Enqueue(node.SelectSingleNode("@id").Value);
                        }

                        bar.AddRecord(
                            node.SelectSingleNode("@id", XPath.Globals.Manager).Value,
                            node.SelectSingleNode("@parentID", XPath.Globals.Manager).Value,
                            0,
                            node);
                    }
                }
            }

            Thread.Sleep(Timeout.Infinite);

            Console.WriteLine(contentDirectory.ToString());
            Console.WriteLine(avTransport.ToString());


            // avTransport.Play("1");
#endif

            while (true)
            {
                Thread.Sleep(30000);

                foreach (ZoneGroup zp in disc.Topology)
                {
                    Console.WriteLine("{0} -> {1}", zp.Coordinator.UniqueDeviceName, zp.Coordinator.DeviceProperties.ZoneName);
                }
            }
        }

        static void Main(string[] args)
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.MTA);

            Discover disc = new Discover();
            disc.PropertyChanged += new PropertyChangedEventHandler(OnZonePlayerChanged);

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(ThreadFunc));

            Dispatcher.Run();
        }

        static void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("Changed: {0}.{1} -> {2}", sender.GetType().Name, e.PropertyName, sender.GetType().GetProperty(e.PropertyName).GetValue(sender, null));
        }

        static void OnZonePlayerChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("Changed: {0}.{1} -> {2}", sender.GetType().Name, e.PropertyName, sender.GetType().GetProperty(e.PropertyName).GetValue(sender, null));           
        }
    }
}
