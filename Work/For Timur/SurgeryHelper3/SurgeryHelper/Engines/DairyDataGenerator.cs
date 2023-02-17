using System;

namespace SurgeryHelper.Engines
{
    public class DairyDataGenerator
    {
        private Random _rand;
        private int _age;
        private string _nosologyDairyInfo;

        public DairyDataGenerator(int age, string nosologyDairyInfo)
        {
            _rand = new Random();
            _age = age;

            if (!string.IsNullOrEmpty(nosologyDairyInfo))
            {
                _nosologyDairyInfo = "\r\n" + nosologyDairyInfo;
            }
            else
            {
                _nosologyDairyInfo = string.Empty;
            }
        }

        /// <summary>
        /// Вернуть случайную температуру тела от 36.4 до 36.8
        /// </summary>
        /// <returns></returns>
        public string GetRandomTemperature()
        {            
            return "\t\t\t\t\t\t\t\tТемпература тела - 36." + _rand.Next(4, 9);
        }

        /// <summary>
        /// Вернуть текст для дневника со случайным пульсом, давлением и ЧДД в зависимости от возраста пациента
        /// </summary>
        /// <returns></returns>
        public string GetDairyText(int operationDairyDay)
        {
            string complaintsText;
            switch (operationDairyDay)
            {
                case 1:
                    complaintsText = "Жалобы на выраженные боли в области операции.";
                    break;
                case 2:
                    complaintsText = "Жалобы на умеренные боли в области операции.";
                    break;
                case 3:
                    complaintsText = "Жалобы на слабо выраженные боли в области операции.";
                    break;
                default:
                    complaintsText = "Жалоб нет.";
                    break;
            }

            return $@"
Состояние удовлетворительное.
{complaintsText}
Пульс {GetRandomPulse()} в мин., ЧДД {GetRandomBreathingRate()} в мин.АД {GetRandomPressure()} мм рт. ст. Кожный покров и видимые слизистые чистые. Катаральных явлений нет. В легких дыхание везикулярное, хрипов нет. Тоны сердца ясные, ритм правильный. Язык влажный. Живот мягкий, безболезненный. Перистальтика выслушивается. Стул регулярный, без особенностей. Мочеиспускание самостоятельно, диурез адекватный, моча обычной окраски.{_nosologyDairyInfo}";
        }

        private string GetRandomPulse()
        {
            if (_age < 1)
                return _rand.Next(115, 151).ToString();

            if (_age < 3)
                return _rand.Next(110, 121).ToString();

            if (_age < 5)
                return _rand.Next(100, 116).ToString();

            if (_age < 12)
                return _rand.Next(85, 91).ToString();

            return _rand.Next(66, 86).ToString();
        }

        private string GetRandomPressure()
        {
            if (_age < 1)
                return $"{_rand.Next(90, 96)}/{_rand.Next(44, 51)}";

            if (_age < 3)
                return $"{_rand.Next(95, 106)}/{_rand.Next(49, 66)}";

            if (_age < 5)
                return $"{_rand.Next(95, 111)}/{_rand.Next(54, 71)}";

            if (_age < 12)
                return $"{_rand.Next(100, 121)}/{_rand.Next(64, 78)}";

            return $"{_rand.Next(110, 136)}/{_rand.Next(69, 86)}";
        }
        
        private string GetRandomBreathingRate()
        {
            if (_age < 1)
                return _rand.Next(40, 53).ToString();

            if (_age < 3)
                return _rand.Next(25, 37).ToString();

            if (_age < 5)
                return _rand.Next(22, 31).ToString();

            if (_age < 12)
                return _rand.Next(16, 25).ToString();

            return _rand.Next(15, 19).ToString();
        }
    }
}
