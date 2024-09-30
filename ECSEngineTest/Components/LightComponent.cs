using Friflo.Engine.ECS;

namespace ECSEngineTest.Components;

internal struct LightComponent : IComponent
{
    public LightType Type { get; set; }
    public float Intensity { get; set; }
    public float Attenuation { get; set; }
    public float InnerConeAngle { get; set; }
    public float OuterConeAngle { get; set; }
}

public enum LightType
{
    Undefined = 0,
    Directional = 1,
    Point = 2,
    Spot = 3,
    Ambient = 4,
    Area = 5
}
