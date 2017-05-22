using System.Runtime.Serialization;

namespace HW.Definitions
{
    [DataContract]
    public class ScanServiceFileChunk
    {
        [DataMember]
        public byte[] Data { get; set; }
    }
}
