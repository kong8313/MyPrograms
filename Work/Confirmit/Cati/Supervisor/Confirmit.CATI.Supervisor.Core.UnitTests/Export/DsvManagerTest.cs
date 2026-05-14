using System;

using Confirmit.CATI.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests.Export
{
    [TestClass]
    public class DsvManagerTest
    {
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ExportToDsv_ExportDataToDSVWithSomeDelimiter_ExportSuccess()
        {
            const string delimiter = "--";
            var collection = new []
                 {
                     new {ID = 1,   Name = "Sergey",      SurName = "Chistyakov",   ObjectEntity = new object()},
                     new {ID = 2,   Name = "Maxim",       SurName = "Lipatov",      ObjectEntity = new object()},
                     new {ID = 3,   Name = "Alexander",   SurName = "Zhigalov",     ObjectEntity = new object()},
                     new {ID = 4,   Name = "Alexander",   SurName = "Lukyanov",     ObjectEntity = new object()},
                     new {ID = 5,   Name = "Alexander",   SurName = "Melnikov",     ObjectEntity = new object()},
                     new {ID = 6,   Name = "Svetlana",    SurName = "Tyurina",      ObjectEntity = new object()},
                     new {ID = 7,   Name = "NewPerson",   SurName = String.Empty,   ObjectEntity = new object()}
                 };

            var dsvString = DsvManager.ExportToDsv(collection, delimiter, x => new[] { x.ID, x.SurName, x.ObjectEntity });

            string expectedString = String.Format(
                "1{0}Chistyakov{0}{1}{2}2{0}Lipatov{0}{1}{2}3{0}Zhigalov{0}{1}{2}4{0}Lukyanov{0}{1}{2}5{0}Melnikov{0}{1}{2}6{0}Tyurina{0}{1}{2}7{0}{0}{1}",
                delimiter,
                new object(),
                Environment.NewLine);

            Assert.AreEqual(expectedString, dsvString);
        }
    }
}
