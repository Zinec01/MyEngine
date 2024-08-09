using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Numerics;

namespace MyEngine;

internal class Camera : CameraTransform
{
    private const float _movementSpeed = 4f;
    private const float _lookSensitivity = 4f;

    private readonly IKeyboard _keyboard;
    private readonly IMouse _mouse;

    private Vector2 _lastMousePos;

    public Camera(Vector3 position, Vector3 target, IWindow window, IInputContext inputContext) : base(position, target)
    {
        _keyboard = inputContext.Keyboards[0];
        _mouse = inputContext.Mice[0];

        window.FramebufferResize += (newSize) => ViewPort = (Vector2)newSize;
        ViewPort = (Vector2)window.FramebufferSize;

        _lastMousePos = _mouse.Position;
    }

    public Camera(IWindow window, IInputContext inputContext) : this(new Vector3(0f, 0f, 3f), new Vector3(0f, 0f, -1f), window, inputContext)
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
        HandleMouseInput(deltaTime * _lookSensitivity);

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
            moveDir -= Vector3.Normalize(Vector3.Cross(Target, Up));

        if (_keyboard.IsKeyPressed(Key.D))
            moveDir += Vector3.Normalize(Vector3.Cross(Target, Up));

        if (moveDir != Vector3.Zero)
            Move(Vector3.Normalize(moveDir) * velocity);
    }

    //TODO: make it not FPS based and make the cursor run infinitely
    private void HandleMouseInput(float velocity)
    {
        if (_mouse.IsButtonPressed(MouseButton.Left))
        {
            _mouse.Cursor.CursorMode = CursorMode.Hidden;

            //var idk = ImGuiNET.ImGui.mouse
            //Console.WriteLine(idk);

            var position = _mouse.Position;
            if (position == _lastMousePos) return;

            Yaw += (position.X - _lastMousePos.X) * velocity;
            Pitch -= (position.Y - _lastMousePos.Y) * velocity;
            if (Pitch >= 90f || Pitch <= -90f)
                Pitch = float.Clamp(Pitch, -89.9f, 89.9f);

            _lastMousePos = position;

            //SetRotation(Quaternion.CreateFromAxisAngle(Vector3.UnitY, (position.X - _lastMousePos.X) * velocity) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, (position.Y - _lastMousePos.Y) * velocity));
            Target = Vector3.Normalize(new Vector3(float.Cos(Yaw.DegToRad()) * float.Cos(Pitch.DegToRad()),
                                                   float.Sin(Pitch.DegToRad()),
                                                   float.Sin(Yaw.DegToRad()) * float.Cos(Pitch.DegToRad())));
        }
        else
        {
            _mouse.Cursor.CursorMode = CursorMode.Normal;
        }
    }
}
