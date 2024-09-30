using System.Drawing;
using System.Numerics;

namespace ECSEngineTest.Extensions;

public static class ColorExtensions
{
    public static Vector3 ToVector3RGB(this Color color)
    {
        return new Vector3(color.R, color.G, color.B);
    }

    public static Vector4 ToVector4RGBA(this Color color)
    {
        return new Vector4(color.R, color.G, color.B, color.A);
    }
}
