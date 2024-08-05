namespace MyEngine;

internal static class MathHelper
{
    public static float Lerp(this float val1, float val2, float weight)
    {
        return val1 * (1 - weight) + val2 * weight;
    }
}
