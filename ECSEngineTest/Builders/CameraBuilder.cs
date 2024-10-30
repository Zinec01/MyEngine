using ECSEngineTest.Components;
using ECSEngineTest.Tags;
using Friflo.Engine.ECS;
using System.Numerics;

namespace ECSEngineTest.Builders;

public class CameraBuilder(EntityStore store, string name)
{
    private readonly string _name = name;
    private Quaternion _rotation = Quaternion.Identity;
    private Vector3 _position = Vector3.Zero;
    private Matrix4x4 _transform = Matrix4x4.Identity;
    private float _fov = 90.0f;
    private float _nearPlane = 0.1f;
    private float _farPlane = 1000.0f;
    private float _aspectRatio = 16.0f / 9.0f;
    private bool _active = false;

    public CameraBuilder SetRotation(Quaternion rotation)
    {
        _rotation = rotation;

        return this;
    }

    public CameraBuilder SetPosition(Vector3 position)
    {
        _position = position;

        return this;
    }

    public CameraBuilder SetTransformation(Matrix4x4 transformation)
    {
        _transform = transformation;

        return this;
    }

    public CameraBuilder SetFieldOfView(float fieldOfView)
    {
        _fov = fieldOfView;

        return this;
    }

    public CameraBuilder SetNearFarClipPlane(float nearPlane, float farPlane)
    {
        _nearPlane = nearPlane;
        _farPlane = farPlane;

        return this;
    }

    public CameraBuilder SetAspectRatio(float aspectRatio)
    {
        _aspectRatio = aspectRatio;

        return this;
    }

    public CameraBuilder SetActive()
    {
        _active = true;

        return this;
    }

    public Entity Build()
    {
        var cameraEntities = store.Query<CameraComponent>();
        if (_active && cameraEntities.Count > 0)
        {
            cameraEntities.ForEachEntity((ref CameraComponent camera, Entity entity) =>
            {
                camera.Active = false;
                entity.Enabled = false;
            });
        }

        var active = _active || cameraEntities.Count == 0;

        var forward = Vector3.Transform(-Vector3.UnitZ, _rotation);
        var target = _position + forward;
        var up = Vector3.Transform(Vector3.UnitY, _rotation);

        var cameraComponent = new CameraComponent
        {
            Active = active,
            FieldOfView = _fov,
            NearPlane = _nearPlane,
            FarPlane = _farPlane,
            AspectRatio = _aspectRatio,
            ViewMat = Matrix4x4.CreateLookAt(_position, target, up),
            ProjectMat = Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(_fov),
                                                                _aspectRatio,
                                                                _nearPlane,
                                                                _farPlane)
        };

        CameraManager.SetUBOData(ref cameraComponent);

        var transformComponent = _transform.IsIdentity
                                    ? new TransformComponent(_position, _rotation, Vector3.One)
                                    : new TransformComponent(_transform);

        var entity = store.CreateEntity(new EntityName(_name), cameraComponent, transformComponent);
        entity.AddTag<CameraTag>();
        entity.Enabled = active;

        return entity;
    }
}
