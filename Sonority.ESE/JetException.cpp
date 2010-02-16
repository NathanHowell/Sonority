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

#include "stdafx.h"
#include "JetException.h"

using namespace System;
using namespace System::IO;
using namespace System::IO::Compression;
using namespace System::Security::Cryptography;
using namespace System::Text;
using namespace System::Xml;
using namespace System::Xml::XPath;

JetException::JetException(
	JET_ERR err) :
	_err(err)
{
}

String^
JetException::ToString()
{
	JET_API_PTR err = _err;
	CHAR errString[1024];
	if (JetGetSystemParameter(NULL, NULL, JET_paramErrorToString, &err, errString, sizeof(errString)) == JET_errSuccess)
	{
		return gcnew String(errString);
	}
	else
	{
		return __super::ToString();
	}
}

JET_ERR JetCall(JET_ERR err)
{
	if (err != JET_errSuccess)
	{
		throw gcnew JetException(err);
	}

	return err;
}
