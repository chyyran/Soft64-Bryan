#pragma once

using namespace System;
using namespace Soft64;
using namespace Soft64::MipsR4300;

namespace CompareEngineMupen
{
	public ref class MupenEngine : public IMipsCompareEngine
	{
	public:
		virtual void Init();
		virtual Boolean CompareState(ExecutionState^ state);
		virtual void StepOnce();
	};
}