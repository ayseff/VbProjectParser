using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserTestsCore.Data;

namespace VbProjectParserTestsCore.Decompression;

public abstract class BaseCase
{
    protected ICompressionData TestData;

    /// <summary>
    /// Tests the reconstruction of uncompressed data ("No compression example", page 103)
    /// </summary>
    [TestMethod]
    public void DecompressionTest()
    {
        byte[] CompressedData = TestData.CompressedData;
        byte[] uncompressed = XlCompressionAlgorithm.Decompress(CompressedData);

        bool success = uncompressed.SequenceEqual(TestData.UncompressedData);

        Assert.IsTrue(success, "Uncompressed byte sequence not equal to expected byte sequence");
    }
}
