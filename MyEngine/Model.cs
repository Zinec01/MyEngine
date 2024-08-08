using MyEngine.Interfaces;
using Silk.NET.OpenGL;
using System.Numerics;

namespace MyEngine;

internal class Model : IDisposable, IMovable
{
    private static uint _idCounter = 0;
    public uint Id { get; }

    private IReadOnlyCollection<float> Vertices { get; }
    private IReadOnlyCollection<int> Indices { get; }
    private VAO VAO { get; }
    private BufferObject<float> VBO { get; }
    private BufferObject<int> EBO { get; }
    private Transform Transform { get; }
    public Texture Texture { get; }

    public event EventHandler<float> PermanentTransform;
    public event EventHandler<ObjectChangedFlag> Moved;

    public Model(float[] vertices, int[] indices)
    {
        Id = _idCounter++;

        Vertices = vertices;
        Indices = indices;
        VAO = new VAO();
        VBO = new BufferObject<float>(vertices, BufferTargetARB.ArrayBuffer);
        EBO = new BufferObject<int>(indices, BufferTargetARB.ElementArrayBuffer);
        Transform = new Transform();

        SetupVertexAttribs();
    }

    public Model(float[] vertices, int[] indices, string texturePath) : this(vertices, indices)
    {
        Texture = new Texture(texturePath);
    }

    public static unsafe void SetupVertexAttribs()
    {
        Game.GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(float) * 5, null);
        Game.GL.EnableVertexAttribArray(0);

        //if (Texture != null)
        //{
            Game.GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(float) * 5, (void*)(sizeof(float) * 3));
            Game.GL.EnableVertexAttribArray(1);
        //}
    }

    public void Update(float deltaTime)
    {
        PermanentTransform?.Invoke(this, deltaTime);
        Transform.Update(deltaTime * 5);
    }

    private Texture TexGreen { get; } = new(@"..\..\..\Textures\green.png");

    public Vector3 Position => Transform.CurrentPosition;

    public unsafe void Draw(ShaderProgram program)
    {
        VAO.Bind();

        if (Transform.TransformPending)
            program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);

        if (Texture != null)
        {
            Texture.Activate();
            program.SetUniform(Shader.TextureSampler, 0);
        }

        Game.GL.DrawElements(PrimitiveType.Triangles, (uint)Indices.Count, DrawElementsType.UnsignedInt, null);


        //Green triangle edge lines
        var origScale = Transform.CurrentScale;
        TexGreen.Activate();

        Transform.SetScale(origScale * 1.01f);
        program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);
        Game.GL.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

        Transform.SetScale(origScale * 1.03f);
        program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);
        Game.GL.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

        Transform.SetScale(origScale * 1.05f);
        program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);
        Game.GL.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

        Transform.SetScale(origScale);
    }

    public void Move(Vector3 position) => Transform.Move(position);
    public void SetPosition(Vector3 position) => Transform.SetPosition(position);
    public void ChangeScale(float scale) => Transform.ChangeScale(scale);
    public void SetScale(float scale) => Transform.SetScale(scale);
    public void Rotate(Quaternion rotation) => Transform.Rotate(rotation);
    public void SetRotation(Quaternion rotation) => Transform.SetRotation(rotation);

    public void Dispose()
    {
        VBO.Dispose();
        EBO.Dispose();
        VAO.Dispose();
    }

    public void SubscribeTo(IMovable @object)
    {
        @object.Moved += OnSubscribedObjectMoved;
    }

    private void OnSubscribedObjectMoved(object? sender, ObjectChangedFlag e)
    {

    }
}
