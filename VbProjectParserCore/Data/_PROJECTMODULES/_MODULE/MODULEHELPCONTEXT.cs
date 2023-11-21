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
/// Page 48
/// </summary>
public class MODULEHELPCONTEXT : DataBase
{
    [AutoRead(1)]
    [MustBe((ushort)0x001E)]
    public readonly ushort Id;

    [AutoRead(2)]
    [MustBe((uint)0x00000004)]
    public readonly uint Size;

    /// <summary>
    /// An unsigned integer that specifies the Help topic identifier in the Help file specified by PROJECTHELPFILEPATH Record (section 2.3.4.2.1.7).
    /// </summary>
    [AutoRead(3)]
    public readonly uint HelpContext;

    public MODULEHELPCONTEXT(XlBinaryReader Data)
        : base(Data)
    {
        Validate();
    }
}
