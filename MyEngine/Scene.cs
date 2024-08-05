namespace MyEngine
{
    internal class Scene : IDisposable
    {
        public Dictionary<int, Model> Objects = [];
        private ShaderProgram ShaderProgram { get; }
        public Camera Camera { get; private set; }

        public Scene()
        {
            var shaders = new List<Shader>();
            foreach (var file in Directory.GetFiles(@"..\..\..\Shaders"))
            {
                if (File.Exists(file))
                {
                    var fileType = file[(file.LastIndexOf('.') + 1)..];
                    if (fileType is not "vert" and not "frag") continue;

                    shaders.Add(new Shader(file, fileType == "vert" ? Shader.ShaderType.Vertex : Shader.ShaderType.Fragment));
                }
            }

            Camera = new Camera();
            ShaderProgram = new ShaderProgram();

            foreach (var shader in shaders)
            {
                ShaderProgram.AddShader(shader);
            }

            ShaderProgram.AttachShadersAndLinkProgram();

            //foreach (var model in Models.Values)
            //{
            //    model.SetupVertexAttribs();
            //}
        }

        public void UpdateObjects(double deltaTime)
        {
            foreach (var model in Objects.Values)
            {
                model.Update((float)deltaTime);
            }
        }

        public void Draw()
        {
            ShaderProgram.Use();

            foreach (var kvp in Objects)
            {
                var model = kvp.Value;

                if (model.Transform.TransformPending)
                    ShaderProgram.SetUniform(Shader.ModelMatrix, model.Transform.ModelMat);

                if (model.Texture != null)
                    ShaderProgram.SetUniform(Shader.TextureSampler, 0);

                Program.GL.StencilFunc(Silk.NET.OpenGL.StencilFunction.Always, kvp.Key, 0xFF);
                Program.GL.StencilOp(Silk.NET.OpenGL.StencilOp.Keep, Silk.NET.OpenGL.StencilOp.Replace, Silk.NET.OpenGL.StencilOp.Replace);

                model.Draw();
            }
        }

        public bool TryAddModel(int id, Model model)
        {
            return Objects.TryAdd(id, model);
        }

        public void Dispose()
        {
            foreach (var model in Objects.Values)
            {
                model.Dispose();
            }

            ShaderProgram.Dispose();
        }
    }
}
