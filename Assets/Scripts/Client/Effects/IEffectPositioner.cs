using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IEffectPositioner
    {
        void ApplyPositioning(IEffectEntity effectEntity, IEffectPositionerSettings settings);
    }
}
