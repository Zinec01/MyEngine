namespace ECSEngineTest;

[Flags]
public enum SceneLoadFlags
{
    None = 0,
    Meshes = 1,
    Lights = 2,
    Cameras = 4,

    Everything = (1 << (4/*num of other enum values*/ - 1)) - 1
}
