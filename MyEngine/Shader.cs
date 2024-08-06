using Silk.NET.OpenGL;

namespace MyEngine
{
    internal class Shader : IDisposable
    {
        private uint Id { get; }
        public string FilePath { get; }
        public ShaderType Type { get; }

        public bool IsAttached { get; private set; }

        public Shader(string filePath, ShaderType type)
        {
            FilePath = filePath;
            Type = type;
            Id = Init();
        }

        private uint Init()
        {
            var id = App.GL.CreateShader(Type == ShaderType.Vertex ? GLEnum.VertexShader : GLEnum.FragmentShader);

            using (var sr = new StreamReader(FilePath))
            {
                App.GL.ShaderSource(id, sr.ReadToEnd());
            }

            App.GL.CompileShader(id);

            var info = App.GL.GetShaderInfoLog(id);
            if (!string.IsNullOrEmpty(info))
            {
                Console.WriteLine($"Error with compiling shader at {FilePath}: {info}");

                Environment.Exit(0);
            }

            return id;
        }

        public void Attach(uint programId)
        {
            App.GL.AttachShader(programId, Id);
            IsAttached = true;
        }

        public void Detach(uint programId)
        {
            App.GL.DetachShader(programId, Id);
            IsAttached = false;
        }

        public enum ShaderType
        {
            Vertex,
            Fragment,
            Compute
        }

        public void Dispose(uint programId)
        {
            Detach(programId);
            Dispose();
        }

        public void Dispose()
        {
            App.GL.DeleteShader(Id);
        }

        public const string ModelMatrix = "v_uModelMat";
        public const string TextureSampler = "f_uTex";
    }
}
