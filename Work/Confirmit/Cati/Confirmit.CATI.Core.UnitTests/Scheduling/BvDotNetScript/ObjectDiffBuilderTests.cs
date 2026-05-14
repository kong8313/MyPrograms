using System.Data;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript;
using Confirmit.CATI.Core.Services.Survey.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Scheduling.BvDotNetScript
{
    [TestClass]
    public class ObjectDiffBuilderTests
    {
        private class TestClass
        {
            public int NumberField;
            public string StringField;
            public string StringProp { get; set; }
            public int IntProp { get; set; }
            public string NullProp { get; set; }
            public string this[int i] { get => "indexer"; set { } } // Should be ignored
        }

        [TestMethod]
        public void GetDiff_FieldsAndProperties_DetectsDifferences()
        {
            var a = new TestClass { NumberField = 1, StringField = "A", StringProp = "X", IntProp = 10, NullProp = null };
            var b = new TestClass { NumberField = 2, StringField = "B", StringProp = "Y", IntProp = 10, NullProp = "notnull" };

            var diff = ObjectDiffBuilder.GetDiff(a, b);

            Assert.IsFalse(diff.Contains("TestClass"));
            StringAssert.Contains(diff, "NumberField: 1 → 2");
            StringAssert.Contains(diff, "StringField: \"A\" → \"B\"");
            StringAssert.Contains(diff, "StringProp: \"X\" → \"Y\"");
            StringAssert.Contains(diff, "NullProp: null → \"notnull\"");
            Assert.IsFalse(diff.Contains("IntProp")); // IntProp is same
        }

        [TestMethod]
        public void GetDiff_NoDifferences_ReturnsNotModified()
        {
            var a = new TestClass { NumberField = 5, StringField = "Same", StringProp = "Same", IntProp = 42, NullProp = null };
            var b = new TestClass { NumberField = 5, StringField = "Same", StringProp = "Same", IntProp = 42, NullProp = null };

            var diff = ObjectDiffBuilder.GetDiff(a, b);

            Assert.AreEqual("    not modified", diff);
        }

        [TestMethod]
        public void GetDiff_CreatedObject_OutputsAllFields()
        {
            var b = new TestClass { NumberField = 7, StringField = "B", StringProp = "Y", IntProp = 3, NullProp = null };
            var diff = ObjectDiffBuilder.GetDiff<TestClass>(null, b);
            StringAssert.StartsWith(diff, "    created");
            StringAssert.Contains(diff, "NumberField: 7");
            StringAssert.Contains(diff, "StringField: \"B\"");
            StringAssert.Contains(diff, "StringProp: \"Y\"");
            StringAssert.Contains(diff, "IntProp: 3");
            StringAssert.Contains(diff, "NullProp: null");
            Assert.IsFalse(diff.Contains("TestClass"));
        }

        [TestMethod]
        public void GetDiff_DeletedObject_OutputsDeleted()
        {
            var a = new TestClass { NumberField = 1 };
            var diff = ObjectDiffBuilder.GetDiff<TestClass>(a, null);
            Assert.AreEqual("    deleted", diff);
        }

        [TestMethod]
        public void GetDiff_BothNull_ReturnsNotModified()
        {
            var diff = ObjectDiffBuilder.GetDiff<TestClass>(null, null);
            Assert.AreEqual("    not modified", diff);
        }
        
        [TestMethod]
        public void GetDiff_HandlesNullAndStringFormatting()
        {
            var a = new TestClass { StringField = null };
            var b = new TestClass { StringField = "abc" };
            var diff = ObjectDiffBuilder.GetDiff(a, b);
            StringAssert.Contains(diff, "StringField: null → \"abc\"");
        }

        [TestMethod]
        public void GetDiff_SurveyDataRowCache_LogsFieldChanges()
        {
            // Setup DataTable and DataRow
            var table = new DataTable();
            table.Columns.Add("A", typeof(int));
            table.Columns.Add("B", typeof(string));
            table.Columns.Add("C", typeof(int));
            var row = table.NewRow();
            row["A"] = 1;
            row["B"] = "x";
            row["C"] = 100;
            table.Rows.Add(row);

            var cache = new SurveyDataRowCache("t", null, null, null, true, row);
            cache.SetFieldValue("form", "A", 2); // Change A: 1 -> 2
            cache.SetFieldValue("form", "B", "y"); // Change B: x -> y
            // Change C twice, only original should be logged
            cache.SetFieldValue("form", "C", 200);
            cache.SetFieldValue("form", "C", 300);

            var diff = ObjectDiffBuilder.GetDiff(cache);
            StringAssert.Contains(diff, "A: 1 → 2");
            StringAssert.Contains(diff, "B: \"x\" → \"y\"");
            StringAssert.Contains(diff, "C: 100 → 300");
            // No other fields
            Assert.IsFalse(diff.Contains("not modified"));
            Assert.IsFalse(diff.Contains("created"));
            Assert.IsFalse(diff.Contains("deleted"));
        }

        [TestMethod]
        public void GetDiff_SurveyDataRowCache_NoChanges_ReturnsEmpty()
        {
            var table = new DataTable();
            table.Columns.Add("A", typeof(int));
            var row = table.NewRow();
            row["A"] = 1;
            table.Rows.Add(row);
            var cache = new SurveyDataRowCache("t", null, null, null, true, row);
            var diff = ObjectDiffBuilder.GetDiff(cache);
            Assert.AreEqual("", diff);
        }

        [TestMethod]
        public void GetDiff_SurveyDataRowCache_WithLoopPath_AppendsLoopInfo()
        {
            var table = new DataTable();
            table.Columns.Add("A", typeof(int));
            var row = table.NewRow();
            row["A"] = 1;
            table.Rows.Add(row);
            var cache = new SurveyDataRowCache("t", "level", new[] { "Loop1", "Loop2" }, new[] { "1", "2" }, true, row);
            cache.SetFieldValue("form", "A", 2);
            var diff = ObjectDiffBuilder.GetDiff(cache);
            StringAssert.Contains(diff, "Loop1(1).Loop2(2).A: 1 → 2");
        }
    }
}
