using System.Numerics;

namespace ECSEngineTest;

public class Interpolatable<T> where T : struct
{
    public T Previous { get; private set; }
    public T Current { get; private set; }
    public T Target { get; private set; }

    public Interpolatable(T initialValue)
    {
        Previous = Current = Target = initialValue;
    }

    public Interpolatable() : this(default) { }

    public Interpolatable<T> Lerp(float amount)
    {
        Previous = Current;
        Current = Lerp(Current, Target, amount);

        return this;
    }

    public Interpolatable<T> Set(T value)
    {
        Previous = Current;
        Current = Target = value;

        return this;
    }

    private static T Lerp(T a, T b, float t)
    {
        object res;

        if (a is float af && b is float bf)
            res = float.Lerp(af, bf, t);

        else if (a is Vector2 av2 && b is Vector2 bv2)
            res = Vector2.Lerp(av2, bv2, t);

        else if (a is Vector3 av3 && b is Vector3 bv3)
            res = Vector3.Lerp(av3, bv3, t);

        else if (a is Vector4 av4 && b is Vector4 bv4)
            res = Vector4.Lerp(av4, bv4, t);

        else if (a is Quaternion aq && b is Quaternion bq)
            res = Quaternion.Slerp(aq, bq, t);

        else
            throw new ArgumentException($"Interpolation not supported for type {typeof(T)}");

        return (T)res;
    }

    public override string ToString()
        => Previous.ToString() + "  ->  " + Current.ToString() + "  ->  " + Target.ToString();
}
