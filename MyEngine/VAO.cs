namespace MyEngine
{
    internal class VAO : IDisposable
    {
        private uint Id { get; }

        public VAO()
        {
            Id = App.GL.GenVertexArray();
            Bind();
        }

        public void Bind()
        {
            App.GL.BindVertexArray(Id);
        }

        public void Dispose()
        {
            App.GL.DeleteVertexArray(Id);
        }
    }
}
