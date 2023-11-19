using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserTestsCore.Data;

namespace VbProjectParserTestsCore.Compression
{
    public abstract class BaseCase
    {
        protected ICompressionData TestData;

        /// <summary>
        /// Tests the reconstruction of uncompressed data ("No compression example", page 103)
        /// </summary>
        [TestMethod]
        public void CompressionTest()
        {
            byte[] UncompressedData = TestData.UncompressedData;

            byte[] CompressedData = XlCompressionAlgorithm.Compress(UncompressedData);

            // Careful: ComressedData is NOT necessarily equal to TestData.CompressedData
            // See comment in Data.BaseCase.
            // So best we just un-compress the compressed data again, and see if the result is equal to UncompressedData
            byte[] UncompressedCompressedData = XlCompressionAlgorithm.Decompress(CompressedData);

            bool success = UncompressedData.SequenceEqual(UncompressedCompressedData);

            Assert.IsTrue(success, "Compressed byte sequence not equal to expected byte sequence");
        }
    }
}
