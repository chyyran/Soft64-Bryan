#include "Stdafx.h"

#undef EXPORT
#define EXPORT __cdecl
#define M64P_CORE_PROTOTYPES

#pragma managed(push, off)
#include <stdio.h>
#include <stdlib.h>
#include <string>
#include "m64p_frontend.h"
#include "m64p_types.h"
#include "m64p_config.h"
#include "m64p_plugin.h"
#include "m64p_debugger.h"
#include "version.h"
#pragma managed(pop)

#include "MupenEngine.h"

using namespace System;
using namespace System::IO;
using namespace Soft64;
using namespace std;
using namespace System::Runtime::InteropServices;


namespace CompareEngine
{
	namespace Mupen
	{
		void CompareCoreCallback(unsigned int);
		int CompareInt64(void *, Int64);

		MupenEngine::MupenEngine()
		{
			m_CompareWaitEvent = gcnew EventWaitHandle(false, EventResetMode::AutoReset);
			MupenEngine::CurrentEngine = this;
		}

		void MupenEngine::Init()
		{

			if (Cartridge::Current == nullptr)
			{
				throw gcnew InvalidOperationException("No cartridge was inserted, compare cannot continue");
			}

			/* init the mupen core */
			/* TODO: Force mupen core to statically link into this assembly */
			m64p_error rval = CoreStartup(CORE_API_VERSION, ".\\", ".\\", "Core", NULL, NULL, NULL);
			if (rval != M64ERR_SUCCESS)
			{
				throw gcnew InvalidOperationException("Could not initialize the mupen core lib");
			}

			/* Setup some configuration */
			/* TODO: I don't know if setting this config crap makes any different as to the preprocessing I am using */
			m64p_handle* configHandle;
			ConfigOpenSection("Core", configHandle);
			ConfigSetParameter(configHandle, "R4300Emulator", M64TYPE_INT, "0");
			ConfigSetParameter(configHandle, "EnableDebugger", M64TYPE_BOOL, "1");
			ConfigSaveSection("Core");

			/* Setup core compare feature in mupen */
			DebugSetCoreCompare(CompareCoreCallback, NULL);

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
			if (CoreDoCommand(M64CMD_ROM_OPEN, len, buffer) != M64ERR_SUCCESS)
			{
				throw gcnew InvalidOperationException("Failed to open rom in mupen engine");
			}

			/* Setup dummy plugins */
			CoreAttachPlugin(M64PLUGIN_AUDIO, NULL);
			CoreAttachPlugin(M64PLUGIN_GFX, NULL);
			CoreAttachPlugin(M64PLUGIN_INPUT, NULL);
			CoreAttachPlugin(M64PLUGIN_RSP, NULL);
		}

		Boolean MupenEngine::CompareState(ExecutionState^ state)
		{
			/* TODO: Store mupen's values into an ExecutionState object, and compare that against the parameter */

			int comparePc = CompareInt64(DebugGetCPUDataPtr(M64P_CPU_PC), state->PC);

			return
				comparePc;
		}

		void MupenEngine::Release()
		{
			this->m_CompareWaitEvent->Set();
		}

		void MupenEngine::CompareCoreWait(unsigned int data)
		{
			/* Block mupen execution thread */
			this->m_CompareWaitEvent->WaitOne();
		}

		void MupenEngine::Run()
		{
			/* Run the emulator */
			CoreDoCommand(M64CMD_EXECUTE, 0, NULL);
		}


		void CompareCoreCallback(unsigned int data)
		{
			MupenEngine::CurrentEngine->CompareCoreWait(data);
		}

		int CompareInt64(void *ptrValue, Int64 value)
		{
			__int64 cValue = *(__int64*)ptrValue;

			if (value.Equals(cValue))
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
	}
}