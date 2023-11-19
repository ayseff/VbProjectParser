using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Compression
{
    // my own naming
    public class XlCompressionAlgorithm
    {
        // Section 2.4.1.3.6
        // page 62
        /// <summary>
        /// Compresses the data given in UncompressedData.
        /// </summary>
        /// <param name="UncompressedData">Uncompressed raw data</param>
        /// <returns>A byte array containing the compressed data</returns>
        public static byte[] Compress(byte[] UncompressedData)
        {
            if (UncompressedData == null)
                throw new ArgumentNullException(nameof(UncompressedData));

            var resultBuffer = new CompressedBuffer(UncompressedData.Length);

            DecompressedBuffer uncompressedDataBuffer = new DecompressedBuffer(UncompressedData);
            var state = new DecompressionState(uncompressedDataBuffer);

            // 2.4.1.3.6 Compression algorithm
            resultBuffer.SetByte(state.CompressedCurrent, 0x01);
            ++state.CompressedCurrent;

            while (state.DecompressedCurrent < state.DecompressedBufferEnd)
            {
                state.CompressedChunkStart = state.CompressedCurrent;
                state.DecompressedChunkStart = state.DecompressedCurrent;
                CompressDecompressedChunk(UncompressedData, resultBuffer, state);
            }

            return resultBuffer.GetData();
        }

        /// <summary>
        /// Decompresses the given data
        /// </summary>
        /// <param name="CompressedData">Byte array of the compressed data</param>
        /// <returns>A byte array containing the uncompressed data</returns>
        public static byte[] Decompress(byte[] CompressedData)
        {
            if (CompressedData == null)
                throw new ArgumentNullException(nameof(CompressedData));

            var reader = new XlBinaryReader(ref CompressedData);
            reader.OutputAllAsBinary();
            var container = new CompressedContainer(reader);
            var buffer = new DecompressedBuffer();
            container.Decompress(buffer);
            byte[] UncompressedCompressedData = buffer.GetData();

            return UncompressedCompressedData;
        }

        // 2.4.1.3.7 Compressing a DecompressedChunk
        // page 62
        private static void CompressDecompressedChunk(byte[] Data, CompressedBuffer resultBuffer, DecompressionState state)
        {
            var CompressedEnd = state.CompressedChunkStart + 4098;
            state.CompressedCurrent = state.CompressedChunkStart + 2;
            var DecompressedEnd = Math.Min(state.DecompressedChunkStart + 4096, state.DecompressedBufferEnd);

            while (state.DecompressedCurrent < DecompressedEnd && state.CompressedCurrent < CompressedEnd)
            {
                CompressTokenSequence(Data, resultBuffer, state, CompressedEnd, DecompressedEnd);
            }

            ushort CompressedFlag;

            if (state.DecompressedCurrent < DecompressedEnd)
            {
                CompressRawChunk(Data, resultBuffer, state, DecompressedEnd - 1);
                CompressedFlag = 0;
            }
            else
            {
                CompressedFlag = 1;
            }


            ushort size = Convert.ToUInt16(state.CompressedCurrent - state.CompressedChunkStart);
            var header = new CompressedChunkHeader(0x0000);

            PackCompressedChunkSize(Data, state, size, header);
            PackCompressedChunkFlag(Data, state, CompressedFlag, header);
            PackCompressedChunkSignature(Data, state, header);

            // SET the CompressedChunkHeader (section 2.4.1.1.5) located at CompressedChunkStart TO Header
            var bytes = BitConverter.GetBytes(header.AsUInt16());
            resultBuffer.SetByte(state.CompressedChunkStart, bytes.First());
            resultBuffer.SetByte(state.CompressedChunkStart + 1, bytes.Skip(1).Single());

        }

        // Section 2.4.1.3.13
        // page 66
        private static void PackCompressedChunkSize(byte[] Data, DecompressionState state, ushort size, CompressedChunkHeader header)
        {
            if (size > 4098 || size < 3)
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be between 3 - 4098");

            ushort temp1 = (ushort)(header.AsUInt16() & 0xF000);
            ushort temp2 = (ushort)(size - 3);
            ushort result = (ushort)(temp1 | temp2);

            header.SetFrom(result);
        }

        // Section 2.4.1.3.14
        private static void PackCompressedChunkSignature(byte[] Data, DecompressionState state, CompressedChunkHeader header)
        {
            ushort temp = (ushort)(header.AsUInt16() & 0x8FFF);
            ushort result = (ushort)(temp | 0x3000);

            header.SetFrom(result);
        }

        // Section 2.4.1.3.16
        private static void PackCompressedChunkFlag(byte[] Data, DecompressionState state, ushort CompressedFlag, CompressedChunkHeader header)
        {
            if (CompressedFlag != 0 && CompressedFlag != 1)
                throw new ArgumentOutOfRangeException(nameof(CompressedFlag), "CompressedFlag must be 0 or 1");

            ushort temp1 = (ushort)(header.AsUInt16() & 0x7FFF);
            ushort temp2 = (ushort)(CompressedFlag << 15);
            ushort result = (ushort)(temp1 | temp2);

            header.SetFrom(result);
        }


        // Section 2.4.1.3.8
        // page 63
        private static void CompressTokenSequence(byte[] Data, CompressedBuffer resultBuffer, DecompressionState state, int CompressedEnd, int DecompressedEnd)
        {
            var FlagByteIndex = state.CompressedCurrent;
            byte TokenFlags = 0x0; // 0b00000000
            ++state.CompressedCurrent;

            for (int index = 0; index <= 7; index++)
            {
                if (state.DecompressedCurrent < DecompressedEnd && state.CompressedCurrent < CompressedEnd)
                {
                    TokenFlags = CompressToken(Data, resultBuffer, state, CompressedEnd, DecompressedEnd, index, TokenFlags);
                }
            }

            resultBuffer.SetByte(FlagByteIndex, TokenFlags);
        }

        // section 2.4.1.3.18
        // page 67, 68
        private static byte SetFlagBit(int Index, int Flag, byte FlagByte)
        {
            var temp1 = Flag << Index;
            var temp2 = FlagByte & ~temp1;
            var result = (byte)(temp2 | temp1);

            return result;
        }

        // Section 2.4.1.3.9
        // page 64
        private static byte CompressToken(byte[] Data, CompressedBuffer resultBuffer, DecompressionState state, int CompressedEnd, int DecompressedEnd, int index, byte Flags)
        {
            ushort Offset = 0;

            var match = Matching(Data, state, DecompressedEnd);
            Offset = match.Offset;
            ushort Length = match.Length;

            if (Offset != 0)
            {
                if (state.CompressedCurrent + 1 < CompressedEnd)
                {
                    var Token = PackCopyToken(Data, state, Offset, Length);

                    var bytes = BitConverter.GetBytes(Token.AsUInt16());

                    // Convert to little endian order, if necessary
                    if (!BitConverter.IsLittleEndian)
                        bytes = bytes.Reverse().ToArray();

                    byte byte1 = bytes.First();
                    byte byte2 = bytes.Skip(1).Single();

                    /*
                    ////////////////// DEBUG ////////////////////////////////////

                    byte[] _debug_all_expected_bytes = .... get expected output e.g. from unit test data ...
                    byte[] _debug_expected_bytes = new byte[] { _debug_all_expected_bytes.ElementAt(state.CompressedCurrent), _debug_all_expected_bytes.ElementAt(state.CompressedCurrent + 1) };
       
                    var TokenValue_Expected = BitConverter.ToUInt16(_debug_expected_bytes, 0);
                    var Token_Expected = new CopyToken(TokenValue_Expected);

                    CopyToken.UnpackedInfo Actual_Infos = Token.UnpackCopyToken(state.DecompressedCurrent, state.DecompressedChunkStart);
                    CopyToken.UnpackedInfo Expected_Infos = Token_Expected.UnpackCopyToken(state.DecompressedCurrent, state.DecompressedChunkStart);
         
                    string _debug_bitStr = BitHelper.ToBitString(bytes);
                    string _debug_str_found = String.Format("ACTUAL:  Token at index {0}: {1} (Bits {2}). Length = {3}, Offset = {4} (-> copy bytes: {5}).", state.CompressedCurrent, BitConverter.ToString(bytes), BitHelper.ToBitString(bytes), Actual_Infos.Length, Actual_Infos.Offset, BitConverter.ToString(Data.Skip(state.DecompressedCurrent - Actual_Infos.Offset).Take(Actual_Infos.Length).ToArray()));
                    string _debug_str_expected = String.Format("EXPECTED: Token at index {0}: {1} (Bits {2}). Length = {3}, Offset = {4} (-> copy bytes: {5})", state.CompressedCurrent, BitConverter.ToString(_debug_expected_bytes), BitHelper.ToBitString(_debug_expected_bytes), Expected_Infos.Length, Expected_Infos.Offset, BitConverter.ToString(Data.Skip(state.DecompressedCurrent - Expected_Infos.Offset).Take(Expected_Infos.Length).ToArray()));

                    Trace.WriteLine(_debug_str_found);
                    Trace.WriteLine(_debug_str_expected);


                    ////////////////// DEBUG ////////////////////////////////////
                     * */

                    resultBuffer.SetByte(state.CompressedCurrent, byte1);
                    resultBuffer.SetByte(state.CompressedCurrent + 1, byte2);

                    Flags = SetFlagBit(index, 1, Flags);

                    state.CompressedCurrent += 2;
                    state.DecompressedCurrent += Length;
                }
                else
                {
                    state.CompressedCurrent = CompressedEnd;
                }
            }
            else
            {
                if (state.CompressedCurrent < CompressedEnd)
                {
                    byte LiteralToken = Data[state.DecompressedCurrent];
                    resultBuffer.SetByte(state.CompressedCurrent, LiteralToken);
                    ++state.CompressedCurrent;
                    ++state.DecompressedCurrent;
                }
                else
                {
                    state.CompressedCurrent = CompressedEnd;
                }
            }

            return Flags;
        }

        // Section 2.4.1.3.19.3
        // page 69, 70
        private static CopyToken PackCopyToken(byte[] Data, DecompressionState state, ushort Offset, ushort Length)
        {
            var help = CopyToken.CopyTokenHelp(state.DecompressedCurrent, state.DecompressedChunkStart);

            var temp1 = (ushort)(Offset - 1);
            var temp2 = (ushort)(16 - help.BitCount);
            var temp3 = (ushort)(Length - 3);

            ushort TokenAsUInt16 = (ushort)((ushort)(temp1 << temp2) | temp3);
            var result = new CopyToken(TokenAsUInt16);

            return result;
        }

        private class MatchingResult
        {
            public readonly ushort Offset;
            public readonly ushort Length;

            public MatchingResult(ushort Offset, ushort Length)
            {
                this.Offset = Offset;
                this.Length = Length;
            }
        }

        // Section 2.4.1.3.19.4
        // page 70
        private static MatchingResult Matching(byte[] Data, DecompressionState state, int DecompressedEnd)
        {
            int Candidate = state.DecompressedCurrent - 1;
            ushort BestLength = 0;

            int BestCandidate = 0;
            ushort Offset;
            ushort Length;


            while (Candidate >= state.DecompressedChunkStart)
            {
                int C = Candidate;
                int D = state.DecompressedCurrent;
                ushort Len = 0;

                while (D < DecompressedEnd && Data[D] == Data[C])
                {
                    ++Len;
                    ++C;
                    ++D;
                }

                if (Len > BestLength)
                {
                    BestLength = Len;
                    BestCandidate = Candidate;
                }

                --Candidate;
            }

            if (BestLength >= 3)
            {
                ushort MaximumLength = CopyToken.CopyTokenHelp(state.DecompressedCurrent, state.DecompressedChunkStart).MaximumLength;
                Length = Math.Min(BestLength, MaximumLength);
                Offset = (ushort)(state.DecompressedCurrent - BestCandidate);
            }
            else
            {
                Length = 0;
                Offset = 0;
            }

            return new MatchingResult(Offset, Length);
        }

        // section 2.4.1.3.10
        // page 65
        private static void CompressRawChunk(byte[] Data, CompressedBuffer resultBuffer, DecompressionState state, int LastByte)
        {
            state.CompressedCurrent = state.CompressedChunkStart + 2;
            state.DecompressedCurrent = state.DecompressedChunkStart;
            int PadCount = 4096;

            for (int i = state.DecompressedChunkStart; i <= LastByte; i++)
            {
                byte B = Data[i]; // issue: do they really mean from Data?
                resultBuffer.SetByte(state.CompressedCurrent, B);

                ++state.CompressedCurrent;
                ++state.DecompressedCurrent;
                --PadCount;
            }

            for (int counter = 1; counter <= PadCount; counter++)
            {
                resultBuffer.SetByte(state.CompressedCurrent, 0x00);
                ++state.CompressedCurrent;
            }
        }
    }
}
