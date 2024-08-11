using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Numerics;

namespace MyEngine;

internal class Camera : CameraTransform
{
    private const float _movementSpeed = 4f;
    private const float _lookSensitivity = 0.002f;

    private readonly IKeyboard _keyboard;
    private readonly IMouse _mouse;

    private Vector2 _lastMousePos;

    public Camera(Vector3 position, Vector3 target, IWindow window, IInputContext inputContext)
        : base(position, target)
    {
        _keyboard = inputContext.Keyboards[0];
        _mouse = inputContext.Mice[0];

        _mouse.MouseMove += (sender, newPos) => HandleMouseInput(newPos);
        window.FramebufferResize += (newSize) => ViewPort = (Vector2)newSize;

        ViewPort = (Vector2)window.FramebufferSize;
    }

    public Camera(IWindow window, IInputContext inputContext)
        : this(new Vector3(0f, 2f, 3f), new Vector3(0f, 0f, -1f), window, inputContext)
    {
    }

    public void ApplyChanges(ShaderProgram shaderProgram)
    {
        if (ProjectTransformPending)
            shaderProgram.SetUniform(Shader.ProjectionMatrix, ProjectMat);
        if (ViewTransformPending)
            shaderProgram.SetUniform(Shader.ViewMatrix, ViewMat);
    }

    public override void Update(float deltaTime)
    {
        var velocity = deltaTime * _movementSpeed;

        HandleKeyboardInput(velocity);

        base.Update(velocity);
    }

    private void HandleKeyboardInput(float velocity)
    {
        if (_keyboard.IsKeyPressed(Key.ShiftLeft))
            velocity *= 2f;

        var moveDir = Vector3.Zero;
        if (_keyboard.IsKeyPressed(Key.W))
            moveDir += Target;

        if (_keyboard.IsKeyPressed(Key.S))
            moveDir -= Target;

        if (_keyboard.IsKeyPressed(Key.A))
            moveDir -= Right;

        if (_keyboard.IsKeyPressed(Key.D))
            moveDir += Right;

        if (_keyboard.IsKeyPressed(Key.Space))
            moveDir -= Vector3.Cross(Target, Right);

        if (_keyboard.IsKeyPressed(Key.ControlLeft))
            moveDir += Vector3.Cross(Target, Right);

        if (moveDir != Vector3.Zero)
            MoveBy(Vector3.Normalize(moveDir) * velocity);
    }

    private void HandleMouseInput(Vector2 position)
    {
        //var position = _mouse.Position;
        if (_mouse.IsButtonPressed(MouseButton.Left))
        {
            _mouse.Cursor.CursorMode = CursorMode.Disabled;

            if (position == _lastMousePos) return;

            var deltaX = position.X - _lastMousePos.X;
            var deltaY = position.Y - _lastMousePos.Y;

            Yaw -= deltaX * _lookSensitivity;
            Pitch -= deltaY * _lookSensitivity;
            //if (Pitch >= 90f || Pitch <= -90f)
            //    Pitch = float.Clamp(Pitch, -89f, 89f);

            _lastMousePos = position;

            SetRotation(Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, Roll));
        }
        else
        {
            _mouse.Cursor.CursorMode = CursorMode.Normal;
            _lastMousePos = position;
        }
    }
}
