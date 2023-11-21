using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Data.ABNF.Enums;

public enum LibidReferenceKind
{
    /// <summary>
    /// Windows file path
    /// </summary>
    WindowsFilePath,

    /// <summary>
    /// Macintosh file path
    /// </summary>
    MacintoshFilePath
}


public static class LibidReferenceKindKindExtensions
{
    public static byte ToByte(this LibidReferenceKind LibidReferenceKind)
    {
        switch (LibidReferenceKind)
        {
            case LibidReferenceKind.WindowsFilePath:
                return 0x47;
            case LibidReferenceKind.MacintoshFilePath:
                return 0x48;
            default:
                throw new NotSupportedException($"LibidReferenceKind {LibidReferenceKind} not supported");

        }
    }

    public static LibidReferenceKind ToLibidReferenceKindType(this byte @byte)
    {
        switch (@byte)
        {
            case 0x47:
                return LibidReferenceKind.WindowsFilePath;
            case 0x48:
                return LibidReferenceKind.MacintoshFilePath;
            default:
                throw new NotSupportedException($"LibidReferenceKind byte {@byte} not supported");
        }
    }

    public static char ToChar(this LibidReferenceKind LibidReferenceKind)
    {
        byte[] bytes = new byte[] { LibidReferenceKind.ToByte() };
        return Encoding.ASCII.GetChars(bytes).Single();
    }
}
