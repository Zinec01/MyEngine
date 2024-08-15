using Silk.NET.OpenGL;
using StbImageSharp;

namespace MyEngine;

public class Texture
{
    private readonly GL _gl;

    private uint Id { get; }
    public string FilePath { get; private set; }

    public Texture(GL gl, string filePath)
    {
        _gl = gl;

        FilePath = filePath;
        Id = Init();
    }

    private unsafe uint Init()
    {
        var id = _gl.GenTexture();

        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, id);

        var imgRes = ImageResult.FromMemory(File.ReadAllBytes(FilePath), ColorComponents.RedGreenBlueAlpha);

        fixed (byte* dataPtr = imgRes.Data)
        {
            _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)imgRes.Width, (uint)imgRes.Height, 0, PixelFormat.Rgba, GLEnum.UnsignedByte, dataPtr);
        }

        _gl.TextureParameter(Id, GLEnum.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
        _gl.TextureParameter(Id, GLEnum.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);

        _gl.TextureParameter(Id, GLEnum.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        _gl.TextureParameter(Id, GLEnum.TextureMagFilter, (int)TextureMagFilter.Linear);

        _gl.GenerateMipmap(TextureTarget.Texture2D);
        
        return id;
    }

    public void Activate()
    {
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, Id);
    }
}
