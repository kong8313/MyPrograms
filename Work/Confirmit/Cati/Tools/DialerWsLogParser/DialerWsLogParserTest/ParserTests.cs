using System.Collections.Generic;
using DialerWsLogParserLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerWsLogParserTest
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseEventsAndGroups_Empty()
        {
            var text = new List<string>();

            var parser = new Parser();
            parser.ParseEventsAndGroups(text, new ParseSettings());

            Assert.AreEqual(parser.Events.Count, 0);
        }

        [TestMethod]
        public void SetSettings()
        {
            var parser = new Parser();
            var settings = new Settings();

            settings.SetColumnsFilter("A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A");

            parser.SetSettings(settings);
            Assert.AreEqual(parser.ParserSettings, settings);
        }

        [TestMethod]
        public void ResetEvents()
        {
            var parser = new Parser();

            var sourse = new List<string>
            {
                "DialerService Information: 0 : +3 2019 - 06 - 25 09:26:51.845   DialerService.ctor[rid = 0] DialerService object is created.Settings:" +
                " [StatefulMode=False, DialerId=1, UseAuthorization=False, ServiceStateExpirationTimeout=200]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.869	DialerService.ConfigureSecurityProtocols[rid = 0]" +
                " Default host security protocols: [Tls, Tls11, Tls12]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.883	DialerService.ConfigureSecurityProtocols[rid = 0]" +
                "	'SecurityProtocols' section in web.config is empty or does not exist."
            };

            parser.ParseEventsAndGroups(sourse, new ParseSettings());

            parser.Reset();

            Assert.AreEqual(parser.Events.Count, 0);
        }

        [TestMethod]
        public void ResetEventsGroups()
        {
            var parser = new Parser();

            var sourse = new List<string>
            {
                "DialerService Information: 0 : +3 2019 - 06 - 25 09:26:51.845   DialerService.ctor[rid = 0] DialerService object is created.Settings:" +
                " [StatefulMode=False, DialerId=1, UseAuthorization=False, ServiceStateExpirationTimeout=200]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.869	DialerService.ConfigureSecurityProtocols[rid = 0]" +
                " Default host security protocols: [Tls, Tls11, Tls12]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.883	DialerService.ConfigureSecurityProtocols[rid = 0]" +
                "	'SecurityProtocols' section in web.config is empty or does not exist."
            };

            parser.ParseEventsAndGroups(sourse, new ParseSettings());

            parser.Reset();

            Assert.AreEqual(parser.EventsGroups.Count, 0);
        }

        [TestMethod]
        public void ResetFilteredEventsGroups()
        {
            var parser = new Parser();

            var sourse = new List<string>
            {
                "DialerService Information: 0 : +3 2019 - 06 - 25 09:26:51.845   DialerService.ctor[rid = 0] DialerService object is created.Settings:" +
                " [StatefulMode=False, DialerId=1, UseAuthorization=False, ServiceStateExpirationTimeout=200]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.869	DialerService.ConfigureSecurityProtocols[rid = 0]" +
                " Default host security protocols: [Tls, Tls11, Tls12]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.883	DialerService.ConfigureSecurityProtocols[rid = 0]" +
                "	'SecurityProtocols' section in web.config is empty or does not exist."
            };

            parser.ParseEventsAndGroups(sourse, new ParseSettings());
            parser.FillFilteredEventsGroups("DialerService.ctor", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty, string.Empty);

            parser.Reset();

            Assert.AreEqual(parser.FilteredEventsGroups.Count, 0);
        }

        [TestMethod]
        public void ResetMatchingCondition()
        {
            var parser = new Parser();

            var sourse = new List<string>
            {
                "DialerService Information: 0 : +3 2019 - 06 - 25 09:26:51.845   DialerService.ctor [rid=0] DialerService object is created.Settings:" +
                " [StatefulMode=False, DialerId=1, UseAuthorization=False, ServiceStateExpirationTimeout=200]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.869	DialerService.ConfigureSecurityProtocols    [rid=0]" +
                " Default host security protocols: [Tls, Tls11, Tls12]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.883	DialerService.ConfigureSecurityProtocols    [rid=0]" +
                "	'SecurityProtocols' section in web.config is empty or does not exist."
            };

            parser.ParseEventsAndGroups(sourse, new ParseSettings());
            parser.FillFilteredEventsGroups(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty, "DialerService");

            parser.ResetMatchingCondition();

            foreach(var e in parser.Events)
                Assert.AreEqual(e.IsHighlighted, false);
        }

        [TestMethod]
        public void ParseEventsAndGroups_Standart()
        {
            var parser = new Parser();

            var sourse = new List<string>
            {
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.845	DialerService.ctor [rid=0] DialerService object is created.Settings:" +
                " [StatefulMode=False, DialerId=1, UseAuthorization=False, ServiceStateExpirationTimeout=200]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.869	DialerService.ConfigureSecurityProtocols    [rid=1]" +
                " Default host security protocols: [Tls, Tls11, Tls12]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.883	DialerService.ConfigureSecurityProtocols    [rid=1]" +
                "	'SecurityProtocols' section in web.config is empty or does not exist."
            };

            parser.ParseEventsAndGroups(sourse, new ParseSettings());
            
            Assert.AreEqual(parser.EventsGroups.Count, 2);
        }

        [TestMethod]
        public void ParseEventsAndGroups_SourseEmpty()
        {
            var parser = new Parser();
            var sourse = new List<string>();

            parser.ParseEventsAndGroups(sourse, new ParseSettings());

            Assert.AreEqual(parser.EventsGroups.Count, 0);
        }

        [TestMethod]
        public void ParseEventsAndGroups_AllEventsInOneGroup()
        {
            var parser = new Parser();

            var sourse = new List<string>
            {
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.845	DialerService.ctor [rid=0] DialerService object is created.Settings:" +
                " [StatefulMode=False, DialerId=1, UseAuthorization=False, ServiceStateExpirationTimeout=200]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.869	DialerService.ConfigureSecurityProtocols    [rid=0]" +
                " Default host security protocols: [Tls, Tls11, Tls12]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.883	DialerService.ConfigureSecurityProtocols    [rid=0]" +
                "	'SecurityProtocols' section in web.config is empty or does not exist."
            };

            parser.ParseEventsAndGroups(sourse, new ParseSettings());

            Assert.AreEqual(parser.Events.Count, 3);
            Assert.AreEqual(parser.EventsGroups.Count, 1);
        }

        [TestMethod]
        public void FillFilteredEventsGroups_Standart()
        {
            var parser = new Parser();

            var sourse = new List<string>
            {
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.845	DialerService.ctor [rid=0] DialerService object is created.Settings:" +
                " [StatefulMode=False, DialerId=1, UseAuthorization=False, ServiceStateExpirationTimeout=200]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.869	DialerService.ConfigureSecurityProtocols    [rid=1]" +
                " Default host security protocols: [Tls, Tls11, Tls12]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.883	DialerService.ConfigureSecurityProtocols    [rid=1]" +
                "	'SecurityProtocols' section in web.config is empty or does not exist."
            };

            parser.ParseEventsAndGroups(sourse, new ParseSettings());
            parser.FillFilteredEventsGroups("DialerService.ctor", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty, string.Empty);

            Assert.AreEqual(parser.FilteredEventsGroups[0].Name, "DialerService.ctor");
        }

        [TestMethod]
        public void FillFilteredEventsGroups_AllFiltersEmpty()
        {
            var parser = new Parser();

            var sourse = new List<string>
            {
                "DialerService Information: 0 : +3 2019 - 06 - 25 09:26:51.845   DialerService.ctor [rid=0] DialerService object is created.Settings:" +
                " [StatefulMode=False, DialerId=1, UseAuthorization=False, ServiceStateExpirationTimeout=200]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.869	DialerService.ConfigureSecurityProtocols    [rid=0]" +
                " Default host security protocols: [Tls, Tls11, Tls12]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.883	DialerService.ConfigureSecurityProtocols    [rid=0]" +
                "	'SecurityProtocols' section in web.config is empty or does not exist."
            };

            parser.ParseEventsAndGroups(sourse, new ParseSettings());
            parser.FillFilteredEventsGroups(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty, string.Empty);

            CollectionAssert.AreEqual(parser.FilteredEventsGroups, parser.EventsGroups);
        }

        [TestMethod]
        public void FillFilteredEventsGroups_NoEventsInFilteredEventsGroups()
        {
            var parser = new Parser();

            var sourse = new List<string>
            {
                "DialerService Information: 0 : +3 2019 - 06 - 25 09:26:51.845   DialerService.ctor [rid=0] DialerService object is created.Settings:" +
                " [StatefulMode=False, DialerId=1, UseAuthorization=False, ServiceStateExpirationTimeout=200]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.869	DialerService.ConfigureSecurityProtocols    [rid=0]" +
                " Default host security protocols: [Tls, Tls11, Tls12]",
                "DialerService Information: 0 : +3 2019-06-25 09:26:51.883	DialerService.ConfigureSecurityProtocols    [rid=0]" +
                "	'SecurityProtocols' section in web.config is empty or does not exist."
            };

            parser.ParseEventsAndGroups(sourse, new ParseSettings());
            parser.FillFilteredEventsGroups("gdganshdag", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty, string.Empty);

            CollectionAssert.AreEqual(parser.FilteredEventsGroups, new List<EventsGroup>());
        }
    }
}
