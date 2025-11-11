namespace ECSEngineTest;

[Flags]
public enum EventTypeFlags
{
    None                         = 0b_0000_0000_0000_0000,
                           
    MouseMove                    = 0b_0000_0000_0000_0001,
    MouseScroll                  = 0b_0000_0000_0000_0010,
    MouseUp                      = 0b_0000_0000_0000_0100,
    MouseDown                    = 0b_0000_0000_0000_1000,
    MouseClick                   = 0b_0000_0000_0001_0000,
    MouseDoubleClick             = 0b_0000_0000_0010_0000,
    MouseEvent                   = 0b_0000_0000_0011_1111,
                           
    KeyUp                        = 0b_0000_0000_0100_0000,
    KeyDown                      = 0b_0000_0000_1000_0000,
    KeyChar                      = 0b_0000_0001_0000_0000,
    KeyboardEvent                = 0b_0000_0001_1100_0000,

    InputDeviceConnectionChanged = 0b_0000_0010_0000_0000,
                           
    WindowLoaded                 = 0b_0000_0100_0000_0000,
    WindowResized                = 0b_0000_1000_0000_0000,
    WindowFileDrop               = 0b_0001_0000_0000_0000,
    WindowClosing                = 0b_0010_0000_0000_0000,
    WindowEvent                  = 0b_0011_1100_0000_0000,
}
