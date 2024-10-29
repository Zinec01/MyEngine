using ECSEngineTest.Components;
using Silk.NET.OpenGL;
using System.Numerics;

namespace ECSEngineTest;

internal class CameraManager
{
    public static unsafe void SetUBOData(ref CameraComponent camera)
    {
        Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, ShaderUniforms.CameraMatricesUniformBlock.UBO);

        var size = sizeof(Matrix4x4);
        Window.GL.BufferSubData(BufferTargetARB.UniformBuffer, 0, (nuint)size, ref camera.ViewMat);
        Window.GL.BufferSubData(BufferTargetARB.UniformBuffer, size, (nuint)size, ref camera.ProjectMat);

        Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, 0);
    }

    public static unsafe void SetUBOViewMat(ref CameraComponent camera)
    {
        Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, ShaderUniforms.CameraMatricesUniformBlock.UBO);

        var size = sizeof(Matrix4x4);
        Window.GL.BufferSubData(BufferTargetARB.UniformBuffer, 0, (nuint)size, ref camera.ViewMat);

        Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, 0);
    }

    public static unsafe void SetUBOProjectMat(ref CameraComponent camera)
    {
        Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, ShaderUniforms.CameraMatricesUniformBlock.UBO);

        var size = sizeof(Matrix4x4);
        Window.GL.BufferSubData(BufferTargetARB.UniformBuffer, size, (nuint)size, ref camera.ProjectMat);

        Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, 0);
    }
}
