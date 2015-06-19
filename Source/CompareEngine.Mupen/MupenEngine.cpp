#include "Stdafx.h"

#pragma managed(push, off)
#include <stdio.h>
#include <stdlib.h>
#include <string>
#include "m64p_frontend.h"
#include "m64p_types.h"
#include "core_interface.h"
#pragma managed(pop)

#include "MupenEngine.h"

using namespace System;
using namespace System::IO;
using namespace Soft64;
using namespace std;

namespace CompareEngineMupen
{
	void MupenEngine::Init()
	{
		String^ dirName = String::Concat(System::Environment::CurrentDirectory->ToString(), "\\mupen64plus.dll");
		IntPtr strPtr = System::Runtime::InteropServices::Marshal::StringToCoTaskMemUni(dirName);
		char * path = reinterpret_cast<char*>(strPtr.ToPointer());

		AttachCoreLib(path);

		/* Grab reference of the PI Stream */
		Stream^ piStream = Cartridge::Current->PiCartridgeStream;
		piStream->Position = 0;
		int len = (int)piStream->Length;

		/* Allocate an unmanaged buffer */
		char* buffer = (char *)malloc(piStream->Length);

		/* ROM sizes will be guarenteed to be smaller the size of an integer */
		for (int i = 0; i < len; i++)
		{
			buffer[i] = (char)piStream->ReadByte();
		}

		/* Try opening the rom in mupen64plus core, mupen should see it as a z64 since its already swapped */
		if ((CoreDoCommand)(M64CMD_ROM_OPEN, len, buffer) != M64ERR_SUCCESS)
		{
			throw gcnew InvalidOperationException("Failed to open rom in mupen engine");
		}
	}

	Boolean MupenEngine::CompareState(ExecutionState^ state)
	{
		return false;
	}

	void MupenEngine::StepOnce()
	{

	}
}