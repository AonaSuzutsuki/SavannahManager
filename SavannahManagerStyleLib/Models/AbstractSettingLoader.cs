using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavannahManagerStyleLib.Models
{
    public abstract class AbstractSettingLoader
    {
        protected abstract void Load();
        public abstract void Save();
    }
}
