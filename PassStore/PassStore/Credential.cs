using System;

namespace PassStore
{
    public class Credential
    {
        public string Name { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public Credential(string[] oneRow)
        {
            Name = oneRow.Length > 0 ? oneRow[0] : string.Empty;
            Login = oneRow.Length > 1 ? oneRow[1] : string.Empty;
            Password = oneRow.Length > 2 ? oneRow[2] : string.Empty;
        }

        public Credential(string registryInfo)
        {
            string[] cells = registryInfo.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

            Name = cells.Length > 0 ? cells[0] : string.Empty;
            Login = cells.Length > 1 ? cells[1] : string.Empty;
            Password = cells.Length > 2 ? cells[2] : string.Empty;
        }
    }
}
