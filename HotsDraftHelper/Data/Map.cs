namespace HotsDraftHelper.Data
{
    internal sealed class Map
    {
        public int Id { get; }
        public string Name { get; }

        public Map(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Id}: {Name}";
        }
    }
}
