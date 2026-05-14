using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services.Survey.Data;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript
{
    public static class ObjectDiffBuilder
    {
        private const string Indent = "    ";

        private static readonly HashSet<string> BvCallEntityFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            nameof(BvCallEntity.CallState),
            nameof(BvCallEntity.ShiftID),
            nameof(BvCallEntity.TimeInShift),
            nameof(BvCallEntity.TimeToExpire),
            nameof(BvCallEntity.ApptID),
            nameof(BvCallEntity.Priority),
            nameof(BvCallEntity.Resource),
            nameof(BvCallEntity.ResourceType),
            nameof(BvCallEntity.DialTypeId)
        };

        private struct DiffMember
        {
            public string Name { get; }
            public Func<object, object> Getter { get; }
            public MemberInfo Member { get; }
            public DiffMember(string name, Func<object, object> getter, MemberInfo member)
            {
                Name = name;
                Getter = getter;
                Member = member;
            }
        }

        private static readonly ConcurrentDictionary<Type, List<DiffMember>> MemberCache = new ConcurrentDictionary<Type, List<DiffMember>>();

        private static readonly Dictionary<(string, string), string> Overrides = new Dictionary<(string, string),string>
        {
            {("TimeInShift", "1899-12-30 00:00:00"), "[Now]"},
            {("TimeInShift", "null"), "[Now]"},
            {("LastCallTime", "1899-12-30 00:00:00"), "-"},
            {("TimeToExpire", "9999-01-01 00:00:00"), "[Never]"},
            {("TimeToExpire", "null"), "[Never]"},
            {("ShiftID", "-2147483648"), "[None]"},
            {("CallState", "-2"), "[Sent to dialer]"},
            {("CallState", "-1"), "[In progress]"},
            {("CallState", "0"), "[Soft deleted]"},
            {("CallState", "1"), "[Disabled by quota]"},
            {("CallState", "2"), "[Scheduled]"},
            {("CallState", "3"), "[Disabled]"}
        };

        /// <summary>
        /// Compares two objects of the same type and returns a string listing all fields and properties with different values.
        /// </summary>
        /// <typeparam name="T">The type of the objects to compare.</typeparam>
        /// <param name="oldObj">The original object.</param>
        /// <param name="newObj">The modified object.</param>
        /// <returns>A string listing the fields/properties that have different values.</returns>
        public static string GetDiff<T>(T oldObj, T newObj)
        {
            if (oldObj == null && newObj != null)
                return BuildCreatedDiff(newObj);
            if (oldObj != null && newObj == null)
                return Indent + "deleted";
            if (oldObj == null && newObj == null)
                return Indent + "not modified";
            return BuildFieldAndPropertyDiff(oldObj, newObj);
        }

        /// <summary>
        /// Logs all changed fields in a SurveyDataRowCache instance using its OriginalFieldValues and current values.
        /// </summary>
        /// <param name="cache">The SurveyDataRowCache instance.</param>
        /// <returns>A string listing all changed fields in the format: field: old → new. Returns "" if no changes.</returns>
        public static string GetDiff(SurveyDataRowCache cache)
        {
            if (cache == null)
                return "";

            var lines = new List<string>();
            foreach (var kvp in cache.OriginalFieldValues)
            {
                var field = kvp.Key;
                var oldValue = kvp.Value;
                var newValue = cache.GetFieldValue(field);
                var fieldName = GetFieldName(field, cache.LoopPath, cache.LoopQualifyer);
                if (!object.Equals(oldValue, newValue))
                {
                    lines.Add(FormatDiffLine(fieldName, oldValue, newValue));
                }
            }
            return string.Join(Environment.NewLine, lines);
        }

        private static IEnumerable<DiffMember> GetRelevantMembers(Type type)
        {
            return MemberCache.GetOrAdd(type, t =>
            {
                var onlyFields = t == typeof(BvCallEntity) ? BvCallEntityFields : null;
                var members = new List<DiffMember>();
                foreach (var field in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (onlyFields != null && !onlyFields.Contains(field.Name))
                        continue;
                    members.Add(new DiffMember(field.Name, obj => field.GetValue(obj), field));
                }
                foreach (var prop in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!prop.CanRead || prop.GetIndexParameters().Length > 0)
                        continue;
                    if (onlyFields != null && !onlyFields.Contains(prop.Name))
                        continue;
                    members.Add(new DiffMember(prop.Name, obj => prop.GetValue(obj, null), prop));
                }
                return members;
            });
        }

        private static string BuildCreatedDiff<T>(T newObj)
        {
            var type = typeof(T);
            var lines = new List<string> { Indent + "created" };
            foreach (var member in GetRelevantMembers(type))
            {
                lines.Add(FormatCreatedLine(member.Name, member.Getter(newObj)));
            }
            return string.Join(Environment.NewLine, lines);
        }

        private static string BuildFieldAndPropertyDiff<T>(T oldObj, T newObj)
        {
            var type = typeof(T);
            var lines = new List<string>();
            foreach (var member in GetRelevantMembers(type))
            {
                var oldValue = member.Getter(oldObj);
                var newValue = member.Getter(newObj);
                if (!object.Equals(oldValue, newValue) && FormatValue(oldValue, member.Name) != FormatValue(newValue, member.Name))
                {
                    lines.Add(FormatDiffLine(member.Name, oldValue, newValue));
                }
            }
            return lines.Count == 0 ? Indent + "not modified" : string.Join(Environment.NewLine, lines);
        }

        private static string FormatDiffLine(string name, object oldValue, object newValue)
            => Indent + $"{name}: {FormatValue(oldValue, name)} → {FormatValue(newValue, name)}";

        private static string FormatCreatedLine(string name, object value)
            => Indent + $"{name}: {FormatValue(value, name)}";

        private static string GetFieldName(string field, string[] cacheLoopPath, string[] cacheLoopQualifyer)
        {
            if (cacheLoopPath == null || cacheLoopPath.Length == 0)
                return field;
            var loops = string.Join(".", cacheLoopPath.Select((x,i) => $"{x}({cacheLoopQualifyer[i]})"));
            return $"{loops}.{field}";
        }

        private static string FormatValue(object value, string fieldName)
        {
            var result = FormatInternal(value);
            if (Overrides.ContainsKey((fieldName, result)))
                return Overrides[(fieldName, result)];
            return result;
        }

        private static string FormatInternal(object value)
        {
            if (value == null) return "null";
            if (value is string s) return $"\"{s}\"";
            if (value is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss");
            if (value is DateTimeOffset dto) return dto.ToString("yyyy-MM-dd HH:mm:ss");
            return value.ToString();
        }
    }
}
