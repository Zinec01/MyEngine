using ECSEngineTest.Extensions;
using Friflo.Engine.ECS;
using System.Drawing;
using System.Numerics;

namespace ECSEngineTest.Components;

public struct ColorComponent : IComponent
{
    public Vector4 Color { get; set; } = Vector4.One;

    public ColorComponent() { }

    public ColorComponent(Vector4 color)
    {
        Color = color;
    }

    public ColorComponent(Color color)
    {
        Color = color.ToVector4RGBA();
    }
}
