using Jint.Native.Object;

namespace Jint.DevToolsProtocol
{
    public class ObjectData
    {
        public int Id { get; }
        public ObjectInstance Instance { get; }
        public bool IsScope { get; }

        public ObjectData(int id, ObjectInstance instance, bool isScope = false)
        {
            Id = id;
            Instance = instance;
            IsScope = isScope;
        }
    }
}
