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
#include "Sonority.ESE.h"

using namespace System;
using namespace System::IO;
using namespace System::IO::Compression;
using namespace System::Security::Cryptography;
using namespace System::Text;
using namespace System::Xml;
using namespace System::Xml::XPath;

public ref class JetException : Exception
{
public:
	JetException(JET_ERR err) : _err(err)
	{
	}

	virtual String^ ToString() override
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

private:
	JET_INSTANCE _instance;
	JET_SESID _sesid;
	JET_ERR _err;
};

JET_ERR JetCall(JET_ERR err)
{
	if (err != JET_errSuccess)
	{
		throw gcnew JetException(err);
	}

	return err;
}

ref class JetColumn
{
public:
	JetColumn(
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

		(err);
	}

	~JetColumn()
	{
		(*this).!JetColumn();
	}

	!JetColumn()
	{
		JetCloseTable(_sesid, _tableid);

		delete _column;
		_column = NULL;
	}

	JET_COLUMNDEF* operator ->()
	{
		return _column;
	}

private:
	JET_SESID _sesid;
	JET_DBID _dbid;
	JET_TABLEID _tableid;

	JET_COLUMNDEF* _column;
};

ref class JetTableHelper
{
protected:
	JetTableHelper(
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

	~JetTableHelper()
	{
		this->!JetTableHelper();
	}

	!JetTableHelper()
	{
		JetCloseTable(_sesid, _tableid);
	}

	JetColumn^ GetColumn(
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

	static array<Byte>^ HashString(String^ str)
	{
		MD5CryptoServiceProvider hash;
		array<Byte>^ utf8 = System::Text::Encoding::UTF8->GetBytes(str);
		hash.TransformFinalBlock(utf8, 0, utf8->Length);
		return hash.Hash;
	}

protected:
	JET_SESID _sesid;
	JET_DBID _dbid;
	JET_TABLEID _tableid;
};

ref class BrowseTable : JetTableHelper
{
internal:
	BrowseTable(
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

	~BrowseTable()
	{
		delete _objectID;
		delete _updateID;
		delete _updateDateTime;
		delete _metaData;
	}

	void UpdateRecord(String^ objectID, String^ parentID, UInt32 updateID, XPathNavigator^ metaData)
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

internal:
	JetColumn^ _objectID;
	JetColumn^ _updateID;
	JetColumn^ _updateDateTime;
	JetColumn^ _metaData;

	static LPCSTR ObjectIndex = "ObjectIndex";
};

ref class RelationsTable : JetTableHelper
{
internal:
	RelationsTable(
		JET_SESID sesid,
		JET_DBID dbid) :
		JetTableHelper(sesid, dbid, "Relations")
	{
		_objectID = GetColumn("ObjectID", JET_coltypBinary, 16, JET_bitColumnNotNULL);
		_parentID = GetColumn("ParentID", JET_coltypBinary, 16, JET_bitColumnNotNULL);
		_updateID = GetColumn("UpdateID", JET_coltypUnsignedLong, 0, JET_bitColumnNotNULL);
		_updateDateTime = GetColumn("UpdateDateTime", JET_coltypDateTime, 0, JET_bitColumnNotNULL);
		EnsureIndex(ParentIndex, "+ParentID\0+ObjectID\0", JET_bitIndexPrimary | JET_bitIndexUnique | JET_bitIndexDisallowTruncation | JET_bitIndexLazyFlush);
	}

	~RelationsTable()
	{
		delete _objectID;
		delete _parentID;
		delete _updateID;
		delete _updateDateTime;
	}

	void UpdateRecord(String^ objectID, String^ parentID, UInt32 updateID)
	{
		array<Byte>^ hashObjectID = HashString(objectID);
		pin_ptr<Byte> pinnedObjectID = &hashObjectID[0];

		array<Byte>^ hashParentID = HashString(parentID);
		pin_ptr<Byte> pinnedParentID = &hashParentID[0];
		DATE updateTime = DateTime::UtcNow.ToOADate();

		JET_ERR err;
		err = JetCall(JetSetCurrentIndex(_sesid, _tableid, ParentIndex));
		err = JetCall(JetMakeKey(_sesid, _tableid, pinnedParentID, hashParentID->Length, JET_bitNewKey));
		err = JetCall(JetMakeKey(_sesid, _tableid, pinnedObjectID, hashObjectID->Length, 0));
		err = JetSeek(_sesid, _tableid, JET_bitSeekEQ);
		if (err == JET_errRecordNotFound)
		{
			Console::WriteLine("Adding: {0} -> {1}", objectID, parentID);
			err = JetCall(JetPrepareUpdate(_sesid, _tableid, JET_prepInsert));
			try
			{
				err = JetCall(JetSetColumn(_sesid, _tableid, _objectID->columnid, pinnedObjectID, hashObjectID->Length, 0, NULL));
				err = JetCall(JetSetColumn(_sesid, _tableid, _parentID->columnid, pinnedParentID, hashParentID->Length, 0, NULL));
				err = JetCall(JetSetColumn(_sesid, _tableid, _updateID->columnid, &updateID, sizeof(updateID), 0, NULL));
				err = JetCall(JetSetColumn(_sesid, _tableid, _updateDateTime->columnid, &updateTime, sizeof(updateTime), 0, NULL));
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
			Console::WriteLine("Skipping: {0} -> {1}", objectID, parentID);
			JetCall(err);
		}
	}

internal:
	JetColumn^ _objectID;
	JetColumn^ _parentID;
	JetColumn^ _updateID;
	JetColumn^ _updateDateTime;

	static LPCSTR ParentIndex = "ParentIndex";
};

public ref class foo
{
public:
	foo()
	{
		JET_ERR err;
		JET_INSTANCE instance;
		err = JetCall(JetCreateInstance(&instance, NULL));
		err = JetCall(JetSetSystemParameterW(&instance, NULL, JET_paramBaseName, 0, L"sns"));
		err = JetCall(JetSetSystemParameterW(&instance, NULL, JET_paramCircularLog, TRUE, NULL));
		err = JetCall(JetSetSystemParameterW(&instance, NULL, JET_paramLogFileSize, 1024, NULL));
		err = JetCall(JetSetSystemParameterW(&instance, NULL, JET_paramLegacyFileNames , 0, NULL));
		err = JetCall(JetSetSystemParameterW(&instance, NULL, JET_paramCheckpointDepthMax, 1 * 1024 * 1024, NULL));
		err = JetCall(JetSetSystemParameterW(&instance, NULL, JET_paramEnableIndexChecking, TRUE, NULL));
		err = JetCall(JetInit2(&instance, JET_bitTruncateLogsAfterRecovery));
		_instance = instance;

		JET_SESID sesid;
		err = JetCall(JetBeginSessionW(_instance, &sesid, NULL, NULL));
		_sesid = sesid;

		err = JetAttachDatabase(_sesid, s_fileName, JET_bitDbDeleteCorruptIndexes);
		if (err != JET_errFileNotFound)
		{
			JET_DBID dbid;
			err = JetCall(JetOpenDatabase(_sesid, s_fileName, NULL, &dbid, 0));
			_dbid = dbid;
		}
		else // (err == JET_errFileNotFound)
		{
			JET_DBID dbid;
			err = JetCall(JetCreateDatabase(_sesid, s_fileName, NULL, &dbid, JET_bitDbOverwriteExisting));
			_dbid = dbid;
		}
		JetCall(err);

		err = JetCall(JetBeginTransaction(_sesid));

		try
		{
			_browse = gcnew BrowseTable(_sesid, _dbid);
			_relations = gcnew RelationsTable(_sesid, _dbid);
			err = JetCall(JetCommitTransaction(_sesid, 0));
		}
		catch (Exception^)
		{
			JetRollback(_sesid, 0);
			throw;
		}
	}

	void AddRecord(String^ objectID, String^ parentID, UInt32 updateID, XPathNavigator^ metaData)
	{
		JET_ERR err;
		err = JetCall(JetBeginTransaction(_sesid));

		try
		{
			_browse->UpdateRecord(objectID, parentID, updateID, metaData);
			_relations->UpdateRecord(objectID, parentID, updateID);

			err = JetCall(JetCommitTransaction(_sesid, 0));
		}
		catch (Exception^)
		{
			err = JetRollback(_sesid, 0);
			throw;
		}
	}

	~foo()
	{
		JET_ERR err;
		err = JetCloseDatabase(_sesid, _dbid, 0);
		err = JetEndSession(_sesid, 0);
		err = JetTerm2(_instance, JET_bitTermComplete);
	}

private:
	JET_INSTANCE _instance;
	JET_SESID _sesid;
	JET_DBID _dbid;

	BrowseTable^ _browse;
	RelationsTable^ _relations;

	static LPCSTR s_fileName = "snstags.edb";
};
