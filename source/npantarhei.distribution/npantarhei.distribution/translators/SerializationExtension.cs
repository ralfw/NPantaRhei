using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace npantarhei.distribution.translators
{
    public static class SerializationExtension
    {
        public static byte[] Serialize(this object obj)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.GetBuffer();
        }

        public static object Deserialize(this byte[] bytes)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream(bytes);
            return bf.Deserialize(ms);
        }  
    }
}
