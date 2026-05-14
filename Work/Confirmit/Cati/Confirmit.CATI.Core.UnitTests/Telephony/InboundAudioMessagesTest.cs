using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Inbound;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.UnitTests.Telephony
{
    [TestClass]
    public class InboundAudioMessagesTest
    {
        [TestMethod, Owner(@"FIRM\alm")]
        public void TheBothAudioCollectionsAreNull_DdiNumbersMessages_EmptyCollectionIsReturned()
        {
            var stubIDialerSettings = new StubIDialerSettings();
            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity();

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var actual = target.DdiNumbersMessages(telephoneNumberEntity);

            Assert.AreEqual(0, actual.Count(), "The collection is expected to be empty, but Count() is not equals to zero.");
        }

        [Ignore] // Temporary ignored as deserialization of {\"IncomingCall\":null} json does not work as expected
        [TestMethod, Owner(@"FIRM\alm")]
        public void TheBothAudioCollectionsContainNulls_DdiNumbersMessages_EmptyCollectionIsReturned()
        {
            var stubIDialerSettings = new StubIDialerSettings
            {
                GetInboundAudioMessageAudioMessageType = type => null
            };

            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity()
            {
                AudioMessagesJson = "{\"IncomingCall\":null,\"TimedOut\":null,\"SystemFault\":null}"
            };

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var actual = target.DdiNumbersMessages(telephoneNumberEntity);

            Assert.AreEqual(0, actual.Count(), "The collection is expected to be empty, but Count() is not equals to zero.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SettingsAudioCollectionHasAudioWhileDdiAudioCollectionHasNot_DdiNumbersMessages_ProperCollectionIsReturned()
        {
            var stubIDialerSettings = new StubIDialerSettings
            {
                GetInboundAudioMessageAudioMessageType = type => new AudioMessageDescriptor
                {
                    Type = (AudioSourceType)type,
                    Source = type.ToString()
                }
            };

            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity();

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var expected = target.DdiNumbersMessageTypes.Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type,
                        Source = type.ToString()
                    })).ToList();

            var actual = target.DdiNumbersMessages(telephoneNumberEntity).ToList();

            Assert.AreEqual(target.DdiNumbersMessageTypes.Count, actual.Count,
                "The collection contains wrong number of items");

            foreach (var item in expected)
            {
                var found = actual.Any(y =>
                    (item.Key == y.Key)
                    && (item.Value.Type == y.Value.Type)
                    && (item.Value.Source == y.Value.Source));

                Assert.IsTrue(found,
                    "Expected item {0} is not found in actual collection." +
                    "\nExpected collection: [{1}].\nActual collection: [{2}]. ",
                    item, string.Join(", ", expected), string.Join(", ", actual));
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void DdiAudioCollectionHasAudioWhileSettingsAudioCollectionHasNot_DdiNumbersMessages_ProperCollectionIsReturned()
        {
            var random = new Random();

            var stubIDialerSettings = new StubIDialerSettings();

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var expected = target.DdiNumbersMessageTypes.Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(1, 100),
                        Source = type.ToString() + random.Next(1000, 9999),
                        RepeatCount = 0
                    })).ToList();

            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity
            {
                AudioMessagesJson = JsonConvert.SerializeObject(expected.ToDictionary(x => x.Key, x => x.Value))
            };

            var actual = target.DdiNumbersMessages(telephoneNumberEntity).ToList();

            Assert.AreEqual(target.DdiNumbersMessageTypes.Count, actual.Count,
                "The collection contains wrong number of items");

            foreach (var item in expected)
            {
                var found = actual.Any(y =>
                    (item.Key == y.Key)
                    && (item.Value.Type == y.Value.Type)
                    && (item.Value.Source == y.Value.Source));

                Assert.IsTrue(found,
                    "Expected item {0} is not found in actual collection." +
                    "\nExpected collection: [{1}].\nActual collection: [{2}]. ",
                    item, string.Join(", ", expected), string.Join(", ", actual));
            }
        }

        /// <summary>
        /// Values from "ddi audio collection" should override values from "settings audio collection"
        /// </summary>
        [TestMethod, Owner(@"FIRM\alm")]
        public void DdiAudioCollectionHasAudioAndSettingsAudioCollectionHasAudio_DdiNumbersMessages_ProperCollectionIsReturned()
        {
            var random = new Random();

            var stubIDialerSettings = new StubIDialerSettings();

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var settingsAudioCollection = target.DdiNumbersMessageTypes.Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(1, 10),
                        Source = type.ToString() + random.Next(100, 999)
                    })).ToList();

            stubIDialerSettings.GetInboundAudioMessageAudioMessageType = type =>
                settingsAudioCollection.Select(x => x).Where(y => y.Key == type).Select(z => z.Value).FirstOrDefault();

            var expected = target.DdiNumbersMessageTypes.Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(10, 100),
                        Source = type.ToString() + random.Next(1000, 9999)
                    })).ToList();

            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity
            {
                AudioMessagesJson = JsonConvert.SerializeObject(expected.ToDictionary(x => x.Key, x => x.Value))
            };

            var actual = target.DdiNumbersMessages(telephoneNumberEntity).ToList();

            Assert.AreEqual(target.DdiNumbersMessageTypes.Count, actual.Count,
                "The collection contains wrong number of items");

            foreach (var item in expected)
            {
                var found = actual.Any(y =>
                    (item.Key == y.Key)
                    && (item.Value.Type == y.Value.Type)
                    && (item.Value.Source == y.Value.Source));

                Assert.IsTrue(found,
                    "Expected item {0} is not found in actual collection." +
                    "\nExpected collection: [{1}].\nActual collection: [{2}]. ",
                    item, string.Join(", ", settingsAudioCollection), string.Join(", ", actual));
            }
        }

        /// <summary>
        /// Values from "ddi audio collection" should override values from "settings audio collection".
        /// In this case "ddi audio collection" contains not all possible message types.
        /// AudioMessageType.IncomingCall message must be taken from the "settings audio collection".
        /// </summary>
        [TestMethod, Owner(@"FIRM\alm")]
        public void DdiAudioCollectionHasSomeAudioAndSettingsAudioCollectionHasAudio_DdiNumbersMessages_ProperCollectionIsReturned()
        {
            var random = new Random();

            var stubIDialerSettings = new StubIDialerSettings();

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var settingsAudioCollection = target.DdiNumbersMessageTypes.Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(1, 10),
                        Source = type.ToString() + random.Next(100, 999)
                    })).ToList();

            stubIDialerSettings.GetInboundAudioMessageAudioMessageType = type =>
                settingsAudioCollection.Select(x => x).Where(y => y.Key == type).Select(z => z.Value).FirstOrDefault();

            var ddiAudioCollection = target.DdiNumbersMessageTypes.Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(10, 100),
                        Source = type.ToString() + random.Next(1000, 9999)
                    })).Where(x => x.Key != AudioMessageType.IncomingCall).ToList();

            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity
            {
                AudioMessagesJson = JsonConvert.SerializeObject(
                    ddiAudioCollection.ToDictionary(x => x.Key, x => x.Value))
            };

            var actual = target.DdiNumbersMessages(telephoneNumberEntity).ToList();

            Assert.AreEqual(target.DdiNumbersMessageTypes.Count, actual.Count,
                "The collection contains wrong number of items");

            var expected = ddiAudioCollection
                .Concat(settingsAudioCollection.Select(x => x)
                .Where(y => y.Key == AudioMessageType.IncomingCall));

            foreach (var item in expected)
            {
                var found = actual.Any(y =>
                    (item.Key == y.Key)
                    && (item.Value.Type == y.Value.Type)
                    && (item.Value.Source == y.Value.Source));

                Assert.IsTrue(found,
                    "Expected item {0} is not found in actual collection." +
                    "\nExpected collection: [{1}].\nActual collection: [{2}]. ",
                    item, string.Join(", ", settingsAudioCollection), string.Join(", ", actual));
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void TheBothAudioCollectionsAreNull_Get_NullObjectsAreReturned()
        {
            var stubIDialerSettings = new StubIDialerSettings();

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity();

            foreach (AudioMessageType messageType in Enum.GetValues(typeof(AudioMessageType)))
            {
                var actual = target.Get(telephoneNumberEntity, messageType);

                Assert.IsNull(actual,
                    "AudioMessageDescriptor for type [{0}] is not as expected." +
                    "\nExpected: [{1}].\nActual: [{2}].",
                    messageType, null, actual);
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SettingsAudioCollectionHasAudioWhileDdiAudioCollectionHasNot_Get_ProperObjectsAreReturned()
        {
            var random = new Random();

            var stubIDialerSettings = new StubIDialerSettings();

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var expectedCollection = Enum.GetValues(typeof(AudioMessageType)).Cast<AudioMessageType>().Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(10, 100),
                        Source = type.ToString() + random.Next(1000, 9999)
                    })).ToList();

            stubIDialerSettings.GetInboundAudioMessageAudioMessageType = type =>
                expectedCollection.Select(x => x).Where(y => y.Key == type).Select(z => z.Value).FirstOrDefault();

            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity();

            foreach (var expected in expectedCollection)
            {
                var actual = target.Get(telephoneNumberEntity, expected.Key);

                var areEqual = (expected.Value.Type == actual.Type) && (expected.Value.Source == actual.Source);

                Assert.IsTrue(areEqual,
                    "AudioMessageDescriptor for type [{0}] is not as expected." +
                    "\nExpected: [{1}].\nActual: [{2}].",
                    expected.Key, expected.Value, actual);
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void DdiAudioCollectionHasAudioWhileSettingsAudioCollectionHasNot_Get_ProperObjectsAreReturned()
        {
            var random = new Random();

            var stubIDialerSettings = new StubIDialerSettings();

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var expectedCollection = Enum.GetValues(typeof(AudioMessageType)).Cast<AudioMessageType>().Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(10, 100),
                        Source = type.ToString() + random.Next(1000, 9999),
                        RepeatCount = 0
                    })).ToList();

            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity
            {
                AudioMessagesJson = JsonConvert.SerializeObject(expectedCollection.ToDictionary(x => x.Key, x => x.Value))
            };

            foreach (var expected in expectedCollection)
            {
                var actual = target.Get(telephoneNumberEntity, expected.Key);

                var areEqual = (expected.Value.Type == actual.Type) && (expected.Value.Source == actual.Source);

                Assert.IsTrue(areEqual,
                    "AudioMessageDescriptor for type [{0}] is not as expected." +
                    "\nExpected: [{1}].\nActual: [{2}].",
                    expected.Key, expected.Value, actual);
            }
        }

        /// <summary>
        /// Values from "ddi audio collection" should override values from "settings audio collection"
        /// </summary>
        [TestMethod, Owner(@"FIRM\alm")]
        public void DdiAudioCollectionHasAudioAndSettingsAudioCollectionHasAudio_Get_ProperObjectsAreReturned()
        {
            var random = new Random();

            var stubIDialerSettings = new StubIDialerSettings();

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var settingsAudioCollection = target.DdiNumbersMessageTypes.Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(1, 10),
                        Source = type.ToString() + random.Next(100, 999),
                        RepeatCount = random.Next(0, 2)
                    })).ToList();

            stubIDialerSettings.GetInboundAudioMessageAudioMessageType = type =>
                settingsAudioCollection.Select(x => x).Where(y => y.Key == type).Select(z => z.Value).FirstOrDefault();

            var expectedCollection = Enum.GetValues(typeof(AudioMessageType)).Cast<AudioMessageType>().Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(10, 100),
                        Source = type.ToString() + random.Next(1000, 9999),
                        RepeatCount = random.Next(3, 5)
                    })).ToList();

            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity
            {
                AudioMessagesJson = JsonConvert.SerializeObject(expectedCollection.ToDictionary(x => x.Key, x => x.Value))
            };

            foreach (var expected in expectedCollection)
            {
                var actual = target.Get(telephoneNumberEntity, expected.Key);

                var areEqual = (expected.Value.Type == actual.Type) && (expected.Value.Source == actual.Source) && (expected.Value.RepeatCount == actual.RepeatCount);

                Assert.IsTrue(areEqual,
                    "AudioMessageDescriptor for type [{0}] is not as expected." +
                    "\nExpected: [{1}].\nActual: [{2}].",
                    expected.Key, expected.Value, actual);
            }
        }

        /// <summary>
        /// Values from "ddi audio collection" should override values from "settings audio collection"
        /// But if value in "settings audio collection" is not defined should return null
        /// </summary>
        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void SettingAudioCollectionHasValueAndDdiAudioCollectionHasExceptRepeatCount_Get_AllValuesGotFromDdiAudioCollectionButRepeatCountFromSettingAudioCollection()
        {
            var random = new Random();

            var stubIDialerSettings = new StubIDialerSettings();

            var target = new InboundAudioMessages(stubIDialerSettings, null);

            var settingsAudioCollection = target.DdiNumbersMessageTypes.Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(1, 9),
                        Source = type.ToString() + random.Next(100, 999),
                        RepeatCount = 0
                    })).ToList();

            stubIDialerSettings.GetInboundAudioMessageAudioMessageType = type =>
                settingsAudioCollection.Select(x => x).Where(y => y.Key == type).Select(z => z.Value).FirstOrDefault();

            var expectedCollection = Enum.GetValues(typeof(AudioMessageType)).Cast<AudioMessageType>().Select(
                type => new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    type,
                    new AudioMessageDescriptor
                    {
                        Type = (AudioSourceType)type + random.Next(10, 100),
                        Source = type.ToString() + random.Next(1000, 9999),
                        RepeatCount = null
                    })).ToList();

            var telephoneNumberEntity = new BvInboundTelephoneNumberEntity
            {
                AudioMessagesJson = JsonConvert.SerializeObject(expectedCollection.ToDictionary(x => x.Key, x => x.Value))
            };

            foreach (var expected in expectedCollection)
            {
                var actual = target.Get(telephoneNumberEntity, expected.Key);

                var hasDefaultAudio = settingsAudioCollection.Any(x => x.Key == expected.Key);
                var defaultAudio = settingsAudioCollection.FirstOrDefault(x => x.Key == expected.Key);
                bool areEqual;

                if (hasDefaultAudio)
                {
                    var typeAndSourceNotMatchDefault = defaultAudio.Value.Type != actual.Type && defaultAudio.Value.Source != actual.Source;
                    var typeAndSourceMatchExpected = expected.Value.Type == actual.Type && expected.Value.Source == actual.Source;
                    var repeatCountNotMatchExpected = expected.Value.RepeatCount != actual.RepeatCount;
                    var repeatCountMatchDefault = defaultAudio.Value.RepeatCount == actual.RepeatCount;

                    areEqual = typeAndSourceNotMatchDefault && typeAndSourceMatchExpected &&
                               repeatCountNotMatchExpected && repeatCountMatchDefault;
                }
                else
                {
                    areEqual = actual == null;
                }

                Assert.IsTrue(areEqual,
                    "AudioMessageDescriptor for type [{0}] is not as expected." +
                    "\nExpected: [{1}].\nActual: [{2}].",
                    expected.Key, expected.Value, actual);
            }
        }

        [TestMethod, Owner(@"FIRM\olegz")]
        public void SettingAudioHasValueAndDdiAudioHasValue_Get_ProperObjectsAreReturned()
        {
            //IncomingCallMandatory: once ^ once = once
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCallMandatory)
                .SetDefault(1, "1", 0)
                .SetOverride(2, "2", 0)
                .ResultEqualsTo(2, "2", 0));

            //IncomingCallMandatory: once ^ default = once
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCallMandatory)
                .SetDefault(1, "1", 0)
                .SetOverride(2, "2", null)
                .ResultEqualsTo(2, "2", 0));

            //IncomingCallMandatory: once ^ null = once
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCallMandatory)
                .SetDefault(1, "1", 0)
                .SetOverride(null)
                .ResultEqualsTo(1, "1", 0));

            //IncomingCallMandatory: once ^ off = null
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCallMandatory)
                .SetDefault(1, "1", 0)
                .SetOverride(2, "2", -1)
                .ResultEqualsTo(null));

            //IncomingCallMandatory: off ^ off = null
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCallMandatory)
                .SetDefault(1, "1", -1)
                .SetOverride(2, "2", -1)
                .ResultEqualsTo(null));

            //IncomingCallMandatory: off ^ null = null
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCallMandatory)
                .SetDefault(1, "1", -1)
                .SetOverride(null)
                .ResultEqualsTo(null));

            //IncomingCallMandatory: off ^ once = once
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCallMandatory)
                .SetDefault(1, "1", -1)
                .SetOverride(2, "2", 0)
                .ResultEqualsTo(2, "2", 0));

            //IncomingCallMandatory: off ^ default = null
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCallMandatory)
                .SetDefault(1, "1", -1)
                .SetOverride(2, "2", null)
                .ResultEqualsTo(null));


            //IncomingCall: looping ^ default = looping
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.DropCallSystemFault)
                .SetDefault(1, "1", int.MaxValue)
                .SetOverride(2, "2", null)
                .ResultEqualsTo(2, "2", int.MaxValue));

            //IncomingCall: looping ^ off = null
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.DropCallSystemFault)
                .SetDefault(1, "1", int.MaxValue)
                .SetOverride(2, "2", -1)
                .ResultEqualsTo(null));

            //IncomingCall: looping ^ once = once
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCall)
                .SetDefault(1, "1", int.MaxValue)
                .SetOverride(2, "2", 0)
                .ResultEqualsTo(2, "2", 0));

            //IncomingCall: looping ^ looping = looping
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCall)
                .SetDefault(1, "1", int.MaxValue)
                .SetOverride(2, "2", int.MaxValue)
                .ResultEqualsTo(2, "2", int.MaxValue));

            //IncomingCall: looping ^ null = looping
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCall)
                .SetDefault(1, "1", int.MaxValue)
                .SetOverride(null)
                .ResultEqualsTo(1, "1", int.MaxValue));

            //IncomingCall: once ^ looping = looping
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCall)
                .SetDefault(1, "1", 0)
                .SetOverride(2, "2", int.MaxValue)
                .ResultEqualsTo(2, "2", int.MaxValue));

            //IncomingCall: off ^ looping = looping
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.IncomingCall)
                .SetDefault(1, "1", -1)
                .SetOverride(2, "2", int.MaxValue)
                .ResultEqualsTo(2, "2", int.MaxValue));


            //DropCallOutOfShift: null ^ once = once
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.DropCallOutOfShift)
                .SetDefault(null)
                .SetOverride(2, "2", 0)
                .ResultEqualsTo(2, "2", 0));

            //DropCallOutOfShift: null ^ looping = looping
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.DropCallOutOfShift)
                .SetDefault(null)
                .SetOverride(2, "2", int.MaxValue)
                .ResultEqualsTo(2, "2", int.MaxValue));

            //DropCallOutOfShift: null ^ default = null
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.DropCallOutOfShift)
                .SetDefault(null)
                .SetOverride(2, "2", null)
                .ResultEqualsTo(null));

            //DropCallOutOfShift: null ^ off = null
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.DropCallOutOfShift)
                .SetDefault(null)
                .SetOverride(2, "2", -1)
                .ResultEqualsTo(null));


            //DropCallCampaignNotAvailable: null ^ null = null
            Assert.IsTrue(new AudioMessageDescriptorContext(AudioMessageType.DropCallCampaignNotAvailable)
                .SetDefault(null)
                .SetOverride(null)
                .ResultEqualsTo(null));
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void FromDropCallReason_Maps_DdiRecordIsNotFound_To_DropCallInterviewNotFound()
        {
            var random = new Random();

            const AudioMessageType expectedAudioMessageType = AudioMessageType.DropCallInterviewNotFound;
            var expectedAudioSourceType = (AudioSourceType)random.Next(10, 100);
            var expectedAudioSource = expectedAudioMessageType.ToString() + random.Next(1000, 9999);

            var audioCollection = new List<KeyValuePair<AudioMessageType, AudioMessageDescriptor>>
            {
                new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    expectedAudioMessageType,
                    new AudioMessageDescriptor
                    {
                        Type = expectedAudioSourceType,
                        Source = expectedAudioSource,
                        RepeatCount = 0
                    })
            };

            var stubIDialerSettings = new StubIDialerSettings();

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = number => new BvInboundTelephoneNumberEntity
                {
                    AudioMessagesJson = JsonConvert.SerializeObject(
                        audioCollection.ToDictionary(x => x.Key, x => x.Value))
                }
            };

            var target = new InboundAudioMessages(stubIDialerSettings, stubIInboundTelephoneNumberRepository);

            var actualAudioMessageDescriptor = target.FromDropCallReason(null, DropInboundCallReason.DdiRecordIsNotFound);

            Assert.IsNotNull(actualAudioMessageDescriptor, "It's expected that AudioMessageDescriptor has to be not null.");
            Assert.AreEqual(expectedAudioSourceType, actualAudioMessageDescriptor.Type, "AudioSourceType is not as expected.");
            Assert.AreEqual(expectedAudioSource, actualAudioMessageDescriptor.Source, "AudioSource is not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void FromDropCallReason_Maps_NotAcceptedBySchedulingScript_To_DropCallInterviewNotFound()
        {
            var random = new Random();

            const AudioMessageType expectedAudioMessageType = AudioMessageType.DropCallInterviewNotFound;
            var expectedAudioSourceType = (AudioSourceType)random.Next(10, 100);
            var expectedAudioSource = expectedAudioMessageType.ToString() + random.Next(1000, 9999);

            var audioCollection = new List<KeyValuePair<AudioMessageType, AudioMessageDescriptor>>
            {
                new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    expectedAudioMessageType,
                    new AudioMessageDescriptor
                    {
                        Type = expectedAudioSourceType,
                        Source = expectedAudioSource,
                        RepeatCount = 0
                    })
            };

            var stubIDialerSettings = new StubIDialerSettings();

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = number => new BvInboundTelephoneNumberEntity
                {
                    AudioMessagesJson = JsonConvert.SerializeObject(
                        audioCollection.ToDictionary(x => x.Key, x => x.Value))
                }
            };

            var target = new InboundAudioMessages(stubIDialerSettings, stubIInboundTelephoneNumberRepository);

            var actualAudioMessageDescriptor = target.FromDropCallReason(null, DropInboundCallReason.NotAcceptedBySchedulingScript);

            Assert.IsNotNull(actualAudioMessageDescriptor, "It's expected that AudioMessageDescriptor has to be not null.");
            Assert.AreEqual(expectedAudioSourceType, actualAudioMessageDescriptor.Type, "AudioSourceType is not as expected.");
            Assert.AreEqual(expectedAudioSource, actualAudioMessageDescriptor.Source, "AudioSource is not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void FromDropCallReason_Maps_InterviewIsNotFound_To_DropCallInterviewNotFound()
        {
            var random = new Random();

            const AudioMessageType expectedAudioMessageType = AudioMessageType.DropCallInterviewNotFound;
            var expectedAudioSourceType = (AudioSourceType)random.Next(10, 100);
            var expectedAudioSource = expectedAudioMessageType.ToString() + random.Next(1000, 9999);

            var audioCollection = new List<KeyValuePair<AudioMessageType, AudioMessageDescriptor>>
            {
                new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    expectedAudioMessageType,
                    new AudioMessageDescriptor
                    {
                        Type = expectedAudioSourceType,
                        Source = expectedAudioSource,
                        RepeatCount = 0
                    })
            };

            var stubIDialerSettings = new StubIDialerSettings();

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = number => new BvInboundTelephoneNumberEntity
                {
                    AudioMessagesJson = JsonConvert.SerializeObject(
                        audioCollection.ToDictionary(x => x.Key, x => x.Value))
                }
            };

            var target = new InboundAudioMessages(stubIDialerSettings, stubIInboundTelephoneNumberRepository);

            var actualAudioMessageDescriptor = target.FromDropCallReason(null, DropInboundCallReason.InterviewIsNotFound);

            Assert.IsNotNull(actualAudioMessageDescriptor, "It's expected that AudioMessageDescriptor has to be not null.");
            Assert.AreEqual(expectedAudioSourceType, actualAudioMessageDescriptor.Type, "AudioSourceType is not as expected.");
            Assert.AreEqual(expectedAudioSource, actualAudioMessageDescriptor.Source, "AudioSource is not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void FromDropCallReason_Maps_SurveyIsNotOpened_To_DropCallCampaignNotAvailable()
        {
            var random = new Random();

            const AudioMessageType expectedAudioMessageType = AudioMessageType.DropCallCampaignNotAvailable;
            var expectedAudioSourceType = (AudioSourceType)random.Next(10, 100);
            var expectedAudioSource = expectedAudioMessageType.ToString() + random.Next(1000, 9999);

            var audioCollection = new List<KeyValuePair<AudioMessageType, AudioMessageDescriptor>>
            {
                new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    expectedAudioMessageType,
                    new AudioMessageDescriptor
                    {
                        Type = expectedAudioSourceType,
                        Source = expectedAudioSource,
                        RepeatCount = 0
                    })
            };

            var stubIDialerSettings = new StubIDialerSettings();

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = number => new BvInboundTelephoneNumberEntity
                {
                    AudioMessagesJson = JsonConvert.SerializeObject(
                        audioCollection.ToDictionary(x => x.Key, x => x.Value))
                }
            };

            var target = new InboundAudioMessages(stubIDialerSettings, stubIInboundTelephoneNumberRepository);

            var actualAudioMessageDescriptor = target.FromDropCallReason(null, DropInboundCallReason.SurveyIsNotOpened);

            Assert.IsNotNull(actualAudioMessageDescriptor, "It's expected that AudioMessageDescriptor has to be not null.");
            Assert.AreEqual(expectedAudioSourceType, actualAudioMessageDescriptor.Type, "AudioSourceType is not as expected.");
            Assert.AreEqual(expectedAudioSource, actualAudioMessageDescriptor.Source, "AudioSource is not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void FromDropCallReason_Maps_SurveyIsNotFound_To_DropCallCampaignNotAvailable()
        {
            var random = new Random();

            const AudioMessageType expectedAudioMessageType = AudioMessageType.DropCallCampaignNotAvailable;
            var expectedAudioSourceType = (AudioSourceType)random.Next(10, 100);
            var expectedAudioSource = expectedAudioMessageType.ToString() + random.Next(1000, 9999);

            var audioCollection = new List<KeyValuePair<AudioMessageType, AudioMessageDescriptor>>
            {
                new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    expectedAudioMessageType,
                    new AudioMessageDescriptor
                    {
                        Type = expectedAudioSourceType,
                        Source = expectedAudioSource,
                        RepeatCount = 0
                    })
            };

            var stubIDialerSettings = new StubIDialerSettings();

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = number => new BvInboundTelephoneNumberEntity
                {
                    AudioMessagesJson = JsonConvert.SerializeObject(
                        audioCollection.ToDictionary(x => x.Key, x => x.Value))
                }
            };

            var target = new InboundAudioMessages(stubIDialerSettings, stubIInboundTelephoneNumberRepository);

            var actualAudioMessageDescriptor = target.FromDropCallReason(null, DropInboundCallReason.SurveyIsNotFound);

            Assert.IsNotNull(actualAudioMessageDescriptor, "It's expected that AudioMessageDescriptor has to be not null.");
            Assert.AreEqual(expectedAudioSourceType, actualAudioMessageDescriptor.Type, "AudioSourceType is not as expected.");
            Assert.AreEqual(expectedAudioSource, actualAudioMessageDescriptor.Source, "AudioSource is not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void FromDropCallReason_Maps_ShiftIsNotFound_To_DropCallOutOfShift()
        {
            var random = new Random();

            const DropInboundCallReason sourceDropInboundCallReason = DropInboundCallReason.ShiftIsNotFound;
            const AudioMessageType expectedAudioMessageType = AudioMessageType.DropCallOutOfShift;

            var expectedAudioSourceType = (AudioSourceType)random.Next(10, 100);
            var expectedAudioSource = expectedAudioMessageType.ToString() + random.Next(1000, 9999);

            var audioCollection = new List<KeyValuePair<AudioMessageType, AudioMessageDescriptor>>
            {
                new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                    expectedAudioMessageType,
                    new AudioMessageDescriptor
                    {
                        Type = expectedAudioSourceType,
                        Source = expectedAudioSource,
                        RepeatCount = 0
                    })
            };

            var stubIDialerSettings = new StubIDialerSettings();

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = number => new BvInboundTelephoneNumberEntity
                {
                    AudioMessagesJson = JsonConvert.SerializeObject(
                        audioCollection.ToDictionary(x => x.Key, x => x.Value))
                }
            };

            var target = new InboundAudioMessages(stubIDialerSettings, stubIInboundTelephoneNumberRepository);

            var actualAudioMessageDescriptor = target.FromDropCallReason(null, sourceDropInboundCallReason);

            Assert.IsNotNull(actualAudioMessageDescriptor, "It's expected that AudioMessageDescriptor has to be not null.");
            Assert.AreEqual(expectedAudioSourceType, actualAudioMessageDescriptor.Type, "AudioSourceType is not as expected.");
            Assert.AreEqual(expectedAudioSource, actualAudioMessageDescriptor.Source, "AudioSource is not as expected.");
        }
    }
}