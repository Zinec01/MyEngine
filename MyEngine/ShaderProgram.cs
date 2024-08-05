using Silk.NET.OpenGL;
using System.Numerics;

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
                Shaders.Add(shader);

                return true;
            }

            return false;
        }

        public void AttachShadersAndLinkProgram()
        {
            foreach (var shader in Shaders)
            {
                shader.Attach(Id);
            }

            Program.GL.LinkProgram(Id);

            Program.GL.GetProgram(Id, GLEnum.LinkStatus, out var status);
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

        public void SetUniform(string name, int value)
        {
            var location = GetUniformLocation(name);

            Program.GL.Uniform1(location, value);
        }

        public void SetUniform(string name, float value)
        {
            var location = GetUniformLocation(name);

            Program.GL.Uniform1(location, value);
        }

        public void SetUniform(string name, Vector3 value)
        {
            var location = GetUniformLocation(name);

            Program.GL.Uniform3(location, value);
        }

        public unsafe void SetUniform(string name, Matrix4x4 value)
        {
            var location = GetUniformLocation(name);

            Program.GL.UniformMatrix4(location, 1, false, (float*)&value);
        }

        private int GetUniformLocation(string name)
        {
            var location = Program.GL.GetUniformLocation(Id, name);
            if (location < 0)
            {
                Console.WriteLine($"Shader variable {name} not found");

                Environment.Exit(0);
            }

            return location;
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
