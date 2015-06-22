#pragma once

using namespace System;
using namespace Soft64;
using namespace Soft64::MipsR4300;
using namespace System::Threading;

namespace CompareEngine
{
	namespace Mupen
	{
		public ref class MupenEngine : public IMipsCompareEngine
		{
		public:
			MupenEngine();
			virtual void Init();
			virtual Boolean CompareState(ExecutionState^ state);
			virtual void Run();
			virtual void Release();

		private:
			EventWaitHandle^ m_CompareWaitEvent;

		internal:
			static MupenEngine^ CurrentEngine;
			virtual void CompareCoreWait(unsigned int);
		};
	}
}