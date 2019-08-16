using System;

namespace ECS.Experimental
{
    internal readonly struct IndexKey
    {
        private readonly Type _componentType;
        private readonly Type _indexType;

        public IndexKey(Type componentType, Type indexType)
        {
            _componentType = componentType;
            _indexType = indexType;
        }

        public override int GetHashCode()
        {
            var componentHash = _componentType.GetHashCode();
            return (componentHash << 5) + componentHash ^ _indexType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is IndexKey compositeKey)
            {
                return _componentType == compositeKey._componentType && _indexType == compositeKey._indexType;
            }

            return false;
        }
    }
}