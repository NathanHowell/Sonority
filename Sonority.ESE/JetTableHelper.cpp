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
#include "JetTableHelper.h"
#include "JetException.h"
#include "JetColumn.h"

using namespace System;
using namespace System::Security::Cryptography;

JetTableHelper::JetTableHelper(
	JET_SESID sesid,
	JET_DBID dbid,
	LPCSTR tableName) :
	_sesid(sesid),
	_dbid(dbid)
{
	JET_ERR err;
	JET_TABLEID tableid;
	err = JetOpenTable(_sesid, _dbid, tableName, NULL, 0, JET_bitTableUpdatable, &tableid);
	if (err == JET_errObjectNotFound)
	{
		err = JetCreateTable(_sesid, _dbid, tableName, 64, 90, &tableid);
	}

	_tableid = tableid;

	JetCall(err);
}

JetTableHelper::~JetTableHelper()
{
	this->!JetTableHelper();
}

JetTableHelper::!JetTableHelper()
{
	JetCloseTable(_sesid, _tableid);
}

JetColumn^
JetTableHelper::GetColumn(
	LPCSTR columnName,
	JET_COLTYP columnType,
	UInt32 maxLength,
	JET_GRBIT grbit)
{
	return gcnew JetColumn(
		_sesid,
		_dbid,
		_tableid,
		columnName,
		columnType,
		maxLength,
		grbit,
		0);
}

#if 0
template <size_t N>
void
JetTableHelper::EnsureIndex(
	LPCSTR indexName,
	const char (&key)[N],
	JET_GRBIT grbit)
{
	JET_INDEXCREATE indexCreate = { 0 };
	indexCreate.cbStruct = sizeof(indexCreate);
	indexCreate.szIndexName = const_cast<char *>(indexName);
	indexCreate.szKey = const_cast<char *>(key);
	indexCreate.cbKey = N;
	indexCreate.grbit = grbit;
	indexCreate.ulDensity = 90;
	indexCreate.lcid = 0;
	indexCreate.cbKeyMost = JET_cbKeyMostMin;
	JET_ERR err = JetCreateIndex2(
		_sesid,
		_tableid,
		&indexCreate,
		1);
	if (err != JET_errIndexDuplicate && err != JET_errIndexHasPrimary)
	{
		JetCall(err);
	}
}
#endif

array<Byte>^
JetTableHelper::HashString(
	String^ str)
{
	MD5CryptoServiceProvider hash;
	array<Byte>^ utf8 = System::Text::Encoding::UTF8->GetBytes(str);
	hash.TransformFinalBlock(utf8, 0, utf8->Length);
	return hash.Hash;
}
