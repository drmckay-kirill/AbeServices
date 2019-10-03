using System.IO;
using ProtoBuf;

namespace AbeServices.Common.Helpers
{
    public class ProtobufDataSerializer: IDataSerializer
    {
        public byte[] Serialize<T>(T data)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(ms, data, PrefixStyle.Fixed32);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            using (var ms = new MemoryStream(data, 0, data.Length))
            {
                var res = Serializer.DeserializeWithLengthPrefix<T>(ms, PrefixStyle.Fixed32);
                return res;
            }
        }

    }
}