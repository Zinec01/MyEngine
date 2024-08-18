using MyEngine.EventArgs;
using MyEngine.Interfaces;
using Silk.NET.OpenGL;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MyEngine;

public class GameObject : GameObjectTransform, ITransformable, IDisposable
{
    private readonly GL _gl;
    private static uint _idCounter = 0;

    public uint Id { get; } = _idCounter++;
    public string Name { get; }

    private GameObject? _parent;
    public GameObject? Parent
    {
        get => _parent;
        set
        {
            if ((_parent = value) == null) return;

            _parent._children.Add(this);

            _parent.PositionChanged += (sender, deltaTime) => ParentObjectChanged?.Invoke(this, new ParentObjectChangedArgs((ITransformable)sender!, deltaTime, ObjectChangedFlag.POSITION));
            _parent.RotationChanged += (sender, deltaTime) => ParentObjectChanged?.Invoke(this, new ParentObjectChangedArgs((ITransformable)sender!, deltaTime, ObjectChangedFlag.ROTATION));
            _parent.ScaleChanged    += (sender, deltaTime) => ParentObjectChanged?.Invoke(this, new ParentObjectChangedArgs((ITransformable)sender!, deltaTime, ObjectChangedFlag.SCALE));
        }
    }

    internal List<GameObject> _children = [];
    private IReadOnlyCollection<GameObject> _childrenReadOnly;
    public IReadOnlyCollection<GameObject> Children => _childrenReadOnly ??= _children.AsReadOnly<GameObject>();

    private IReadOnlyCollection<float> Vertices { get; }
    private IReadOnlyCollection<int> Indices { get; }
    private VAO VAO { get; }
    private BufferObject<float> VBO { get; }
    private BufferObject<int> EBO { get; }
    public Texture Texture { get; }

    public event EventHandler<float> PermanentTransform;
    public event EventHandler<ParentObjectChangedArgs> ParentObjectChanged;

    public GameObject(GL gl, string name, float[] vertices, int[] indices, Vector3? position = null, Quaternion? rotation = null, float? scale = null)
    {
        _gl = gl;

        Name = name;
        Vertices = vertices;
        Indices = indices;
        VAO = new VAO(_gl);
        VBO = new BufferObject<float>(gl, vertices, BufferTargetARB.ArrayBuffer, BufferUsageARB.StaticDraw);
        EBO = new BufferObject<int>(gl, indices, BufferTargetARB.ElementArrayBuffer, BufferUsageARB.StaticDraw);

        PreviousPosition = CurrentPosition = TargetPosition = position ?? Vector3.Zero;
        PreviousRotation = CurrentRotation = TargetRotation = rotation ?? Quaternion.Identity;
        PreviousScale    = CurrentScale    = TargetScale    = scale    ?? 1f;

        TexGreen = new Texture(gl, @"..\..\..\..\MyEngine\Textures\green.png");

        SetupVertexAttribs();
    }

    public GameObject(GL gl, string name, float[] vertices, int[] indices, string texturePath, Vector3? position = null, Quaternion? rotation = null, float? scale = null)
        : this(gl, name, vertices, indices, position, rotation, scale)
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

    public override void Update(float deltaTime)
    {
        PermanentTransform?.Invoke(this, deltaTime);
        base.Update(deltaTime * 5);

        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }

    private Texture TexGreen { get; }

    public unsafe void Draw(ShaderProgram shaderProgram)
    {
        VAO.Bind();

        shaderProgram.SetUniform(Shader.ModelMatrix, ModelMat);

        if (Texture != null)
        {
            Texture.Activate();
            shaderProgram.SetUniform(Shader.TextureSampler, 0);
        }

        _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Count, DrawElementsType.UnsignedInt, null);
        Texture?.Deactivate();


        DrawTriangleLines(shaderProgram, TexGreen);
    }

    private unsafe void DrawTriangleLines(ShaderProgram shaderProgram, Texture lineTexture, byte layers = 10)
    {
        lineTexture.Activate();
        shaderProgram.SetUniform(Shader.TextureSampler, 0);

        for (byte i = 0; i < layers; i++)
        {
            shaderProgram.SetUniform(Shader.ModelMatrix, Matrix4x4.Identity
                                                         * Matrix4x4.CreateScale(CurrentScale * (1f + i / 500f))
                                                         * Matrix4x4.CreateFromQuaternion(CurrentRotation)
                                                         * Matrix4x4.CreateTranslation(CurrentPosition));
            _gl.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);
        }

        lineTexture.Deactivate();
    }

    public void Dispose()
    {
        VBO.Dispose();
        EBO.Dispose();
        VAO.Dispose();
        GC.SuppressFinalize(this);
    }
}
