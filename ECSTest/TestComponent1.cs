using System;
using ECS.Experimental;

namespace ECSTest
{
    [Component(typeof(MainContext))]
    public struct TestComponent1
    {
        private Context _context;
        private int _sourceEntityId;
        private Type _type;

        private int _x;
        public int X
        {
            get => _x;
            set
            {
                _x = value;
                _context.MarkComponentDirty(_sourceEntityId, _type);
            }
        }
    }
}