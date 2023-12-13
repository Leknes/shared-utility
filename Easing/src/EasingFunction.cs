using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senkel.Utility.Easing;

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

public abstract class EasingFunction
{
    public float EaseOut(float t)
    {
        return 1 - EaseIn(1 - t);
    }

    public float EaseInOut(float t)
    {
        return EasingUtility.EaseInOut(t, this, this);
    }

    public abstract float EaseIn(float t);
}

public class PowerEasingFunction : EasingFunction
{
    private readonly float _power;
    public PowerEasingFunction(float power)
    {
        _power = power;
    }

    public override float EaseIn(float t)
    {
        return MathF.Pow(t, _power);      
    }
}

public class ExponentialEasingFunction : EasingFunction
{
    private readonly float _exponent;

    public ExponentialEasingFunction(float exponent)
    {
        _exponent = exponent;
    }


    public override float EaseIn(float t)
    {
        return (MathF.Exp(_exponent * t) - 1) / (MathF.Exp(_exponent) - 1);
    }
}

public class CircleEasingFunction : EasingFunction
{ 
    public override float EaseIn(float t)
    {
        return 1 - MathF.Sqrt(1 - MathF.Pow(t, 2));
    }
}

public class SineEasingFunction : EasingFunction
{ 
    public override float EaseIn(float t)
    {
        return 1 - (MathF.Sin(1 - t) + MathF.PI * 0.5f);
    }
}

public class ElasticEasingFunction : EasingFunction
{
    private readonly int _oscillations;
    private readonly float _springiness;

    public ElasticEasingFunction(int oscillations = 3, float springiness = 3)
    {
        _oscillations = oscillations;
        _springiness = springiness;
    }

     
    public override float EaseIn(float t)
    {
        float exponent;

        if (_springiness == 0)
            exponent = t;
        else
            exponent = (MathF.Exp(_springiness * t) - 1.0f) / (MathF.Exp(_springiness) - 1.0f);

        return exponent * MathF.Sin((MathF.PI * 2.0f * _oscillations + MathF.PI * 0.5f) * t);
    }
}

public class BounceEasingFunction : EasingFunction
{
    private readonly float _bounciness;
    private readonly int _bounces;

    public BounceEasingFunction(int bounces = 3, float bounciness = 2)
    {
        _bounciness = bounciness;
        _bounces = bounces;
    }
     
    public override float EaseIn(float t)
    {
        // The math below is complicated because we have a few requirements to get the correct look for bounce:
        //  1) The bounces should be symetrical
        //  2) Bounciness should control both the amplitude and the period of the bounces
        //  3) Bounces should control the number of bounces without including the final half bounce to get you back to 1.0 
        //
        //  Note: Simply modulating a expo or power curve with a abs(sin(...)) wont work because it violates 1) above. 

        // Constants 
        int bounces = _bounces;
        float bounciness = _bounciness;

        // Clamp the bounciness so we dont hit a divide by zero 
        if (bounciness <= 1.0f)
            bounciness = 1.0000001f;

        float pow = MathF.Pow(bounciness, bounces);
        float oneMinusBounciness = 1.0f - bounciness;

        // 'unit' space calculations.
        // Our bounces grow in the x axis exponentially.  we define the first bounce as having a 'unit' width of 1.0 and compute 
        // the total number of 'units' using a geometric series. 
        // We then compute which 'unit' the current time is in.
        float sumOfUnits = (1.0f - pow) / oneMinusBounciness + pow * 0.5f; // geometric series with only half the last sum 
        float unitAtT = t * sumOfUnits;

        // 'bounce' space calculations.
        // Now that we know which 'unit' the current time is in, we can determine which bounce we're in by solving the geometric equation: 
        // unitAtT = (1 - bounciness^bounce) / (1 - bounciness), for bounce.
        float bounceAtT = MathF.Log(-unitAtT * (1.0f - bounciness) + 1.0f, bounciness);
        float start = MathF.Floor(bounceAtT);
        float end = start + 1.0f;

        // 'time' space calculations.
        // We then project the start and end of the bounce into 'time' space
        float startTime = (1.0f - MathF.Pow(bounciness, start)) / (oneMinusBounciness * sumOfUnits);
        float endTime = (1.0f - MathF.Pow(bounciness, end)) / (oneMinusBounciness * sumOfUnits);

        // Curve fitting for bounce. 
        float midTime = (startTime + endTime) * 0.5f;
        float timeRelativeToPeak = t - midTime;
        float radius = midTime - startTime;
        float amplitude = MathF.Pow(1.0f / bounciness, (bounces - start));

        // Evaluate a quadratic that hits (startTime,0), (endTime, 0), and peaks at amplitude.
        return (-amplitude / (radius * radius)) * (timeRelativeToPeak - radius) * (timeRelativeToPeak + radius);
    }
}

public class BackEasingFunction : EasingFunction
{
    private readonly float _amplitude;

    public BackEasingFunction(float amplitude = 1f)
    {
        _amplitude = amplitude;
    }

    public override float EaseIn(float t)
    {
        return MathF.Pow(t, 3.0f) - t * _amplitude * MathF.Sin(MathF.PI * t);
    }
}
