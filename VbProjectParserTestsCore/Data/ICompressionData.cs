using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserTestsCore.Data
{
    public interface ICompressionData
    {
        byte[] UncompressedData { get; }
        byte[] CompressedData { get; }
    }
}
