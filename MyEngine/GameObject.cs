using MyEngine.Interfaces;
using Silk.NET.OpenGL;

namespace MyEngine;

internal class GameObject : GameObjectTransform, ITransformable, IDisposable
{
    private readonly GL _gl;
    private static uint _idCounter = 0;

    public uint Id { get; } = _idCounter++;

    private GameObject _parent;
    public GameObject Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            _parent._children.Add(this);

            _parent.PositionChanged += (sender, args) => ParentObjectChanged?.Invoke(sender, ObjectChangedFlag.POSITION);
            _parent.RotationChanged += (sender, args) => ParentObjectChanged?.Invoke(sender, ObjectChangedFlag.ROTATION);
            _parent.ScaleChanged += (sender, args) => ParentObjectChanged?.Invoke(sender, ObjectChangedFlag.SCALE);
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
    public event EventHandler<ObjectChangedFlag> ParentObjectChanged;

    public GameObject(GL gl, float[] vertices, int[] indices)
    {
        _gl = gl;

        Vertices = vertices;
        Indices = indices;
        VAO = new VAO(_gl);
        VBO = new BufferObject<float>(gl, vertices, BufferTargetARB.ArrayBuffer, BufferUsageARB.StaticDraw);
        EBO = new BufferObject<int>(gl, indices, BufferTargetARB.ElementArrayBuffer, BufferUsageARB.StaticDraw);

        TexGreen = new Texture(gl, @"..\..\..\Textures\green.png");

        SetupVertexAttribs();
    }

    public GameObject(GL gl, float[] vertices, int[] indices, string texturePath) : this(gl, vertices, indices)
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
    }

    private Texture TexGreen { get; }

    public unsafe void Draw(ShaderProgram shaderProgram)
    {
        VAO.Bind();

        if (ModelTransformPending)
            shaderProgram.SetUniform(Shader.ModelMatrix, ModelMat);

        if (Texture != null)
        {
            Texture.Activate();
            shaderProgram.SetUniform(Shader.TextureSampler, 0);
        }

        _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Count, DrawElementsType.UnsignedInt, null);


        //Green triangle edge lines
        var origScale = CurrentScale;
        TexGreen.Activate();

        SetScale(origScale * 1.01f);
        shaderProgram.SetUniform(Shader.ModelMatrix, ModelMat);
        _gl.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

        SetScale(origScale * 1.03f);
        shaderProgram.SetUniform(Shader.ModelMatrix, ModelMat);
        _gl.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

        SetScale(origScale * 1.05f);
        shaderProgram.SetUniform(Shader.ModelMatrix, ModelMat);
        _gl.DrawElements(PrimitiveType.LineStrip, (uint)Indices.Count + 1, DrawElementsType.UnsignedInt, null);

        SetScale(origScale);
    }

    public void Dispose()
    {
        VBO.Dispose();
        EBO.Dispose();
        VAO.Dispose();
    }

    //public static void SubscribeTo(ITransformable @object, Action<ITransformable, ObjectChangedFlag> updateAction)
    //{
    //    @object.Moved += (sender, flags) =>
    //    {
    //        if (sender is ITransformable obj)
    //            updateAction?.Invoke(obj, flags);
    //    };
    //}
}
