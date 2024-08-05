﻿using Silk.NET.OpenGL;

namespace MyEngine
{
    internal class Model : IDisposable
    {
        private IReadOnlyCollection<float> Vertices { get; }
        private VAO VAO { get; }
        private BufferObject<float> VBO { get; }
        public Transform Transform { get; }

        public event EventHandler<float> OnPermanentTransform;

        public Model(float[] vertices)
        {
            Vertices = vertices;
            VAO = new VAO();
            VBO = new BufferObject<float>(vertices, BufferTargetARB.ArrayBuffer);
            Transform = new Transform();

            SetupVertexAttribs();
        }

        public unsafe void SetupVertexAttribs()
        {
            Program.GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, null);
            Program.GL.EnableVertexAttribArray(0);
        }

        public void Update(float deltaTime)
        {
            OnPermanentTransform?.Invoke(this, deltaTime);
            Transform.Update(deltaTime * 5);
        }

        public void Draw()
        {
            VAO.Bind();
            Program.GL.DrawArrays(GLEnum.Triangles, 0, (uint)Vertices.Count / 3);
        }

        public void Dispose()
        {
            VAO.Dispose();
            VBO.Dispose();
        }
    }
}
