namespace ECS.Experimental
{
    public readonly struct Entity
    {
        public int Id
        {
            get;
        }

        internal bool NotNull
        {
            get;
        }

        public Context CurrentContext
        {
            get;
        }

        public Entity(int id, Context context)
        {
            Id = id;
            CurrentContext = context;
            NotNull = true;
        }
    }
}