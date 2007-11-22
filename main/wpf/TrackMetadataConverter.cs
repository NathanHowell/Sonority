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
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Xml.XPath;
using Sonority.XPath;

namespace wpf
{
    class TrackMetadataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            XPathNavigator nav = (XPathNavigator)value;
            if (parameter == null)
            {
                return String.Format("Artist: {0} / Album: {1} / Track: {2}",
                    nav.SelectSingleNode(Expressions.Creator).Value,
                    nav.SelectSingleNode(Expressions.Album).Value,
                    nav.SelectSingleNode(Expressions.Title).Value);
            }
            else
            {
                FieldInfo fi = typeof(Expressions).GetField(parameter.ToString(), BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);
                XPathExpression e = (XPathExpression)fi.GetValue(null);
                return nav.SelectSingleNode(e).Value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
