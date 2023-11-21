using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Compression;

public class XlBinaryReader
{
    protected readonly byte[] Data;
    public int i { get; protected set; }

    public int Length => Data.Length;

    public bool EndOfData => i >= Length;

    public XlBinaryReader(ref byte[] input)
    {
        Data = input;
        i = 0;
    }

    public byte[] GetUnreadData()
    {
        int N = Length - i;
        var result = new byte[N];
        Array.Copy(Data, i, result, 0, N);
        return result;
    }

    public object Read(Type type)
    {
        if (type == typeof(ushort))
            return ReadUInt16();
        else if (type == typeof(short))
            return ReadInt16();
        else if (type == typeof(uint))
            return ReadUInt32();
        else if (type == typeof(int))
            return ReadInt32();
        else if (type == typeof(bool))
            return ReadBool();
        else if (type == typeof(byte))
            return ReadByte();
        else if (type == typeof(Guid))
            return ReadGuid();
        else
            throw new InvalidCastException();
    }

    public object ReadArray(Type type, int size)
    {
        var result = Array.CreateInstance(type, size);

        for (int i = 0; i < size; i++)
        {
            result.SetValue(Read(type), i);
        }

        return result;
    }

    public int ReadInt32()
    {
        var bytes = Read(sizeof(int));
        return BitConverter.ToInt32(bytes, 0);
    }

    public uint ReadUInt32()
    {
        var bytes = Read(sizeof(uint));
        return BitConverter.ToUInt32(bytes, 0);
    }

    public short ReadInt16()
    {
        var bytes = Read(sizeof(short));
        return BitConverter.ToInt16(bytes, 0);
    }

    public ushort ReadUInt16()
    {
        var bytes = Read(sizeof(ushort));
        return BitConverter.ToUInt16(bytes, 0);
    }

    /// <summary>
    /// Peets at the next UInt16, but does not progress the pointer
    /// </summary>
    public ushort PeekUInt16()
    {
        var bytes = Read(i, sizeof(ushort));
        return BitConverter.ToUInt16(bytes, 0);
    }

    public bool ReadBool()
    {
        var bytes = Read(sizeof(bool));
        return BitConverter.ToBoolean(bytes, 0);
    }

    public byte ReadByte()
    {
        var bytes = Read(1);
        return bytes[0];
    }

    public Guid ReadGuid()
    {
        var bytes = ReadBytes(16);
        var guid = new Guid(bytes);
        return guid;
    }

    /// <summary>
    /// Peeks at the next byte, but does not progress the pointer
    /// </summary>
    public byte PeekByte() => ReadByteAt(i);

    /// <summary>
    /// Reads the byte at the given index
    /// </summary>
    public byte ReadByteAt(uint index)
    {
        if (index > int.MaxValue)
            throw new InvalidOperationException();

        var bytes = Read(Convert.ToInt32(index), 1);
        return bytes[0];
    }

    public byte ReadByteAt(int index) => ReadByteAt(Convert.ToUInt32(index));

    public byte[] ReadBytes(int length) => ReadBytes(Convert.ToUInt32(length));

    public byte[] ReadBytes(uint length)
    {
        var bytes = Read(length);
        return bytes;
    }

    /// <summary>
    /// Reads data of length $length from the current index (this.i), and automatically advances the current index
    /// </summary>
    protected byte[] Read(uint length)
    {
        if (length > int.MaxValue)
            throw new InvalidOperationException();

        StackFrame frame = new(2);
        var method = frame.GetMethod();

        Console.WriteLine($"{method.DeclaringType}.{method.Name} - reading {length} bytes");

        int _length = Convert.ToInt32(length);

        var result = Read(i, _length);
        i += _length;
        return result;
    }

    /// <summary>
    /// Reads data of length at index. Does not progress the index counter.
    /// </summary>
    protected byte[] Read(int index, int length)
    {
        byte[] bytes = new byte[length];
        Array.Copy(Data, i, bytes, 0, length);
        return bytes;
    }

    public void OutputAllAsBinary()
    {
        for (int i = 0; i < Length; i++)
        {
            Console.WriteLine($"{i:00}: {Data[i].ToBitString()} (0x{Data[i]:X})");
        }

    }

}
