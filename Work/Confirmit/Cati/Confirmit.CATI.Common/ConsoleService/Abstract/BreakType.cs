using System;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    public class BreakType
    {
        public BreakType(int id, string name, string description, bool isPaid)
        {
            Id = id;
            Name = name;
            Description = description;
            IsPaid = isPaid;
        }

        public BreakType()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsPaid { get; set; }
    }
}