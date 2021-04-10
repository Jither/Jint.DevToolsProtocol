using Jint.Native.Object;
using System.Collections.Generic;

namespace Jint.DevToolsProtocol
{
    internal sealed class ObjectDataMap
    {
        private readonly Dictionary<int, ObjectData> _dataById = new Dictionary<int, ObjectData>();
        private readonly Dictionary<ObjectInstance, ObjectData> _dataByInstance = new Dictionary<ObjectInstance, ObjectData>();
        private readonly Dictionary<string, List<int>> _objectGroups = new Dictionary<string, List<int>>();

        private int _nextObjectId = 1;

        public ObjectData this[int id]
        {
            get
            {
                if (_dataById.TryGetValue(id, out var obj))
                {
                    return obj;
                }
                return null;
            }
        }

        public ObjectData GetOrAdd(ObjectInstance instance, string objectGroup = null, bool isScope = false)
        {
            if (!_dataByInstance.TryGetValue(instance, out var data))
            {
                data = Add(instance, objectGroup: objectGroup, isScope: isScope);
            }
            return data;
        }

        private ObjectData Add(ObjectInstance instance, string objectGroup = null, bool isScope = false)
        {
            var id = _nextObjectId++;
            var data = new ObjectData(id, instance, isScope);
            _dataById.Add(id, data);
            _dataByInstance.Add(instance, data);

            if (objectGroup != null)
            {
                if (!this._objectGroups.TryGetValue(objectGroup, out List<int> group))
                {
                    group = new List<int>();
                }
                group.Add(id);
            }

            return data;
        }

        public void Release(int id)
        {
            if (_dataById.TryGetValue(id, out var data))
            {
                _dataByInstance.Remove(data.Instance);
            }
        }

        public void ReleaseGroup(string objectGroup)
        {
            if (_objectGroups.TryGetValue(objectGroup, out var ids))
            {
                foreach (var id in ids)
                {
                    Release(id);
                }
            }
            _objectGroups.Remove(objectGroup);
        }
    }
}
