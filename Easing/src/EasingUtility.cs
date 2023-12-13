using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senkel.Utility.Easing
{
    public class EasingUtility
    {
        public float EasePolymonial(float t, float power)
        {
            return MathF.Pow(t, power);
        }
    }
}
