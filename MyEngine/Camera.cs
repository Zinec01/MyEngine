using Silk.NET.Input;
using System.Numerics;

namespace MyEngine;

internal class Camera : CameraTransform
{
    private const float _movementSpeed = 4f;
    private const float _lookSensitivity = 0.1f;

    private readonly IInputContext _inputContext;
    private readonly IKeyboard _keyboard;
    private readonly IMouse _mouse;

    private Vector2 _lastMousePos;

    public Camera(Vector3 position, Vector3 target, Vector2 viewPort, IInputContext inputContext) : base(position, target, viewPort)
    {
        _inputContext = inputContext;

        if (_inputContext.Keyboards.Count == 0)
            throw new Exception("Bro just get a keyboard ???");
        if (_inputContext.Mice.Count == 0)
            throw new Exception("Bro just get a mouse ???");

        _keyboard = inputContext.Keyboards[0];
        _mouse = inputContext.Mice[0];

        _lastMousePos = _mouse.Position;
    }

    public Camera(Vector2 viewPort, IInputContext inputContext) : this(new Vector3(0f, 0f, 3f), new Vector3(0f, 0f, -1f), viewPort, inputContext)
    {
    }

    public override void Update(float deltaTime)
    {
        var velocity = deltaTime * _movementSpeed;

        HandleKeyboardInput(velocity);

        HandleMouseInput(velocity);

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

    private void HandleMouseInput(float velocity)
    {

    }
}
