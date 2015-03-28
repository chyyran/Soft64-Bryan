using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.Engines
{
    public abstract class EmulatorEngine : ILifetimeTrackable
    {
        private EngineTick m_CPUTick;


        public event EngineTick CPUTick
        {
            add
            {
                m_CPUTick = value;
            }

            remove
            {
                m_CPUTick = null;
            }
        }

        private void CreateTickThread(EngineTick tickEvent)
        {

        }


        #region ILifetimeTrackable Members

        public event EventHandler<LifeStateChangedArgs> LifetimeStateChanged;

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Stop()
        {
            throw new NotImplementedException();
        }

        public LifetimeState CurrentRuntimeState
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
