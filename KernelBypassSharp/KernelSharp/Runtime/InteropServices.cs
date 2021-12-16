using System;

namespace System.Runtime.InteropServices
{
    public enum CallingConvention
    {
        Winapi = 1,
        Cdecl = 2,
        StdCall = 3,
        ThisCall = 4,
        FastCall = 5,
    }

    public class UnmanagedType { }

#nullable enable
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class UnmanagedCallersOnlyAttribute : Attribute
    {
        public UnmanagedCallersOnlyAttribute()
        {
        }

        /// <summary>
        /// Optional. If omitted, the runtime will use the default platform calling convention.
        /// </summary>
        public CallingConvention CallingConvention;

        /// <summary>
        /// Optional. If omitted, no named export is emitted during compilation.
        /// </summary>
        public string? EntryPoint;
    }

#nullable disable
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class FieldOffsetAttribute : Attribute
    {
        public FieldOffsetAttribute(int offset)
        {
            Value = offset;
        }

        public int Value { get; }
    }

    sealed class StructLayoutAttribute : Attribute
    {
        public StructLayoutAttribute(LayoutKind layoutKind)
        {
        }
    }

    internal enum LayoutKind
    {
        Sequential = 0, // 0x00000008,
        Explicit = 2, // 0x00000010,
        Auto = 3, // 0x00000000,
    }

    internal enum CharSet
    {
        None = 1,       // User didn't specify how to marshal strings.
        Ansi = 2,       // Strings should be marshalled as ANSI 1 byte chars.
        Unicode = 3,    // Strings should be marshalled as Unicode 2 byte chars.
        Auto = 4,       // Marshal Strings in the right way for the target system.
    }
}
