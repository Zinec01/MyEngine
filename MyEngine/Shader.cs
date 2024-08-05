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
            var id = Program.GL.CreateShader(Type == ShaderType.Vertex ? GLEnum.VertexShader : GLEnum.FragmentShader);

            using (var sr = new StreamReader(FilePath))
            {
                Program.GL.ShaderSource(id, sr.ReadToEnd());
            }

            Program.GL.CompileShader(id);

            var info = Program.GL.GetShaderInfoLog(id);
            if (!string.IsNullOrEmpty(info))
            {
                Console.WriteLine($"Error with compiling shader at {FilePath}: {info}");

                Environment.Exit(0);
            }

            return id;
        }

        public void Attach(uint programId)
        {
            Program.GL.AttachShader(programId, Id);
            IsAttached = true;
        }

        public void Detach(uint programId)
        {
            Program.GL.DetachShader(programId, Id);
            IsAttached = false;
        }

        public enum ShaderType
        {
            Vertex,
            Fragment
        }

        public void Dispose(uint programId)
        {
            Detach(programId);
            Dispose();
        }

        public void Dispose()
        {
            Program.GL.DeleteShader(Id);
        }

        public const string ModelMatrix = "v_ModelMat";
        public const string TextureSampler = "f_Tex";
    }
}
