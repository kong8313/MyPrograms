using CommandLine;

namespace DatabaseMigrationUtility
{
    public class MigratorOptions
    {
        [Option("companyId", SetName = "id", Required = true, HelpText = "Migrate just specific company")]
        public int? CompanyId { get; set; }

        [Option("all", SetName = "all", Required = true, HelpText = "Migrate all companies")]
        public bool All { get; set; }
    }
}