using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace StaticTeamCityBuildEngine
{
    public class BuildVersionExtractor : Task
    {
        [Required]
        public string BuildNumber { get; private set; }

        public string BranchName { get; private set; }

        [Output]
        public string MajorVersion { private get; set; }

        [Output]
        public string MinorVersion { private get; set; }

        [Output]
        public string NugetPackageVersion { private get; set; }

        [Output]
        public string MsiVersion { private get; set; }

        public override bool Execute()
        {
            if (BranchName == null)
            {
                BranchName = string.Empty;
            }

            var version = new Version(BuildNumber);

            MajorVersion = version.Major.ToString();
            MinorVersion = version.Minor.ToString();

            switch (BranchName.ToLowerInvariant())
            {
                case "master":
                    NugetPackageVersion = $"{MajorVersion}.{MinorVersion}.{version.Build}-ci0001";
                    break;
                case "release/cd":
                    NugetPackageVersion = $"{MajorVersion}.{MinorVersion}.{version.Build}";
                    break;
                default:
                    string normalizedBranchName = string.Empty;

                    if (!string.IsNullOrEmpty(BranchName))
                    {
                        normalizedBranchName = "-" + BranchName.Replace("refs/merge-requests", "mr").Replace('/', '-').Replace('_', '-');
                        if (normalizedBranchName.Length > 21)
                        {
                            normalizedBranchName = normalizedBranchName.Substring(0, 21).TrimEnd('-');
                        }
                    }

                    NugetPackageVersion = $"{MajorVersion}.{MinorVersion}.{version.Build}{normalizedBranchName}";
                    break;
            }

            var msiMajorVersion = version.Major > 1994 ? version.Major - 1994 : version.Major;
            MsiVersion = $"{msiMajorVersion}.{MinorVersion}.{version.Build}.0";

            return true;
        }
    }
}