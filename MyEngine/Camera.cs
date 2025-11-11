using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Numerics;

namespace MyEngine;

public class Camera : CameraTransform
{
    private const float _movementSpeed = 4f;
    private const float _lookSensitivity = 0.1f;

    private readonly List<IKeyboard> _keyboards = [];
    private IMouse? _mouse;

    private Vector2 _lastMousePos;

    public Camera(Vector3 position, Vector3 target, IWindow window)
        : base(position, target)
    {
        window.FramebufferResize += (newSize) => ViewPort = (Vector2)newSize;
        ViewPort = (Vector2)window.FramebufferSize;
    }

    public Camera(IWindow window)
        : this(new Vector3(0f, 2f, 5f), new Vector3(0f, 0f, -1f), window)
    {
    }

    public void SendShaderData(ShaderProgram shaderProgram)
    {
        shaderProgram.SetUniform(Shader.ProjectionMatrix, ProjectMat);
        shaderProgram.SetUniform(Shader.ViewMatrix, ViewMat);
        shaderProgram.SetUniform("cameraPos", CurrentPosition);
    }

    public override void Update(float deltaTime)
    {
        var velocity = deltaTime * _movementSpeed;

        foreach (var keyboard in _keyboards)
        {
            HandleKeyboardInput(keyboard, velocity);
        }

        base.Update(velocity);
    }

    private void HandleKeyboardInput(IKeyboard keyboard, float velocity)
    {
        if (keyboard.IsKeyPressed(Key.R))
        {
            SetPosition(new Vector3(0f, 2f, 5f));
            Yaw = 0f;
            Pitch = 0f;
            SetRotation(Quaternion.Identity);
            return;
        }

        if (keyboard.IsKeyPressed(Key.ShiftLeft))
            velocity *= 2f;

        var moveDir = Vector3.Zero;
        if (keyboard.IsKeyPressed(Key.W))
            moveDir += Target;

        if (keyboard.IsKeyPressed(Key.S))
            moveDir -= Target;

        if (keyboard.IsKeyPressed(Key.A))
            moveDir -= Right;

        if (keyboard.IsKeyPressed(Key.D))
            moveDir += Right;

        if (keyboard.IsKeyPressed(Key.Space))
            moveDir -= Vector3.Cross(Target, Right);

        if (keyboard.IsKeyPressed(Key.ControlLeft))
            moveDir += Vector3.Cross(Target, Right);

        if (moveDir != Vector3.Zero)
            MoveBy(Vector3.Normalize(moveDir) * velocity);
    }

    private void HandleMouseInput(object? sender, Vector2 position)
    {
        if (sender is not IMouse mouse) return;

        if (mouse.IsButtonPressed(MouseButton.Left))
        {
            mouse.Cursor.CursorMode = CursorMode.Disabled;

            if (position == _lastMousePos) return;

            var deltaX = position.X - _lastMousePos.X;
            var deltaY = position.Y - _lastMousePos.Y;

            Yaw -= (deltaX * _lookSensitivity).DegToRad();
            Pitch -= (deltaY * _lookSensitivity).DegToRad();

            _lastMousePos = position;

            SetRotation(Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, Roll));
        }
        else
        {
            mouse.Cursor.CursorMode = CursorMode.Normal;
            _lastMousePos = position;
        }
    }

    public void SubscribeToMouseMovement(IMouse mouse)
    {
        if (_mouse != null)
        {
            _mouse.MouseMove -= HandleMouseInput;
        }

        _mouse = mouse;
        _mouse.MouseMove += HandleMouseInput;
    }

    public void SubscribeToKeyboardKeyPress(IKeyboard keyboard)
    {
        _keyboards.Add(keyboard);
    }

    public void UnsubscribeFromKeyboardKeyPress(IKeyboard keyboard)
    {
        _keyboards.Remove(keyboard);
    } 
}
