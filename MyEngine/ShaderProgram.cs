namespace MyEngine
{
    internal class ShaderProgram : IDisposable
    {
        private uint Id { get; }
        private IList<Shader> Shaders { get; } = [];

        public bool IsLinked { get; private set; } = false;

        public ShaderProgram()
        {
            Id = Program.GL.CreateProgram();
        }

        public bool AddShader(Shader shader)
        {
            if (!IsLinked && Shaders.FirstOrDefault(x => x.FilePath == shader.FilePath) == null)
            {
                shader.Attach(Id);

                return true;
            }

            return false;
        }

        public void Link()
        {
            Program.GL.LinkProgram(Id);

            Program.GL.GetProgram(Id, Silk.NET.OpenGL.GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                var info = Program.GL.GetProgramInfoLog(Id);
                Console.WriteLine($"Error linking Shader Program: {info}");

                Environment.Exit(0);
            }

            IsLinked = true;
            
            foreach (var shader in Shaders)
            {
                shader.Dispose(Id);
            }
        }

        public void Use()
        {
            Program.GL.UseProgram(Id);
        }

        public void Dispose()
        {
            Program.GL.DeleteProgram(Id);
        }
    }
}
