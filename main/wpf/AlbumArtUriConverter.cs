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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Xml.XPath;
using Sonority.XPath;

namespace wpf
{
    class AlbumArtUriConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return DependencyProperty.UnsetValue;
            }
            else if (values.Length != 2)
            {
                return DependencyProperty.UnsetValue;
            }
            else if (values[0] == null || values[1] == null)
            {
                return DependencyProperty.UnsetValue;
            }

            XPathNavigator nav = (XPathNavigator)values[0];
            XPathNavigator artNav = nav.SelectSingleNode(Expressions.AlbumArt);
            if (artNav == null)
            {
                return DependencyProperty.UnsetValue;
            }

            string art = artNav.Value;
            if (String.IsNullOrEmpty(art))
            {
                return DependencyProperty.UnsetValue;
            }

            if (Uri.IsWellFormedUriString(art, UriKind.Absolute))
            {
                // Pandora, etc where the full Uri is already given to use
                return new BitmapImage(new Uri(art));
            }
            else
            {
                Uri baseUri = (Uri)values[1];
                string path = art.Substring(0, art.IndexOf('?'));
                string qs = art.Substring(art.IndexOf('?'));
                UriBuilder builder = new UriBuilder(baseUri.Scheme, baseUri.Host, baseUri.Port, path, qs);
                return new BitmapImage(builder.Uri);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}