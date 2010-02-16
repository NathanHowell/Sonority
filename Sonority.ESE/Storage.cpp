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
#include "Storage.h"
#include "JetException.h"

using namespace System;
using namespace System::IO;
using namespace System::Text;
using namespace System::Xml;
using namespace System::Xml::XPath;

SonorityStorage::SonorityStorage()
{
	JET_ERR err;
	JET_INSTANCE instance;
	err = JetCall(JetCreateInstance(&instance, NULL));
	err = JetCall(JetSetSystemParameterW(&instance, NULL, JET_paramBaseName, 0, L"sns"));
	err = JetCall(JetSetSystemParameterW(&instance, NULL, JET_paramCircularLog, TRUE, NULL));
	err = JetCall(JetSetSystemParameterW(&instance, NULL, JET_paramLogFileSize, 1024, NULL));
	err = JetCall(JetSetSystemParameterW(&instance, NULL, JET_paramLegacyFileNames , 0, NULL));		
	// hopefully prevent more than 1 or 2 transaction log files laying around:
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

void
SonorityStorage::AddRecord(
	String^ objectID,
	String^ parentID,
	UInt32 updateID,
	XPathNavigator^ metaData)
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

SonorityStorage::~SonorityStorage()
{
	JET_ERR err;
	err = JetCloseDatabase(_sesid, _dbid, 0);
	err = JetEndSession(_sesid, 0);
	err = JetTerm2(_instance, JET_bitTermComplete);
}
