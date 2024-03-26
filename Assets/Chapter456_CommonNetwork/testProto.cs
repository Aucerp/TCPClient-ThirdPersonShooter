using UnityEngine;
public class testProto : MonoBehaviour
{
    //將protobuf 對象序列化為byte數組
    public static byte[] Encode(ProtoBuf.IExtensible msgBase)
    {
        using (var memory = new System.IO.MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(memory, msgBase);
            return memory.ToArray();
        }
    }

    private void Start()
    {
        //ProtoBuf測試
        MsgMove msgMove = new MsgMove();
        msgMove.x = 1;
        //byte[] bs = Encode(msgMove);
        //Debug.Log(System.BitConverter.ToString(bs));
    }
}
