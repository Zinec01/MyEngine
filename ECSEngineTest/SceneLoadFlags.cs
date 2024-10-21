namespace ECSEngineTest;

[Flags]
public enum SceneLoadFlags
{
    None = 0,
    Meshes = 1,
    Lights = 2,
    WorldTransforms = 4,
    Cameras = 8,

    Everything = (1 << (5/*num of other enum values*/ - 1)) - 1
}
