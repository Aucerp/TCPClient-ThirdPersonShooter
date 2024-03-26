using System;
using UnityEngine;
using System.Linq;

public class MsgBase
{
    public string protoName = "null";
    // 編碼
    public static byte[] Encode(MsgBase msgBase)
    {
        string s = JsonUtility.ToJson(msgBase);
        return System.Text.Encoding.UTF8.GetBytes(s);
    }

    // 解碼
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
        MsgBase msgBase = (MsgBase)JsonUtility.FromJson(s, Type.GetType(protoName));
        return msgBase;
    }

    // 編碼協議名（2字節長度+字符串）
    public static byte[] EncodeName(MsgBase msgBase)
    {
        // 名字bytes和長度
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
        Int16 len = (Int16)nameBytes.Length;
        // 申請bytes數值
        byte[] bytes = new byte[2 + len];
        // 組裝2字節的長度信息
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        // 組裝名字bytes
        Array.Copy(nameBytes, 0, bytes, 2, len);

        return bytes;
    }

    // 解碼協議名（2字節長度+字符串）
    public static string DecodeName(byte[] bytes, int offset, out int count)
    {
        count = 0;
        // 必須大於2字節
        if (offset + 2 > bytes.Length)
        {
            return "";
        }
        // 讀取長度
        Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
        // 長度必須足夠
        if (offset + 2 + len > bytes.Length)
        {
            return "";
        }
        // 解析
        count = 2 + len;
        string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
        return name;
    }
}
