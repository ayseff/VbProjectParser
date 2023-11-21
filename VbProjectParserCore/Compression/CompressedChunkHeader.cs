using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Compression;

public class CompressedChunkHeader
{
    public ushort CompressedChunkSize { get; private set; }
    public byte CompressedChunkFlag { get; private set; }
    protected byte CompressedChunkSignature { get; private set; }

    protected ushort Header; // 2-byte / unsigned 16 bit


    public CompressedChunkHeader(XlBinaryReader Data)
    {
        // Algorithm as per page 60
        var Header = Data.ReadUInt16();

        Console.WriteLine($"CompressionChunkHeader Data Bytes: {Header.ToBitString()}  (uint16: {Header})");
        SetFrom(Header);


        Validate();

    }

    public CompressedChunkHeader(ushort FromValue)
    {
        SetFrom(FromValue);
        /*
        this.CompressedChunkSize = 0;
        this.CompressedChunkFlag = 0;
        this.CompressedChunkSignature = 0;
        this.Header = 0;*/
    }

    public void SetFrom(ushort value)
    {
        CompressedChunkFlag = ExtractCompressionChunkFlag(value);
        CompressedChunkSize = ExtractCompressionChunkSize(value);
        CompressedChunkSignature = ExtractCompressionChunkSignature(value);
        Header = value;
    }

    public ushort AsUInt16()
    {
        return Header;
    }

    protected void Validate()
    {
        ValidateSignature();
        ValidateFlag();
        ValidateSize();
    }

    /// <summary>
    /// Validates this.CompressedChunkFlag
    /// </summary>
    protected void ValidateFlag()
    {
        if (CompressedChunkFlag != 0x00 && CompressedChunkFlag != 0x01)
        {
            throw new FormatException($"Expected CompressedChunkFlag to be either 0x00 or 0x01, but was {CompressedChunkFlag:X}");
        }
    }

    /// <summary>
    /// Validates this.CompressedChunkSignature
    /// </summary>
    protected void ValidateSignature()
    {
        if (CompressedChunkSignature != 0x03)
        {
            throw new FormatException($"Signature byte expected 0x03, but was 0x{CompressedChunkSignature:X} (binary: {CompressedChunkSignature.ToBitString()}). (Header: 0b{Header.ToBitString()})");
        }
    }

    /// <summary>
    /// Validates this.CompressedChunkSize
    /// </summary>
    protected void ValidateSize()
    {
        if (CompressedChunkFlag == 0x00 && CompressedChunkSize != 4095)
        {
            throw new FormatException($"CompressionChunkFlag = {CompressedChunkFlag:X}, expected CompressedChunkSize 4095, but was {CompressedChunkSize}");
        }

        if (CompressedChunkSize < 0 || CompressedChunkFlag == 0x01 && CompressedChunkSize > 4095)
        {
            throw new FormatException($"Expected CompressedChunkSize to be between 0 and 4095, but was {CompressedChunkSize}");
        }
    }

    protected ushort ExtractCompressionChunkSize(ushort FromValue)
    {
        if (CompressedChunkFlag == 0x00)
            return 4095;

        // Extract CompressionChunkSize
        // page 66
        Console.WriteLine("Temp value: 0x{0:X}", FromValue);
        var temp = (ushort)(FromValue & 0x0FFF);
        temp = (ushort)(temp + 3);
        ushort size = temp;

        if (size < 3)
            throw new FormatException("Size was < 3");

        if (size > 4098)
            throw new FormatException("Size was > 4098");

        return size;
    }

    protected byte ExtractCompressionChunkFlag(ushort FromValue)
    {
        // Extract CompressionChunkFlag
        // page 67
        Console.WriteLine($"Extracting CompressionChunkFlag from header {Header}");
        var temp = FromValue & 0x8000;
        temp = temp >> 15;  // right shift 15 bits
        byte result = (byte)temp;
        Console.WriteLine($"Extracted flag: {temp} (cast to Byte: {result})");

        return result;
    }

    protected byte ExtractCompressionChunkSignature(ushort FromValue)
    {
        ushort signature = (ushort)(FromValue >> 12);
        signature = (ushort)(signature & 0x07);

        var result = (byte)signature;

        return (byte)signature;
    }



}
