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
using System.Reflection;
using System.Runtime.InteropServices;
using UPNPLib;

namespace Sonority.UPnP
{
    // TODO: add field attributes
    internal static class StateVariables
    {
        public static void Initialize(object target, UPnPService service)
        {
            foreach (FieldInfo fi in target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (fi.Name.StartsWith("_"))
                {
                    try
                    {
                        fi.SetValue(target, Convert.ChangeType(service.QueryStateVariable(fi.Name.Substring(1)), fi.FieldType));
                    }
                    catch (COMException e)
                    {
                        // this comes back for a reasonable percentage of the variables
                        const int UPNP_E_DEVICE_ERROR = unchecked((int)0x80040214);
                        if (e.ErrorCode == UPNP_E_DEVICE_ERROR)
                            continue;

                        // a few others fail here but they get updated by the service callbacks later...
                        const int UPNP_E_INVALID_VARIABLE = unchecked((int)0x80040213);
                        if (e.ErrorCode == UPNP_E_INVALID_VARIABLE)
                            continue;

                        // dunno about this one
                        const int UPNP_E_VARIABLE_VALUE_UNKNOWN = unchecked((int)0x80040212);
                        if (e.ErrorCode == UPNP_E_VARIABLE_VALUE_UNKNOWN)
                            continue;

                        Console.WriteLine("StateVariable Exception @ {0}: {1}", fi.Name, e.ToString());
                    }
                }
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

            fi.SetValue(target, Convert.ChangeType(value, fi.FieldType));
        }
    }
}