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

namespace wpf
{
    class QueueItemsComparator : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 4)
            {
                return DependencyProperty.UnsetValue;
            }

            foreach (object obj in values)
            {
                if (obj == null || obj == DependencyProperty.UnsetValue)
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            try
            {
                // AVTransport.CurrentTrack == QueueItem.NumericID
                if (Convert.ToInt64(values[0]) != Convert.ToInt64(values[1]))
                {
                    return false;
                }

                // AVTransport.CurrentTrackURI == QueueItem.Res
                // this is a fix if you're playing something that is not in the queue.
                // Sonos doesn't update the CurrentTrack value, so we need to double
                // check that the URIs are identical
                return (Uri)values[2] == (Uri)values[3];
            }
            catch (InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
            }
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
