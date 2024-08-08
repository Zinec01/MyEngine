using MyEngine.Interfaces;
using Silk.NET.OpenGL;
using System.Numerics;

namespace MyEngine;

internal class Model : IDisposable, IMovable
{
    private readonly GL _gl;
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

    public Model(GL gl, float[] vertices, int[] indices)
    {
        _gl = gl;
        Id = _idCounter++;

        Vertices = vertices;
        Indices = indices;
        VAO = new VAO(_gl);
        VBO = new BufferObject<float>(gl, vertices, BufferTargetARB.ArrayBuffer, BufferUsageARB.StaticDraw);
        EBO = new BufferObject<int>(gl, indices, BufferTargetARB.ElementArrayBuffer, BufferUsageARB.StaticDraw);
        Transform = new Transform();
        TexGreen = new Texture(gl, @"..\..\..\Textures\green.png");

        SetupVertexAttribs();
    }

    public Model(float[] vertices, int[] indices, string texturePath, GL gl) : this(gl, vertices, indices)
    {
        Texture = new Texture(gl, texturePath);
    }

    public unsafe void SetupVertexAttribs()
    {
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(float) * 5, null);
        _gl.EnableVertexAttribArray(0);

        //if (Texture != null)
        //{
            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(float) * 5, (void*)(sizeof(float) * 3));
            _gl.EnableVertexAttribArray(1);
        //}
    }

    public void Update(float deltaTime)
    {
        PermanentTransform?.Invoke(this, deltaTime);
        Transform.Update(deltaTime * 5);
    }

    private Texture TexGreen { get; }

    public Vector3 Position => Transform.CurrentPosition;
    public Quaternion Rotation => Transform.CurrentRotation;

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

        _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Count, DrawElementsType.UnsignedInt, null);


        //Green triangle edge lines
        var origScale = Transform.CurrentScale;
        TexGreen.Activate();

        Transform.SetScale(origScale * 1.01f);
        program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);
        _gl.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

        Transform.SetScale(origScale * 1.03f);
        program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);
        _gl.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

        Transform.SetScale(origScale * 1.05f);
        program.SetUniform(Shader.ModelMatrix, Transform.ModelMat);
        _gl.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

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

    public void SubscribeTo(IMovable @object, Action<IMovable, ObjectChangedFlag> updateAction)
    {
        @object.Moved += (sender, flags) =>
        {
            if (sender is IMovable obj)
                updateAction?.Invoke(obj, flags);
        };
    }
}
