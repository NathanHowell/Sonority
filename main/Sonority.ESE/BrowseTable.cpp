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
#include "BrowseTable.h"
#include "JetException.h"

using namespace System;
using namespace System::IO;
using namespace System::IO::Compression;
using namespace System::Text;
using namespace System::Xml;
using namespace System::Xml::XPath;

BrowseTable::BrowseTable(
	JET_SESID sesid,
	JET_DBID dbid) :
	JetTableHelper(sesid, dbid, "Browse")
{
	_objectID = GetColumn("ObjectID", JET_coltypBinary, 16, JET_bitColumnNotNULL);
	_updateID = GetColumn("UpdateID", JET_coltypUnsignedLong, 0, JET_bitColumnNotNULL);
	_updateDateTime = GetColumn("UpdateDateTime", JET_coltypDateTime, sizeof(DATE), JET_bitColumnNotNULL);
	_metaData = GetColumn("MetaData", JET_coltypLongBinary, 4 * 1024, JET_bitColumnNotNULL);
	EnsureIndex(ObjectIndex, "+ObjectID\0", JET_bitIndexPrimary | JET_bitIndexUnique | JET_bitIndexDisallowTruncation | JET_bitIndexLazyFlush);
}

BrowseTable::~BrowseTable()
{
	delete _objectID;
	delete _updateID;
	delete _updateDateTime;
	delete _metaData;
}

void
BrowseTable::UpdateRecord(
	String^ objectID,
	String^ parentID,
	UInt32 updateID,
	XPathNavigator^ metaData)
{
	array<Byte>^ hashObjectID = HashString(objectID);
	pin_ptr<Byte> pinnedObjectID = &hashObjectID[0];

	array<Byte>^ hashParentID = HashString(parentID);
	pin_ptr<Byte> pinnedParentID = &hashParentID[0];
	DATE updateTime = DateTime::UtcNow.ToOADate();

	JET_ERR err;
	err = JetCall(JetSetCurrentIndex(_sesid, _tableid, ObjectIndex));
	err = JetCall(JetMakeKey(_sesid, _tableid, pinnedObjectID, hashObjectID->Length, JET_bitNewKey));
	err = JetSeek(_sesid, _tableid, JET_bitSeekEQ);
	if (err == JET_errRecordNotFound)
	{
		Console::WriteLine("Adding: {0}", objectID);

		// use XPathNavigator::WriteSubtree to promote metadata namespaces?
		StringBuilder outerXml;
		XmlWriter^ writer = XmlWriter::Create(%outerXml);
		metaData->WriteSubtree(writer);
		writer->Flush();
		MemoryStream stream(512);
		GZipStream gzip(%stream, CompressionMode::Compress);
		gzip.Write(Encoding::UTF8->GetBytes(outerXml.ToString()), 0, outerXml.Length);
		gzip.Flush();

		array<Byte>^ compressedMetaData = stream.GetBuffer();
		pin_ptr<Byte> pinnedMetaData = &compressedMetaData[0];

		err = JetCall(JetPrepareUpdate(_sesid, _tableid, JET_prepInsert));

		try
		{
			err = JetCall(JetSetColumn(_sesid, _tableid, _objectID->columnid, pinnedObjectID, hashObjectID->Length, 0, NULL));
			err = JetCall(JetSetColumn(_sesid, _tableid, _updateID->columnid, &updateID, sizeof(updateID), 0, NULL));
			err = JetCall(JetSetColumn(_sesid, _tableid, _updateDateTime->columnid, &updateTime, sizeof(updateTime), 0, NULL));
			err = JetCall(JetSetColumn(_sesid, _tableid, _metaData->columnid, pinnedMetaData, static_cast<unsigned long>(stream.Length), 0, NULL));
			err = JetCall(JetUpdate(_sesid, _tableid, NULL, 0, NULL));
		}
		catch (JetException^)
		{
			JetPrepareUpdate(_sesid, _tableid, JET_prepCancel);
			throw;
		}
	}
	else
	{
		Console::WriteLine("Skipping: {0}", objectID);
		JetCall(err);
	}
}
