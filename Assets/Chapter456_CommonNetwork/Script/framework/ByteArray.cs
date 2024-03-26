using System;

public class ByteArray  
{
    // 預設大小
    const int DEFAULT_SIZE = 1024;
    // 初始大小
    int initSize = 0;
    // 緩衝區
    public byte[] bytes;
    // 讀寫位置
    public int readIdx = 0;
    public int writeIdx = 0;
    // 容量
    private int capacity = 0;
    // 剩餘空間
    public int remain { get { return capacity - writeIdx; } }
    // 資料長度
    public int length { get { return writeIdx - readIdx; } }

    // 建構函式
    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIdx = 0;
        writeIdx = 0;
    }

    // 建構函式
    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }

    // 重設尺寸
    public void ReSize(int size)
    {
        if (size < length) return;
        if (size < initSize) return;
        int n = 1;
        while (n < size) n *= 2;
        capacity = n;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIdx, newBytes, 0, writeIdx - readIdx);
        bytes = newBytes;
        writeIdx = length;
        readIdx = 0;
    }

    // 寫入資料
    public int Write(byte[] bs, int offset, int count)
    {
        if (remain < count)
        {
            ReSize(length + count);
        }
        Array.Copy(bs, offset, bytes, writeIdx, count);
        writeIdx += count;
        return count;
    }

    // 讀取資料
    public int Read(byte[] bs, int offset, int count)
    {
        count = Math.Min(count, length);
        Array.Copy(bytes, 0, bs, offset, count);
        readIdx += count;
        CheckAndMoveBytes();
        return count;
    }

    // 檢查並移動資料
    public void CheckAndMoveBytes()
    {
        if (length < 8)
        {
            MoveBytes();
        }
    }

    // 移動資料
    public void MoveBytes()
    {
        Array.Copy(bytes, readIdx, bytes, 0, length);
        writeIdx = length;
        readIdx = 0;
    }

    // 讀取Int16
    public Int16 ReadInt16()
    {
        if (length < 2) return 0;
        Int16 ret = BitConverter.ToInt16(bytes, readIdx);
        readIdx += 2;
        CheckAndMoveBytes();
        return ret;
    }

    // 讀取Int32
    public Int32 ReadInt32()
    {
        if (length < 4) return 0;
        Int32 ret = BitConverter.ToInt32(bytes, readIdx);
        readIdx += 4;
        CheckAndMoveBytes();
        return ret;
    }

    // 列印緩衝區
    public override string ToString()
    {
        return BitConverter.ToString(bytes, readIdx, length);
    }

    // 列印除錯資訊
    public string Debug()
    {
        return string.Format("readIdx({0}) writeIdx({1}) bytes({2})",
            readIdx,
            writeIdx,
            BitConverter.ToString(bytes, 0, capacity)
        );
    }
}
