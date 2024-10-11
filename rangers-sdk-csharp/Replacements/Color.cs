using System.Runtime.InteropServices;

namespace RangersSDK.CSLib.Utility
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color8
    {
        uint rgba;

        // Doing it like this to force alignment since there is no alignas equivalent afaik.
        public byte a { get { return (byte)((rgba >>  0) & 0xFFu); } set { rgba = (rgba & 0xFFFFFF00u) | ((uint)value <<  0); } }
        public byte b { get { return (byte)((rgba >>  8) & 0xFFu); } set { rgba = (rgba & 0xFFFF00FFu) | ((uint)value <<  8); } }
        public byte g { get { return (byte)((rgba >> 16) & 0xFFu); } set { rgba = (rgba & 0xFF00FFFFu) | ((uint)value << 16); } }
        public byte r { get { return (byte)((rgba >> 24) & 0xFFu); } set { rgba = (rgba & 0x00FFFFFFu) | ((uint)value << 24); } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Colorf
    {
        public float a;
        public float b;
        public float g;
        public float r;
    }
}
