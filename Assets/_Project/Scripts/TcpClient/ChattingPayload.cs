using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.IO;
using static ChattingPacket;

public class ChattingPayload
{
    public ChattingPayloadOneofCase type;
    public string version;
    public int sequence;
    public byte[] payloadBytes;

    public ChattingPacket chattingPacket
    {
        get
        {
            ChattingPacket chattingPacket = new ChattingPacket();
            chattingPacket.MergeFrom(payloadBytes);
            return chattingPacket;
        }
    }

    public MessageDescriptor Descriptor => throw new NotImplementedException();

    public ChattingPayload(byte[] bytes)
    {
        var stream = new MemoryStream(bytes);
        var reader = new BinaryReader(stream);
        var data = reader.ReadBytes(2);
        Array.Reverse(data);
        type = (ChattingPayloadOneofCase)BitConverter.ToInt16(data);
        data = reader.ReadBytes(1);
        var length = data[0] & 0xff;
        data = reader.ReadBytes(length);
        version = BitConverter.ToString(data);
        data = reader.ReadBytes(4);
        Array.Reverse(data);
        sequence = BitConverter.ToInt32(data);
        data = reader.ReadBytes(4);
        Array.Reverse(data);
        var payloadLength = BitConverter.ToInt32(data);
        payloadBytes = reader.ReadBytes(payloadLength);
    }

    public ChattingPayload(ChattingPayloadOneofCase type, string version, int sequence, byte[] payloadBytes)
    {
        this.type = type;
        this.version = version;
        this.sequence = sequence;

        this.payloadBytes = payloadBytes;
    }

    public ArraySegment<byte> ToByteArray()
    {
        var stream = new MemoryStream();        
        var writer = new BinaryWriter(stream);
        byte[] bytes = new byte[1024];
        var fileds = GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (var field in fileds)
        {
            if (field.FieldType == typeof(int))
            {
                bytes = BitConverter.GetBytes((int)field.GetValue(this));
                Array.Reverse(bytes);
                writer.Write(bytes);
            }
            else if (field.FieldType == typeof(string))
            {
                var str = (string)field.GetValue(this);
                //writer.Write((char)UTF8Encoding.UTF8.GetBytes(str).Length);
                writer.Write(str);
            }
            else if (field.FieldType == typeof(bool))
            {
                writer.Write((bool)field.GetValue(this));
            }
            else if (field.FieldType == typeof(short))
            {
                bytes = BitConverter.GetBytes((short)field.GetValue(this));
                Array.Reverse(bytes);
                writer.Write(bytes);
            }
            else if (field.FieldType == typeof(float))
            {
                bytes = BitConverter.GetBytes((float)field.GetValue(this));
                Array.Reverse(bytes);
                writer.Write(bytes);
            }
            else if (field.FieldType == typeof(double))
            {
                bytes = BitConverter.GetBytes((double)field.GetValue(this));
                Array.Reverse(bytes);
                writer.Write(bytes);
            }
            else if (field.FieldType.IsEnum)
            {
                bytes = BitConverter.GetBytes((short)(int)field.GetValue(this));
                Array.Reverse(bytes);
                writer.Write(bytes);
            }
            else
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    var array = (byte[])field.GetValue(this);
                    bytes = BitConverter.GetBytes(array.Length);
                    Array.Reverse(bytes);
                    writer.Write(bytes);
                    writer.Write(new ArraySegment<byte>(array));
                    memory.Dispose();
                }
            }
        }

        writer.Flush();
        stream.Dispose();
        return stream.ToArray();
    }
}
