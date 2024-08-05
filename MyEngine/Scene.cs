﻿namespace MyEngine
{
    internal class Scene : IDisposable
    {
        public Dictionary<int, Model> Models = [];
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
            foreach (var model in Models.Values)
            {
                model.ExecutePermanentTransforms(deltaTime);
            }
        }

        public void Draw()
        {
            ShaderProgram.Use();

            foreach (var model in Models.Values)
            {
                if (model.Transform.TransformPending)
                    ShaderProgram.SetUniform(Shader.ModelMatrix, model.Transform.ModelMat);

                model.Draw();
            }
        }

        public bool TryAddModel(int id, Model model)
        {
            return Models.TryAdd(id, model);
        }

        public void Dispose()
        {
            foreach (var model in Models.Values)
            {
                model.Dispose();
            }

            ShaderProgram.Dispose();
        }
    }
}
