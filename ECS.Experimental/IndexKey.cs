using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace ECS.Experimental
{
    internal readonly struct IndexKey
    {
        private readonly Type _componentType;
        private readonly int _componentTypeHash;
        private readonly int _valueHash;

        public IndexKey(Type componentType, int valueHash)
        {
            _componentType = componentType;
            _componentTypeHash = componentType.GetHashCode();
            _valueHash = valueHash;
        }

        public override int GetHashCode()
        {
            return CombineHashCode(_componentTypeHash, _valueHash);
        }

        public override bool Equals(object obj)
        {
            if (obj is IndexKey compositeKey)
            {
                return _componentType == compositeKey._componentType;
            }

            return false;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CombineHashCode(int h1, int h2)
        {
            return (int)((uint)(h1 << 5) | (uint)h1 >> 27) + h1 ^ h2;
        }
    }
}