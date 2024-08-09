using System.Numerics;

namespace MyEngine;

internal class CameraTransform
{
    private float _fov = 90;
    private Vector2 _viewPort;
    private float _nearPlane = 0.1f;
    private float _farPlane = 100f;
    private Vector3 _target = Vector3.Zero;

    private Matrix4x4 _projectMat = Matrix4x4.Identity;
    private Matrix4x4 _viewMat = Matrix4x4.Identity;

    public event EventHandler OnPositionChanged;
    public event EventHandler OnRotationChanged;

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
                var right = Vector3.Normalize(Vector3.Cross(new Vector3(0f, 1f, 0f), DirectionToCamera));
                Up = Vector3.Cross(DirectionToCamera, right);

                _viewMat = Matrix4x4.CreateLookAt(CurrentPosition, CurrentPosition + Target, Up);

                ViewTransformPending = false;
            }

            return _viewMat;
        }
    }

    public Vector3 PreviousPosition { get; protected set; }
    public Vector3 TargetPosition { get; protected set; }
    public Vector3 CurrentPosition { get; protected set; }

    public float Yaw { get; protected set; } = -90f;
    public float Pitch { get; protected set; } = 0f;

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

    public CameraTransform(Vector3 position, Vector3 target)
    {
        PreviousPosition = CurrentPosition = TargetPosition = position;
        Target = target;
    }

    public virtual void Update(float deltaTime)
    {
        if (CurrentPosition != TargetPosition)
        {
            PreviousPosition = CurrentPosition;
            CurrentPosition = Vector3.Lerp(CurrentPosition, TargetPosition, deltaTime);

            OnPositionChanged?.Invoke(this, System.EventArgs.Empty);
            ViewTransformPending = true;
        }
    }

    public virtual void Move(Vector3 position)
    {
        TargetPosition += position;
    }

    public void SetPosition(Vector3 position)
    {
        CurrentPosition = TargetPosition = position;
        ViewTransformPending = true;
    }
}
