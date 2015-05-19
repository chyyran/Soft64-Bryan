using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64
{
    public interface IConfiguration
    {
        void SaveConfig(IDictionary<String, Object> propertyBag);

        void ReadConfig(IDictionary<String, Object> proeprtyBag);
    }
}
