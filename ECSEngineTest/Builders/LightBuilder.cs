using ECSEngineTest.Components;
using ECSEngineTest.Tags;
using Friflo.Engine.ECS;
using System.Numerics;

namespace ECSEngineTest.Builders;

public class LightBuilder
{
    private readonly EntityStore _store;
    private readonly string _name;
    private readonly LightType _type;

    private Vector3 _position = Vector3.Zero;
    private Quaternion _rotation = Quaternion.Identity;
    private Matrix4x4 _transform = Matrix4x4.Identity;
    private float _intensity = 1.0f;
    private Vector4 _color = Vector4.One;
    private float _attenuation = 1.0f;
    private float _innerCone = 0.0f;
    private float _outerCone = 0.0f;

    public LightBuilder(EntityStore store, string name, LightType type)
    {
        _store = store;
        _name = name;
        _type = type;
    }

    public LightBuilder SetPosition(Vector3 position)
    {
        _position = position;
        return this;
    }

    public LightBuilder SetRotation(Quaternion rotation)
    {
        _rotation = rotation;
        return this;
    }

    public LightBuilder SetTransform(Matrix4x4 transform)
    {
        _transform = transform;
        return this;
    }

    public LightBuilder SetIntensity(float intensity)
    {
        _intensity = intensity;
        return this;
    }

    public LightBuilder SetColor(Vector3 color)
    {
        _color = new Vector4(color, 1.0f);
        return this;
    }

    public LightBuilder SetColor(Vector4 color)
    {
        _color = color;
        return this;
    }

    public LightBuilder SetAttenuation(float attenuation)
    {
        _attenuation = attenuation;
        return this;
    }

    public LightBuilder SetInnerOuterConeAngles(float innerCone, float outerCone)
    {
        _innerCone = innerCone;
        _outerCone = outerCone;
        return this;
    }

    public Entity Build()
    {
        var entity = _store.CreateEntity(new EntityName(_name));
        entity.AddTag<LightTag>();
        entity.AddComponent(new LightComponent
        {
            Type = _type,
            Attenuation = -_attenuation,
            InnerConeAngle = _innerCone,
            OuterConeAngle = _outerCone,
            Intensity = _intensity
        });
        entity.AddComponent(new ColorComponent(_color));
        entity.AddComponent(_transform.IsIdentity
                            ? new TransformComponent(_position, _rotation, Vector3.One)
                            : new TransformComponent(_transform));

        return entity;
    }
}
