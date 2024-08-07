namespace MyEngine;

internal class VAO : IDisposable
{
    private uint Id { get; }

    public VAO()
    {
        Id = Game.GL.GenVertexArray();
        Bind();
    }

    public void Bind()
    {
        Game.GL.BindVertexArray(Id);
    }

    public void Dispose()
    {
        Game.GL.DeleteVertexArray(Id);
    }
}
