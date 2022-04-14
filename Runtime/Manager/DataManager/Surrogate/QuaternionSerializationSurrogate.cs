namespace SoraCore.Manager.Serialization
{
    using System.Runtime.Serialization;
    using UnityEngine;

    public class QuaternionSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var quat = (Quaternion)obj;
            info.AddValue("x", quat.x);
            info.AddValue("y", quat.y);
            info.AddValue("z", quat.z);
            info.AddValue("w", quat.w);

        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var quat = (Quaternion)obj;
            quat.x = (float)info.GetValue("x", typeof(float));
            quat.y = (float)info.GetValue("y", typeof(float));
            quat.z = (float)info.GetValue("z", typeof(float));
            quat.w = (float)info.GetValue("w", typeof(float));

            return quat;
        }
    }
}