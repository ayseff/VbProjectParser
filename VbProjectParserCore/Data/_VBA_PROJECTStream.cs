using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data;

/// <summary>
/// Section 2.3.4.1, page 29
/// 
/// The _VBA_PROJECT stream contains the version-dependent description of a VBA project.
/// The first seven bytes of the stream are version-independent and therefore can be read by any version.
/// </summary>
public class _VBA_PROJECTStream : DataBase
{
    /// <summary>
    /// Reserved1 (2 bytes): MUST be 0x61CC. MUST be ignored.
    /// </summary>
    [MustBe((ushort)0x61CC)]
    public ushort Reserved1 { get; private set; }

    /// <summary>
    /// Version (2 bytes): An unsigned integer that specifies the version of VBA used to create the VBA project. MUST be ignored on read. MUST be 0xFFFF on write.
    /// </summary>
    public ushort Version { get; private set; }

    /// <summary>
    /// Reserved2 (1 byte): MUST be 0x00. MUST be ignored.
    /// </summary>
    [MustBe((byte)0x00)]
    public byte Reserved2 { get; private set; }

    /// <summary>
    /// Reserved3 (2 bytes): Undefined. MUST be ignored.
    /// </summary>
    public ushort Reserved3 { get; set; }

    public byte[] PerformanceCache { get; private set; }

    public _VBA_PROJECTStream(XlBinaryReader data)
    {
        Reserved1 = data.ReadUInt16();
        Version = data.ReadUInt16();
        Reserved2 = data.ReadByte();
        Reserved3 = data.ReadUInt16();

        int PerformanceCacheLength = data.Length - 7;
        PerformanceCache = data.ReadBytes(PerformanceCacheLength);

        Validate();
    }

    internal _VBA_PROJECTStream()
    {
        Reserved1 = 0x61CC;
        Version = 0xFFFF;
        Reserved2 = 0x00;
        Reserved3 = 0x0000; // undefined. we'll just zero it out.
        PerformanceCache = new byte[] { };
    }
}
