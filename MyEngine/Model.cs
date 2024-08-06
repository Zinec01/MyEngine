using Silk.NET.OpenGL;

namespace MyEngine
{
    internal class Model : IDisposable
    {
        private IReadOnlyCollection<float> Vertices { get; }
        private IReadOnlyCollection<int> Indices { get; }
        private VAO VAO { get; }
        private BufferObject<float> VBO { get; }
        private BufferObject<int> EBO { get; }
        public Transform Transform { get; }
        public Texture Texture { get; }

        public event EventHandler<float> OnPermanentTransform;

        public Model(float[] vertices, int[] indices)
        {
            Vertices = vertices;
            Indices = indices;
            VAO = new VAO();
            VBO = new BufferObject<float>(vertices, BufferTargetARB.ArrayBuffer);
            EBO = new BufferObject<int>(indices, BufferTargetARB.ElementArrayBuffer);
            Transform = new Transform();

            SetupVertexAttribs();
        }

        public Model(float[] vertices, int[] indices, string texturePath) : this(vertices, indices)
        {
            Texture = new Texture(texturePath);
        }

        public static unsafe void SetupVertexAttribs()
        {
            App.GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(float) * 5, null);
            App.GL.EnableVertexAttribArray(0);

            //if (Texture != null)
            //{
                App.GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(float) * 5, (void*)(sizeof(float) * 3));
                App.GL.EnableVertexAttribArray(1);
            //}
        }

        public void Update(float deltaTime)
        {
            OnPermanentTransform?.Invoke(this, deltaTime);
            Transform.Update(deltaTime * 5);
        }

        private Texture TexGreen { get; } = new(@"..\..\..\Textures\green.png");

        public unsafe void Draw(ShaderProgram program)
        {
            VAO.Bind();

            if (Transform.TransformPending)
                program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);

            if (Texture != null)
            {
                Texture.Activate();
                program.SetUniform(Shader.TextureSampler, 0);
            }

            App.GL.DrawElements(PrimitiveType.Triangles, (uint)Indices.Count, DrawElementsType.UnsignedInt, null);


            var origScale = Transform.CurrentScale;
            TexGreen.Activate();

            Transform.SetScale(origScale * 1.01f);
            program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);
            App.GL.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

            Transform.SetScale(origScale * 1.03f);
            program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);
            App.GL.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

            Transform.SetScale(origScale * 1.05f);
            program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);
            App.GL.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

            Transform.SetScale(origScale);
        }

        public void Dispose()
        {
            VBO.Dispose();
            EBO.Dispose();
            VAO.Dispose();
        }
    }
}
