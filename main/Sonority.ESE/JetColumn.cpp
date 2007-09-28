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
#include "JetColumn.h"
#include "JetException.h"

using namespace System;
//using namespace System::IO;
//using namespace System::IO::Compression;
//using namespace System::Security::Cryptography;
//using namespace System::Text;

JetColumn::JetColumn(
	JET_SESID sesid,
	JET_DBID dbid,
	JET_TABLEID tableid,
	LPCSTR columnName,
	JET_COLTYP columnType,
	UInt32 maxLength,
	JET_GRBIT grbit,
	UInt32 codePage) :
	_sesid(sesid),
	_dbid(dbid),
	_tableid(tableid)
{
	_column = new JET_COLUMNDEF();

	JET_ERR err;
	err = JetGetTableColumnInfo(_sesid, _tableid, columnName, _column, sizeof(JET_COLUMNDEF), JET_ColInfo);
	if (err == JET_errColumnNotFound)
	{
		_column->cbStruct = sizeof(JET_COLUMNDEF);
		_column->coltyp = columnType;
		_column->cbMax = maxLength;
		_column->grbit = grbit;
		_column->cp = codePage;

		JET_COLUMNID columnid = 0;
		err = JetCall(JetAddColumn(_sesid, _tableid, columnName, _column, NULL, 0, &columnid));
		_column->columnid = columnid;
	}

	JetCall(err);
}

JetColumn::~JetColumn()
{
	(*this).!JetColumn();
}

JetColumn::!JetColumn()
{
	JetCloseTable(_sesid, _tableid);

	delete _column;
	_column = NULL;
}

JET_COLUMNDEF*
JetColumn::operator ->()
{
	return _column;
}
