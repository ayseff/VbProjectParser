using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Compression
{
    public class CompressedChunk
    {
        public readonly CompressedChunkHeader CompressedHeader;
        public readonly CompressedChunkData CompressedData;

        /// <summary>
        /// Location of the first byte of the CompressedChunk within the CompressedContainer
        /// </summary>
        protected int CompressedChunkStart;

        public CompressedChunk(XlBinaryReader Data)
        {
            CompressedChunkStart = Data.i;

            CompressedHeader = new CompressedChunkHeader(Data);
            CompressedData = new CompressedChunkData(CompressedHeader, Data);
        }

        public void Decompress(DecompressedBuffer buffer, DecompressionState state)
        {
            state.DecompressedChunkStart = state.DecompressedCurrent;
            //var CompressedEnd = Math.Min(state.CompressionRecordEnd, state.CompressedChunkStart + this.CompressedHeader.CompressedChunkSize);
            state.CompressedCurrent += 2;

            CompressedData.Decompress(buffer, state);
        }

        /*
        public void Decompress(IList<Byte> ToTarget)
        {
            this.CompressedData.Decompress(ToTarget);
        }*/
    }
}
