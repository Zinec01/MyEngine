namespace MyEngine
{
    internal class VAO : IDisposable
    {
        private uint Id { get; }

        public VAO()
        {
            Id = Program.GL.GenVertexArray();
            Bind();
        }

        public void Bind()
        {
            Program.GL.BindVertexArray(Id);
        }

        public void Dispose()
        {
            Program.GL.DeleteVertexArray(Id);
        }
    }
}
