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
                Serializer.Serialize(ms, data);
                return ms.ToArray();
            }
        }
    }
}