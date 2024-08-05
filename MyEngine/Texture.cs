using Silk.NET.OpenGL;
using StbImageSharp;

namespace MyEngine;

internal class Texture
{
    private uint Id { get; }
    public string FilePath { get; private set; }

    public Texture(string filePath)
    {
        FilePath = filePath;
        Id = Init();
    }

    private unsafe uint Init()
    {
        var id = Program.GL.GenTexture();

        Activate();

        var imgRes = ImageResult.FromMemory(File.ReadAllBytes(FilePath), ColorComponents.RedGreenBlueAlpha);

        fixed (byte* dataPtr = imgRes.Data)
        {
            Program.GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)imgRes.Width, (uint)imgRes.Height, 0, PixelFormat.Rgba, GLEnum.UnsignedByte, dataPtr);
        }

        Program.GL.TextureParameter(Id, GLEnum.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
        Program.GL.TextureParameter(Id, GLEnum.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);

        Program.GL.TextureParameter(Id, GLEnum.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        Program.GL.TextureParameter(Id, GLEnum.TextureMagFilter, (int)TextureMagFilter.Linear);

        Program.GL.GenerateMipmap(TextureTarget.Texture2D);

        return id;
    }

    public void Activate()
    {
        Program.GL.ActiveTexture(TextureUnit.Texture0);
        Program.GL.BindTexture(TextureTarget.Texture2D, Id);
    }
}
