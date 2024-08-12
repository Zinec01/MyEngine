using System.Numerics;

namespace MyEngine;

internal class CameraTransform : TransformObject
{
    private float _fov = 90;
    private Vector2 _viewPort;
    private float _nearPlane = 0.1f;
    private float _farPlane = 100f;
    private Vector3 _target = Vector3.Zero;

    private Matrix4x4 _projectMat = Matrix4x4.Identity;
    private Matrix4x4 _viewMat = Matrix4x4.Identity;

    public Vector3 Target
    {
        get => _target;
        set
        {
            _target = value;
            DirectionToCamera = Vector3.Normalize(CurrentPosition - value);
            ViewTransformPending = true;
        }
    }
    public Vector3 DirectionToCamera { get; private set; }
    public Vector3 Up { get; protected set; } = new Vector3(0f, 1f, 0f);
    public Vector3 Right { get; protected set; } = new Vector3(1f, 0f, 0f);

    public float Yaw { get; protected set; } = 0f;
    public float Pitch { get; protected set; } = 0f;
    public float Roll { get; protected set; } = 0f;

    public float FOV
    {
        get => _fov;
        set
        {
            _fov = value;
            ProjectTransformPending = true;
        }
    }
    public Vector2 ViewPort
    {
        get => _viewPort;
        protected set
        {
            _viewPort = value;
            ProjectTransformPending = true;
        }
    }
    public float NearPlane
    {
        get => _nearPlane;
        set
        {
            _nearPlane = value;
            ProjectTransformPending = true;
        }
    }
    public float FarPlane
    {
        get => _farPlane;
        set
        {
            _farPlane = value;
            ProjectTransformPending = true;
        }
    }

    public bool ViewTransformPending { get; private set; } = true;
    public bool ProjectTransformPending { get; private set; } = true;

    public Matrix4x4 ProjectMat
    {
        get
        {
            if (ProjectTransformPending)
            {
                _projectMat = Matrix4x4.CreatePerspectiveFieldOfView(FOV.DegToRad(), ViewPort.X / ViewPort.Y, _nearPlane, FarPlane);

                ProjectTransformPending = false;
            }

            return _projectMat;
        }
    }
    public Matrix4x4 ViewMat
    {
        get
        {
            if (ViewTransformPending)
            {
                _viewMat = Matrix4x4.CreateLookAt(CurrentPosition, CurrentPosition + Target, Up);

                ViewTransformPending = false;
            }

            return _viewMat;
        }
    }

    public CameraTransform(Vector3 position, Vector3 target)
    {
        SetPosition(position);
        SetRotation(Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, Roll));
        Target = target;
    }

    protected override void OnCurrentToTargetPositionTransition(float deltaTime)
    {
        base.OnCurrentToTargetPositionTransition(deltaTime);
        ViewTransformPending = true;
    }

    protected override void OnCurrentToTargetRotationTransition(float deltaTime)
    {
        PreviousRotation = CurrentRotation;
        CurrentRotation = TargetRotation;

        Target = Vector3.Transform(-Vector3.UnitZ, CurrentRotation);
        Up = Vector3.Transform(Vector3.UnitY, CurrentRotation);
        Right = Vector3.Transform(Vector3.UnitX, CurrentRotation);

        ViewTransformPending = true;
    }

    public override void SetPosition(Vector3 position)
    {
        CurrentPosition = TargetPosition = position;
        ViewTransformPending = true;
    }

    public override void SetRotation(Quaternion rotation)
    {
        TargetRotation = Quaternion.Normalize(rotation);
        ViewTransformPending = true;

        //var yaw = float.Atan2(2f * (TargetRotation.Y * TargetRotation.Z + TargetRotation.W * TargetRotation.X), float.Pow(TargetRotation.W, 2f) - float.Pow(TargetRotation.X, 2f) - float.Pow(TargetRotation.Y, 2f) + float.Pow(TargetRotation.Z, 2f));
        //var pitch = float.Asin(2f * (TargetRotation.W * TargetRotation.Y - TargetRotation.X * TargetRotation.Z));
        //var roll = float.Atan2(2f * (TargetRotation.X * TargetRotation.Y + TargetRotation.W * TargetRotation.Z), float.Pow(TargetRotation.W, 2f) + float.Pow(TargetRotation.X, 2f) - float.Pow(TargetRotation.Y, 2f) - float.Pow(TargetRotation.Z, 2f));
    }
}
