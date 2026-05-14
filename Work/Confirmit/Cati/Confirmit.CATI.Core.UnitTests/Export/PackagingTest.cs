using System.IO;
using System.IO.Compression;
using Confirmit.CATI.Core.Export;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Export
{
    [TestClass]
    public class PackagingTest
    {
        [TestMethod, Owner(@"FIRM\DenisM")]
        public void CreatePackage_OneFile_PackageCreatedSuccessfully()
        {
            const string contentFileName = "content.txt";
            const string contentString = "аҧсуа Afaraf Afrikaans Akan Shqip አማርኛ العربية Aragonés Հայերեն অসমীয়া авар мацӀ, магӀарул мацӀ avesta aymar aru azərbaycan dili bamanankan башҡорт теле euskara, euskera Беларуская বাংলা भोजपुरी Bislama bosanski jezik brezhoneg български език ဗမာစာ Català Chamoru нохчийн мотт chiCheŵa, chinyanja 中文 (Zhōngwén), 汉语, 漢語 чӑваш чӗлхи Kernewek corsu, lingua corsa ᓀᐦᐃᔭᐍᐏᐣ hrvatski česky, čeština dansk ދިވެހި Nederlands, Vlaams English Esperanto eesti, eesti keel Eʋegbe føroyskt vosa Vakaviti suomi, suomen kieli français, langue française Fulfulde, Pulaar, Pular Galego ქართული Deutsch Ελληνικά Avañeẽ ગુજરાતી Kreyòl ayisyen Hausa, هَوُسَ עברית Otjiherero हिन्दी, हिंदी Hiri Motu Magyar Interlingua Bahasa Indonesia Originally called Occidental; then Interlingue after WWII Gaeilge Asụsụ Igbo Iñupiaq, Iñupiatun Ido Íslenska Italiano ᐃᓄᒃᑎᑐᑦ 日本語 (にほんご／にっぽんご) basa Jawa kalaallisut, kalaallit oqaasii ಕನ್ನಡ Kanuri कश्मीरी, كشميري‎ Қазақ тілі ភាសាខ្មែរ Gĩkũyũ Ikinyarwanda кыргыз тили коми кыв KiKongo 한국어 (韓國語), 조선말 (朝鮮語) Kurdî, كوردی‎ Kuanyama latine, lingua latina Lëtzebuergesch Luganda Limburgs Lingála ພາສາລາວ lietuvių kalba latviešu valoda Gaelg, Gailck македонски јазик Malagasy fiteny bahasa Melayu, بهاس ملايو‎ മലയാളം Malti te reo Māori मराठी Kajin M̧ajeļ монгол Ekakairũ Naoero Diné bizaad, Dinékʼehǰí Norsk bokmål isiNdebele नेपाली Owambo Norsk nynorsk Norsk ꆈꌠ꒿ Nuosuhxop isiNdebele Occitan ᐊᓂᔑᓈᐯᒧᐎᓐ ѩзыкъ словѣньскъ Afaan Oromoo ଓଡ଼ିଆ ирон æвзаг ਪੰਜਾਬੀ, پنجابی‎ पाऴि فارسی polski پښتو Português Runa Simi, română русский язык संस्कृतम् sardu सिन्धी, سنڌي، سندھی‎ Davvisámegiella gagana faa Samoa yângâ tî sängö српски језик Gàidhlig chiShona සිංහල slovenčina slovenščina Soomaaliga, af Soomaali Sesotho español, castellano Basa Sunda Kiswahili SiSwati svenska தமிழ் తెలుగు тоҷикӣ, toğikī, تاجیکی‎ ไทย ትግርኛ བོད་ཡིག Türkmen, Түркмен Wikang Tagalog, ᜏᜒᜃᜅ᜔ ᜆᜄᜎᜓᜄ᜔ Setswana faka Tonga Türkçe Xitsonga татарча, tatarça, تاتارچا‎ Twi Reo Tahiti Uyƣurqə, ئۇيغۇرچە‎ українська اردو zbek, Ўзбек, أۇزبېك‎ Tshivenḓa Tiếng Việt Volapük Walon Cymraeg Wollof Frysk isiXhosa ייִדיש Yorùbá Saɯ cueŋƅ, Saw cuengh";

            string packageFileName = new Packaging().CreatePackage(contentFileName, contentString);

            using (var archive = ZipFile.OpenRead(packageFileName))
            {
                var entry = archive.Entries[0];

                var stream = entry.Open();
                var streamReader = new StreamReader(stream);

                Assert.AreEqual(contentString, streamReader.ReadToEnd());
            } 
        }
    }
}
