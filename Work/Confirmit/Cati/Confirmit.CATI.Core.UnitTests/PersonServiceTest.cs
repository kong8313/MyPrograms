using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests
{
    [TestClass]
    public class PersonServiceTest
    {
        [TestMethod, Owner(@"FIRM\alm")]
        public void PersonDialerAttributesAreGeneratedCorrectly()
        {
            var random = new Random();

            var expectedLocation = random.Next(1000, int.MaxValue).ToString(CultureInfo.InvariantCulture);
            var expectedCallCenterId = random.Next(1, int.MaxValue);

            var personEntity = new BvPersonEntity
            {
                Location = expectedLocation,
                CallCenterID = expectedCallCenterId
            };

            var dialerAttributes = PersonService.GetPersonDialerAttributes(personEntity).ToDictionary(x => x.Key, x => x.Value);

            Assert.AreEqual(2, dialerAttributes.Count(), string.Format("Unexpected attributes count /// {0}", DictionaryToString(dialerAttributes)));

            string actualLocation;
            string actualCallCenterId;

            var locationAttributeIsFound = dialerAttributes.TryGetValue("Location", out actualLocation);
            var callCenterIdAttributeIsFound = dialerAttributes.TryGetValue("CallCenterId", out actualCallCenterId);


            Assert.IsTrue(locationAttributeIsFound, string.Format("[Location] attribute is not found /// {0}", DictionaryToString(dialerAttributes)));
            Assert.IsTrue(callCenterIdAttributeIsFound, string.Format("[CallCenterId] attribute is not found /// {0}", DictionaryToString(dialerAttributes)));

            Assert.AreEqual(expectedLocation, actualLocation, string.Format("[Location] attribute value is incorrect /// {0}", DictionaryToString(dialerAttributes)));
            Assert.AreEqual(expectedCallCenterId.ToString(CultureInfo.InvariantCulture), actualCallCenterId, string.Format("[CallCenterId] attribute value is incorrect /// {0}", DictionaryToString(dialerAttributes)));
        }

        private static string DictionaryToString(Dictionary<string, string> dictionary)
        {
            return dictionary.Aggregate(
                new StringBuilder(), (sb, keyval) => sb.AppendFormat("['{0}' => '{1}']", keyval.Key, keyval.Value)).ToString();
        }
    }
}