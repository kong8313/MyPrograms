namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    public class ReplicatedColumn
    {
        public ReplicatedColumn(string name, string alias)
        {
            Name = name;
            Alias = alias;
        }

        public string Name { get; private set; }
        public string Alias { get; private set; }
    }
}