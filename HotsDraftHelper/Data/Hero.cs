namespace HotsDraftHelper.Data
{
    internal sealed class Hero
    {
        public int Id { get; }
        public string Name { get; }
        public string Group { get; }
        public string SubGroup { get; }

        public Hero(int id, string name, string group, string subGroup)
        {
            Id = id;
            Name = name;
            Group = group;
            SubGroup = subGroup;
        }

        public override string ToString()
        {
            return $"{Id}: {Name} ({Group}/{SubGroup})";
        }
    }
}
