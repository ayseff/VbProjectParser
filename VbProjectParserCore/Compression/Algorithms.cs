using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Compression
{
    public class DecompressionState
    {
        public int CompressionRecordEnd = 0;
        public int CompressedCurrent = 0;
        public int CompressedChunkStart = 0;
        public int DecompressedCurrent = 0;
        public int DecompressedBufferEnd = 0;
        public int DecompressedChunkStart = 0;

        public DecompressionState()
        {
        }

        /// <summary>
        /// For compressing, where buffer is already filled with the fully decompressed data
        /// </summary>
        public DecompressionState(DecompressedBuffer buffer)
        {
            CompressionRecordEnd = 0; // unknown yet
            CompressedCurrent = 0; // to start with
            CompressedChunkStart = 0; // to start with
            DecompressedCurrent = 0; // to start with
            DecompressedBufferEnd = buffer.Data.Count(); // todo: or +1?
            DecompressedChunkStart = 0; // to start with
        }
    }

    public class Algorithms
    {


        public static void Decompress(DecompressedBuffer outputBuffer, CompressedContainer container)
        {

        }
    }
}
