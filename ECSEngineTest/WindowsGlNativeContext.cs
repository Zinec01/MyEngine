using Silk.NET.Core.Contexts;
using Silk.NET.Core.Loader;
using System.Runtime.InteropServices;
using System.Text;

namespace ECSEngineTest;

internal class WindowsGlNativeContext : INativeContext
{
    private readonly UnmanagedLibrary _l;

    [DllImport("opengl32.dll", SetLastError = true)]
    private static unsafe extern IntPtr wglGetProcAddress(sbyte* procName);

    public WindowsGlNativeContext()
    {
        // The base library, with functions that exist in all versions
        _l = new UnmanagedLibrary("opengl32.dll");
    }

    public unsafe bool TryGetProcAddress(string proc, out nint addr, int? slot = null)
    {
        // Firstly, we try to get the function in the base library
        if (_l.TryLoadFunction(proc, out addr))
        {
            return true;
        }

        // If we fail, we assume that this is an extended function that we need to query Windows for

        // Buffer for out ASCII null-terminated string
        var asciiName = new byte[proc.Length + 1];
        Encoding.ASCII.GetBytes(proc, asciiName);

        // We ask the GC not to move the buffer
        fixed (byte* name = asciiName)
        {
            // Query Windows for the extended OpenGL function
            addr = wglGetProcAddress((sbyte*)name);

            // If the address is not null -> we succeeded
            if (addr != IntPtr.Zero)
            {
                return true;
            }
        }

        // We failed to get the function
        return false;
    }

    public nint GetProcAddress(string proc, int? slot = null)
    {
        if (TryGetProcAddress(proc, out var address, slot))
        {
            return address;
        }

        throw new InvalidOperationException("No function was found with the name " + proc + ".");
    }

    public void Dispose() => _l.Dispose();
}
