using ECSEngineTest.Components;
using Silk.NET.OpenGL;
using System.Numerics;

namespace ECSEngineTest;

public class CameraManager
{
    internal static CameraComponent CreateCameraComponent()
    {
        return new CameraComponent(GenUBO());
    }

    private static unsafe uint GenUBO()
    {
        var ubo = Window.GL.GenBuffer();

        Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, ubo);

        var size = (nuint)sizeof(Matrix4x4) * 2;
        Window.GL.BufferData(BufferTargetARB.UniformBuffer, size, null, BufferUsageARB.StaticDraw);
        Window.GL.BindBufferRange(BufferTargetARB.UniformBuffer, 0, ubo, 0, size);

        return ubo;
    }

    public static unsafe void BindAndSetUBO(ref CameraComponent camera)
    {
        Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, camera.UBO);

        var data = new[] { camera.ProjectMat, camera.ViewMat };
        var size = (nuint)sizeof(Matrix4x4) * 2;

        fixed (void* dataPtr = &data[0])
        {
            Window.GL.BufferSubData(BufferTargetARB.UniformBuffer, 0, size, dataPtr);
        }

        Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, 0);
    }
}
