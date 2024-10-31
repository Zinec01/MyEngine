using ECSEngineTest.Components;
using Silk.NET.OpenGL;
using System.Numerics;

namespace ECSEngineTest;

public static class MeshManager
{
    private static uint _currentVAO = 0;
    private static readonly Dictionary<int, MeshComponent> _meshes = [];

    public static unsafe MeshComponent CreateMeshComponent(Span<Vector3> vertices, Span<Vector3> normals, Span<Vector3> uvs, uint[] indices)
    {
        bool isTexture;
        if (vertices.IsEmpty || vertices.Length != normals.Length || ((isTexture = uvs != null && !uvs.IsEmpty) && vertices.Length != uvs.Length))
            throw new InvalidDataException();

        var hash = ComputeVertexHash(vertices, normals, uvs);
        if (_meshes.TryGetValue(hash, out var component)) return component;

        VertexData[]? vertexData = null;
        VertexTextureData[]? vertTexData = null;

        component = new MeshComponent(GenVAO(),
                                      isTexture ? GenVBO(vertices, normals, uvs, out vertTexData)
                                                : GenVBO(vertices, normals, out vertexData),
                                      GenEBO(indices),
                                      vertexData,
                                      vertTexData,
                                      indices,
                                      hash);

        SetupVertexAttribs(isTexture);

        _meshes.Add(hash, component);

        return component;
    }

    private static int ComputeVertexHash(Span<Vector3> vertices, Span<Vector3> normals, Span<Vector3> uvs)
    {
        var hash = 0;

        for (int i = 0; i < vertices.Length; i++)
        {
            hash = HashCode.Combine(hash, Math.Round(vertices[i].X, 2));
            hash = HashCode.Combine(hash, Math.Round(vertices[i].Y, 2));
            hash = HashCode.Combine(hash, Math.Round(vertices[i].Z, 2));

            hash = HashCode.Combine(hash, Math.Round(normals[i].X, 2));
            hash = HashCode.Combine(hash, Math.Round(normals[i].Y, 2));
            hash = HashCode.Combine(hash, Math.Round(normals[i].Z, 2));

            if (uvs.IsEmpty) continue;

            hash = HashCode.Combine(hash, Math.Round(uvs[i].X, 2));
            hash = HashCode.Combine(hash, Math.Round(uvs[i].Y, 2));
        }

        return hash;
    }

    private static uint GenVAO()
    {
        var vao = Window.GL.GenVertexArray();
        Window.GL.BindVertexArray(vao);

        return _currentVAO = vao;
    }

    private static unsafe uint GenVBO(Span<Vector3> vertices, Span<Vector3> normals, out VertexData[] data)
    {
        var vbo = Window.GL.GenBuffer();
        Window.GL.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

        data = new VertexData[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            data[i].Vertex = vertices[i];
            data[i].Normal = normals[i];
        }

        fixed (void* dataPtr = &data[0])
        {
            Window.GL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(data.Length * sizeof(VertexData)), dataPtr, BufferUsageARB.StaticDraw);
        }

        return vbo;
    }

    private static unsafe uint GenVBO(Span<Vector3> vertices, Span<Vector3> normals, Span<Vector3> uvs, out VertexTextureData[] data)
    {
        var vbo = Window.GL.GenBuffer();
        Window.GL.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

        data = new VertexTextureData[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            data[i].Vertex = vertices[i];
            data[i].Normal = normals[i];
            data[i].UVs = new Vector2(uvs[i].X, uvs[i].Y);
        }

        fixed (void* dataPtr = &data[0])
        {
            Window.GL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(data.Length * sizeof(VertexTextureData)), dataPtr, BufferUsageARB.StaticDraw);
        }

        return vbo;
    }

    private static unsafe uint GenEBO(uint[] indices)
    {
        var ebo = Window.GL.GenBuffer();
        Window.GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);

        fixed (void* dataPtr = &indices[0])
        {
            Window.GL.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(int)), dataPtr, BufferUsageARB.StaticDraw);
        }

        return ebo;
    }

    private static unsafe void SetupVertexAttribs(bool isTexture)
    {
        var stride = sizeof(float) * (uint)(isTexture ? 8 : 6);

        Window.GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        Window.GL.EnableVertexAttribArray(0);

        Window.GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, sizeof(float) * 3);
        Window.GL.EnableVertexAttribArray(1);

        if (!isTexture) return;

        Window.GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, sizeof(float) * 6);
        Window.GL.EnableVertexAttribArray(2);
    }

    public static void BindVAO(uint vao)
    {
        if (_currentVAO == vao) return;

        Window.GL.BindVertexArray(vao);
        _currentVAO = vao;
    }

    public static void DisposeMesh(MeshComponent meshComponent)
    {
        Window.GL.DeleteBuffer(meshComponent.VBO);
        Window.GL.DeleteBuffer(meshComponent.EBO);
        Window.GL.DeleteVertexArray(meshComponent.VAO);

        _meshes.Remove(_meshes.FirstOrDefault(x => x.Value.Hash == meshComponent.Hash).Key);
    }

    public static void DisposeAllMeshes()
    {
        foreach (var mesh in _meshes.Values)
        {
            Window.GL.DeleteBuffer(mesh.VBO);
            Window.GL.DeleteBuffer(mesh.EBO);
            Window.GL.DeleteVertexArray(mesh.VAO);
        }

        _meshes.Clear();
    }
}
