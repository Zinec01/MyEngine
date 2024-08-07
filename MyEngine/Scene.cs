namespace MyEngine;

internal class Scene : IDisposable
{
    private static uint _idCounter = 0;
    public uint Id { get; }

    public List<Model> Objects { get; } = [];
    private ShaderProgram ShaderProgram { get; }
    public Camera Camera { get; private set; }

    public Scene()
    {
        Id = _idCounter++;

        //var shaders = new List<Shader>();
        //foreach (var file in Directory.GetFiles(@"..\..\..\Shaders"))
        //{
        //    if (File.Exists(file))
        //    {
        //        var fileType = file[(file.LastIndexOf('.') + 1)..];
        //        if (fileType is not "vert" and not "frag") continue;

        //        shaders.Add(new Shader(file, fileType == "vert" ? Shader.ShaderType.Vertex : Shader.ShaderType.Fragment));
        //    }
        //}

        Camera = new Camera();
        ShaderProgram = new ShaderProgram(@"..\..\..\Shaders\basic.vert", @"..\..\..\Shaders\basic.frag");

        // TODO: Move shader program out of scene and make them static to use freely with models

        //foreach (var shader in shaders)
        //{
        //    ShaderProgram.AddShader(shader);
        //}

        ShaderProgram.AttachShadersAndLinkProgram();

        //foreach (var model in Models.Values)
        //{
        //    model.SetupVertexAttribs();
        //}
    }

    public void UpdateObjects(float deltaTime)
    {
        foreach (var model in Objects)
        {
            model.Update(deltaTime);
        }
    }

    public void Draw()
    {
        ShaderProgram.Use();

        foreach (var model in Objects)
        {
            //Application.GL.StencilFunc(Silk.NET.OpenGL.StencilFunction.Always, kvp.Key, 0xFF);
            //Application.GL.StencilOp(Silk.NET.OpenGL.StencilOp.Keep, Silk.NET.OpenGL.StencilOp.Replace, Silk.NET.OpenGL.StencilOp.Replace);

            model.Draw(ShaderProgram);
        }
    }

    public void Dispose()
    {
        foreach (var model in Objects)
        {
            model.Dispose();
        }

        ShaderProgram.Dispose();
    }
}
