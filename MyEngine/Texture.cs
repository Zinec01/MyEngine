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
        var id = Game.GL.GenTexture();

        Game.GL.ActiveTexture(TextureUnit.Texture0);
        Game.GL.BindTexture(TextureTarget.Texture2D, id);

        var imgRes = ImageResult.FromMemory(File.ReadAllBytes(FilePath), ColorComponents.RedGreenBlueAlpha);

        fixed (byte* dataPtr = imgRes.Data)
        {
            Game.GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)imgRes.Width, (uint)imgRes.Height, 0, PixelFormat.Rgba, GLEnum.UnsignedByte, dataPtr);
        }

        Game.GL.TextureParameter(Id, GLEnum.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
        Game.GL.TextureParameter(Id, GLEnum.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);

        Game.GL.TextureParameter(Id, GLEnum.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        Game.GL.TextureParameter(Id, GLEnum.TextureMagFilter, (int)TextureMagFilter.Linear);

        Game.GL.GenerateMipmap(TextureTarget.Texture2D);
        
        return id;
    }

    public void Activate()
    {
        Game.GL.ActiveTexture(TextureUnit.Texture0);
        Game.GL.BindTexture(TextureTarget.Texture2D, Id);
    }
}
