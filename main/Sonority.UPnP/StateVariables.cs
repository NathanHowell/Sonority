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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using UPNPLib;

namespace Sonority.UPnP
{
    // TODO: add field attributes
    internal static class StateVariables
    {
        // do upnp query on threadpool thread, and then queue results setter on dispatcher thread
        public static void Initialize<T>(T target, UPnPService service) where T : DispatcherObject, IUPnPServiceCallback
        {
            foreach (FieldInfo fi in target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (fi.Name.StartsWith("_"))
                {
                    ThreadPool.UnsafeQueueUserWorkItem(delegate { UpdateField(fi, target, service); }, null);
                }
            }
        }

        static private void UpdateField<T>(FieldInfo fi, T target, UPnPService service) where T : DispatcherObject, IUPnPServiceCallback
        {
            try
            {
                object stateVariable = Convert.ChangeType(service.QueryStateVariable(fi.Name.Substring(1)), fi.FieldType);
                target.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (ThreadStart)delegate { target.StateVariableChanged(service, fi.Name.Substring(1), stateVariable); });
            }
            catch (COMException e)
            {
                // this comes back for a reasonable percentage of the variables
                const int UPNP_E_DEVICE_ERROR = unchecked((int)0x80040214);
                if (e.ErrorCode == UPNP_E_DEVICE_ERROR)
                    return;

                // a few others fail here but they get updated by the service callbacks later...
                const int UPNP_E_INVALID_VARIABLE = unchecked((int)0x80040213);
                if (e.ErrorCode == UPNP_E_INVALID_VARIABLE)
                    return;

                // dunno about this one
                const int UPNP_E_VARIABLE_VALUE_UNKNOWN = unchecked((int)0x80040212);
                if (e.ErrorCode == UPNP_E_VARIABLE_VALUE_UNKNOWN)
                    return;

                Console.WriteLine("StateVariable Exception @ {0}: {1}", fi.Name, e.ToString());
            }
        }

        public static void Changed(object target, UPnPService pus, string stateVariable, object value)
        {
            string fieldName = String.Format("_{0}", stateVariable);
            FieldInfo fi = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi == null)
            {
                Console.Error.WriteLine("Field not found: {0}", fieldName);
                return;
            }

            if (fi.FieldType.IsEnum)
            {
                fi.SetValue(target, Enum.Parse(fi.FieldType, value.ToString(), true));
            }
            else
            {
                fi.SetValue(target, Convert.ChangeType(value, fi.FieldType));
            }
        }
    }
}