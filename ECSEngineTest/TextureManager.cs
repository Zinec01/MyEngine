﻿using ECSEngineTest.Components;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace ECSEngineTest;

public static class TextureManager
{
    private static readonly List<TextureComponent> _textures = [];

    public static TextureComponent GetTexture(string texturePath)
    {
        return IsLoaded(texturePath)
                ? _textures.Single(x => x.FilePath == texturePath)
                : LoadTexture(texturePath);
    }

    public static TextureComponent GetActiveTexture(string texturePath)
    {
        var texture = GetTexture(texturePath);

        ActivateTexture(texture.Id);

        return texture;
    }

    public static bool IsLoaded(string texturePath)
    {
        return _textures.Any(x => x.FilePath == texturePath);
    }

    internal static unsafe TextureComponent LoadTexture(string texturePath, byte[]? data = null)
    {
        if (IsLoaded(texturePath))
            return GetTexture(texturePath);

        var id = Window.GL.GenTexture();

        Window.GL.BindTexture(TextureTarget.Texture2D, id);

        var imgRes = ImageResult.FromMemory(data != null && data.Length > 0 ? data : File.ReadAllBytes(texturePath), ColorComponents.RedGreenBlueAlpha);

        fixed (byte* dataPtr = imgRes.Data)
        {
            Window.GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)imgRes.Width, (uint)imgRes.Height, 0, PixelFormat.Rgba, GLEnum.UnsignedByte, dataPtr);
        }

        Window.GL.TextureParameter(id, GLEnum.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
        Window.GL.TextureParameter(id, GLEnum.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);

        Window.GL.TextureParameter(id, GLEnum.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        Window.GL.TextureParameter(id, GLEnum.TextureMagFilter, (int)TextureMagFilter.Linear);

        Window.GL.GenerateMipmap(TextureTarget.Texture2D);

        var component = new TextureComponent(id, texturePath);

        _textures.Add(component);

        return component;
    }

    public static void ActivateTexture(uint textureId)
    {
        Window.GL.ActiveTexture(GLEnum.Texture0);
        Window.GL.BindTexture(GLEnum.Texture2D, textureId);
    }
}