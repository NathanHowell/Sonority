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

#pragma once
#include "JetColumn.h"

ref class JetTableHelper
{
protected:
	JetTableHelper(
		JET_SESID sesid,
		JET_DBID dbid,
		LPCSTR tableName);
	~JetTableHelper();
	!JetTableHelper();

	JetColumn^ GetColumn(
		LPCSTR columnName,
		JET_COLTYP columnType,
		UInt32 maxLength,
		JET_GRBIT grbit);

	template <size_t N>
	void EnsureIndex(LPCSTR indexName, const char (&key)[N], JET_GRBIT grbit)
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

	static array<Byte>^ HashString(String^ str);

protected:
	JET_SESID _sesid;
	JET_DBID _dbid;
	JET_TABLEID _tableid;
};
