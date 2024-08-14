using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace MyEngine;

internal class Scene : IDisposable
{
    private static uint _idCounter = 0;

    public uint Id { get; } = _idCounter++;

    public List<GameObject> Objects { get; } = [];
    private ShaderProgram ShaderProgram { get; }
    public Camera MainCamera { get; private set; }

    public Scene(GL gl, IWindow window, IInputContext inputContext)
    {
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

        MainCamera = new Camera(window);
        MainCamera.SubscribeToKeyboardKeyPress(inputContext.Keyboards[0]);
        MainCamera.SubscribeToMouseMovement(inputContext.Mice[0]);

        ShaderProgram = new ShaderProgram(gl, @"..\..\..\Shaders\basic.vert", @"..\..\..\Shaders\basic.frag");

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

    public void Update(float deltaTime)
    {
        foreach (var model in Objects.Where(x => x.Parent == null))
        {
            model.Update(deltaTime);
        }

        MainCamera.Update(deltaTime);
    }

    public void Draw()
    {
        ShaderProgram.Use();

        MainCamera.SendShaderData(ShaderProgram);

        foreach (var model in Objects)
        {
            //Game.GL.StencilFunc(Silk.NET.OpenGL.StencilFunction.Always, (int)model.Id, 0xFF);
            //Game.GL.StencilOp(Silk.NET.OpenGL.StencilOp.Keep, Silk.NET.OpenGL.StencilOp.Replace, Silk.NET.OpenGL.StencilOp.Replace);

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
