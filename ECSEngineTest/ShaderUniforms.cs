using Silk.NET.OpenGL;
using System.Collections.ObjectModel;
using System.Numerics;

namespace ECSEngineTest
{
    public static class ShaderUniforms
    {
        public const string ModelMatrix = "uModelMat";
        public const string TextureSampler = "uTex";
        public const string Color = "uColor";

        private static readonly List<UniformBlock> _uniformBlocks = [];
        public static ReadOnlyCollection<UniformBlock> UniformBlocks => _uniformBlocks.AsReadOnly();

        public static UniformBlock CameraMatricesUniformBlock => _uniformBlocks.FirstOrDefault(x => x.Name == _cameraMatricesUniformBlockName);

        public static void InitUniformBlocks()
        {
            _uniformBlocks.Add(CreateCameraUBO());
        }

        private static unsafe UniformBlock CreateCameraUBO()
        {
            uint binding = 0;
            var ubo = Window.GL.GenBuffer();

            Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, ubo);

            var size = (nuint)sizeof(Matrix4x4) * 2;
            Window.GL.BufferData(BufferTargetARB.UniformBuffer, size, null, BufferUsageARB.StaticDraw); //TODO: Experiment with dynamic draw performance
            Window.GL.BindBuffer(BufferTargetARB.UniformBuffer, 0);
            Window.GL.BindBufferRange(BufferTargetARB.UniformBuffer, binding, ubo, 0, size);

            return new UniformBlock(_cameraMatricesUniformBlockName, binding, ubo);
        }

        private const string _cameraMatricesUniformBlockName = "CameraMatrices";

        public static void Dispose()
        {
            foreach (var uniformBlock in _uniformBlocks)
            {
                Window.GL.DeleteBuffer(uniformBlock.UBO);
            }
        }
    }
}
