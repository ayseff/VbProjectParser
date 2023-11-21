using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Compression;

// Buffer to write decompressed data to
public class DecompressedBuffer
{
    protected List<byte> _Data;
    public IEnumerable<byte> Data => _Data;

    public DecompressedBuffer(int InitialSizeAllocation = 10000)
    {
        _Data = new List<byte>(InitialSizeAllocation);
    }

    public DecompressedBuffer(byte[] UncompressedData)
    {
        _Data = UncompressedData.ToList();
    }


    public void Add(DecompressedChunk Chunk)
    {
        _Data.AddRange(Chunk.Data);
    }

    public virtual void SetByte(int index, byte value)
    {
        int C = Data.Count();

        if (index < C)
        {
            _Data[index] = value;
        }
        else if (index == C)
        {
            _Data.Add(value);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be <= " + C);
        }
    }

    public byte GetByteAt(int index)
    {
        return _Data.ElementAt(index);
    }

    public byte[] GetData()
    {
        return Data.ToArray();
    }
}
