using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senkel.Utility.Easing
{
    public static class EasingUtility
    {
        public static float EaseInOut(float t, EasingFunction easeInFunction, EasingFunction easeOutFunction)
        {
            t *= 2;

            if (t < 1f)
                return easeInFunction.EaseIn(t) * 0.5f;

            return easeOutFunction.EaseOut(t) * 0.5f + 0.5f;
        }
         
    }
 
}
