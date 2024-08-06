using Silk.NET.OpenGL;

namespace MyEngine
{
    internal class BufferObject<TDataType> : IDisposable where TDataType : unmanaged
    {
        private uint Id { get; }

        public unsafe BufferObject(ReadOnlySpan<TDataType> data, BufferTargetARB bufferType, BufferUsageARB bufferUsage = BufferUsageARB.StaticDraw)
        {
            Id = App.GL.GenBuffer();
            App.GL.BindBuffer(bufferType, Id);

            fixed (void* dataPtr = &data[0])
            {
                App.GL.BufferData(bufferType, (nuint)(data.Length * sizeof(TDataType)), dataPtr, bufferUsage);
            }
        }

        public void Dispose()
        {
            App.GL.DeleteBuffer(Id);
        }
    }
}
