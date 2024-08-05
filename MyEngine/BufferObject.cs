﻿using Silk.NET.OpenGL;

namespace MyEngine
{
    internal class BufferObject<TDataType> : IDisposable where TDataType : unmanaged
    {
        private uint Id { get; }

        public unsafe BufferObject(ReadOnlySpan<TDataType> data, BufferTargetARB bufferType, BufferUsageARB bufferUsage = BufferUsageARB.StaticDraw)
        {
            Id = Program.GL.GenBuffer();
            Program.GL.BindBuffer(bufferType, Id);

            fixed (void* d = &data[0])
            {
                Program.GL.BufferData(bufferType, (nuint)(data.Length * sizeof(TDataType)), d, bufferUsage);
            }
        }

        public void Dispose()
        {
            Program.GL.DeleteBuffer(Id);
        }
    }
}
