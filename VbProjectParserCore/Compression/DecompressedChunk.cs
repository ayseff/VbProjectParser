using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Compression
{
    public class DecompressedChunk
    {
        public byte[] Data;

        public DecompressedChunk(byte[] Data)
        {
            this.Data = Data;
        }
    }
}
