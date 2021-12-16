namespace System.Runtime.CompilerServices
{
    public class RuntimeHelpers
    {
        public static unsafe int OffsetToStringData => sizeof(IntPtr) + sizeof(int);
    }

    public sealed class ExtensionAttribute : Attribute { }

    public enum MethodImplOptions
    {
        Unmanaged = 0x0004,
        NoInlining = 0x0008,
        NoOptimization = 0x0040,
        AggressiveInlining = 0x0100,
        AggressiveOptimization = 0x200,
        InternalCall = 0x1000,
    }

    //Implementing the MethodImpl attribute for RuntimeExport to work
    public sealed class MethodImplAttribute : Attribute
    {
        public MethodImplAttribute(MethodImplOptions opt) { }
    }
}

