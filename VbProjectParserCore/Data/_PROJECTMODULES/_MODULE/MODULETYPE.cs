using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTMODULES._MODULE;

/// <summary>
/// Page 49
/// </summary>
public class MODULETYPE : DataBase
{
    /// <summary>
    /// An unsigned integer that specifies the identifier for this record. MUST be 0x0021 when the containing MODULE Record (section 2.3.4.2.3.2) is a procedural module. MUST be 0x0022 when the containing MODULE Record (section 2.3.4.2.3.2) is a document module, class module, or designer module.
    /// </summary>
    [AutoRead(1)]
    [MustBe((ushort)0x0021, (ushort)0x0022)]
    public readonly ushort Id;

    [AutoRead(2)]
    [MustBe((uint)0x00000000)]
    public readonly uint Size;

    public MODULETYPE(XlBinaryReader Data)
        : base(Data)
    {
        Validate();
    }
}
