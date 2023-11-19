using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserTestsCore.Data
{
    public abstract class BaseCase : ICompressionData
    {
        public byte[] UncompressedData { get; protected set; }
        public byte[] CompressedData { get; protected set; }
    }
}
