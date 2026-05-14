using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Confirmit.CATI.Core.ActivityLogging;

namespace Confirmit.CATI.DatabaseUpdateLibrary
{
    public class UpdateScriptInfo : IEquatable<UpdateScriptInfo>
    {
        public string Name { get; private set; }

        public string Extension { get; set; }

        public int Major { get; private set; }

        public int Minor { get; private set; }

        public string BranchName { get; private set; }

        public int ScriptNumber { get; private set; }

        public string Description { get; private set; }

        public DateTime ScriptAppliedDate { get; private set; }

        public int Duration { get; private set; }

        public string ScriptText { get; set; }

        public string ScriptOutput { get; private set; }

        public bool IsAppliedDuringDBCreation { get; private set; }

        public string DbUpateUtilityVersion { get; private set; }

        public string ActiveUser { get; private set; }

        public bool HasSqlScriptUnsafeType { get; private set; }

        private static readonly Regex NamePattern = new Regex(@"^(?<unsafe>U)?(?<folder>.*)\\(?<name>[^ ]*)\.(?<extension>sql|ps1) (?<description>.*)$");

        public UpdateScriptInfo(string line)
        {
            var match = NamePattern.Match(line);
            if (!match.Success)
            {
                throw new Exception($"Wrong file description, which us not matched parse pattern: {NamePattern}");
            }
            var desc =
                $"unsafe={match.Groups["unsafe"].Success} folder={match.Groups["folder"]} name={match.Groups["name"]} extension={match.Groups["extension"]} desc={match.Groups["description"]}";

            bool isUnsafeScript = match.Groups["unsafe"].Success;

            string name = "_" + match.Groups["name"].Value.Replace('.', '_');
            string extension = match.Groups["extension"].Value;
            string description = match.Groups["description"].Value;

            FillParameters(name, extension, description, isUnsafeScript, DateTime.Now, 0, string.Empty, string.Empty, false, string.Empty, string.Empty);
        }

        public UpdateScriptInfo(string name, string description, bool isUnsafe)
        {
            FillParameters(name, "sql", description, isUnsafe, DateTime.Now, 0, string.Empty, string.Empty, false, string.Empty, string.Empty);
        }

        public UpdateScriptInfo(
            string name, string extension, string description, bool isUnsafe, DateTime scriptAppliedDate, int duration, string scriptText, 
            string scriptOutput, bool isAppliedDuringDBCreation, string dbUpateUtilityVersion, string activeUser)
        {
            FillParameters(name, extension, description, isUnsafe, scriptAppliedDate, duration, scriptText, scriptOutput, isAppliedDuringDBCreation, dbUpateUtilityVersion, activeUser);
        }

        private void FillParameters(string name, string extension, string description, bool isUnsafe, DateTime scriptAppliedDate, int duration, string scriptText,
            string scriptOutput, bool isAppliedDuringDBCreation, string dbUpateUtilityVersion, string activeUser)
        {
            Name = name;
            Extension = extension;
            Description = description;
            ScriptAppliedDate = scriptAppliedDate;
            Duration = duration;
            ScriptText = scriptText;
            ScriptOutput = scriptOutput;
            IsAppliedDuringDBCreation = isAppliedDuringDBCreation;
            DbUpateUtilityVersion = dbUpateUtilityVersion;
            ActiveUser = activeUser;

            string[] nameParts = name.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            int major;
            if (int.TryParse(nameParts[0], out major))
            {
                Major = major;
                Minor = Convert.ToInt32(nameParts[1]);
                BranchName = nameParts[2];
                ScriptNumber = Convert.ToInt32(nameParts[3]);
            }
            else
            {
                Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                Major = assemblyVersion.Major;
                Minor = assemblyVersion.Minor;
                ScriptNumber = -1;
                BranchName = string.Empty;
            }

            HasSqlScriptUnsafeType = isUnsafe;
        }

        public DatabaseUpdateScriptApplyingParameters ToDatabaseUpdateScriptApplyingParameters()
        {
            return new DatabaseUpdateScriptApplyingParameters
            {
                Major = Major,
                Minor = Minor,
                BranchName = BranchName,
                ScriptNumber = ScriptNumber,

                Description = Description,
                IsAppliedDuringDBCreation = IsAppliedDuringDBCreation,
                ScriptOutput = ScriptOutput,
                ScriptText = ScriptText
            };
        }

        public bool Equals(UpdateScriptInfo other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Name.Equals(other.Name);
        }

        public override string ToString()
        {
            var unsafeScript = HasSqlScriptUnsafeType ? ", unsafe" : String.Empty;
            return $"{Name}.{Extension}{unsafeScript}";
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}