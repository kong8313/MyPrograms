﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SurgeryHelper.Engines
{
    public class ConvertEngine
    {
        /// <summary>
        /// Сравнивает две переменные типа DateTime. 
        /// Возвращает 1, если первая больше второй, -1 - если первая меньше второй, 0 - если равны
        /// </summary>
        /// <param name="dateTime1">Первая дата</param>
        /// <param name="dateTime2">Вторая дата</param>
        /// <param name="useTime">Использовать ли время при сравнении</param>
        /// <returns></returns>
        public static int CompareDateTimes(DateTime dateTime1, DateTime dateTime2, bool useTime)
        {
            if (dateTime1.Year > dateTime2.Year)
            {
                return 1;
            }
            
            if (dateTime1.Year < dateTime2.Year)
            {
                return -1;
            }

            if (dateTime1.Month > dateTime2.Month)
            {
                return 1;
            }
            
            if (dateTime1.Month < dateTime2.Month)
            {
                return -1;
            }

            if (dateTime1.Day > dateTime2.Day)
            {
                return 1;
            }
            
            if (dateTime1.Day < dateTime2.Day)
            {
                return -1;
            }

            if (!useTime)
            {
                return 0;
            }

            if (dateTime1.Hour > dateTime2.Hour)
            {
                return 1;
            }
            
            if (dateTime1.Hour < dateTime2.Hour)
            {
                return -1;
            }

            if (dateTime1.Minute > dateTime2.Minute)
            {
                return 1;
            }
            
            if (dateTime1.Minute < dateTime2.Minute)
            {
                return -1;
            }

            if (dateTime1.Second > dateTime2.Second)
            {
                return 1;
            }
            
            if (dateTime1.Second < dateTime2.Second)
            {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Преобразует объект типа DateTime в строку с датой вида 11.12.1111
        /// </summary>
        /// <param name="dateTime">Объект DateTime для преобразования</param>
        /// <returns></returns>
        public static string GetRightDateString(DateTime dateTime)
        {
            return GetRightDateString(dateTime, false);
        }

        /// <summary>
        /// Преобразует объект типа DateTime? в строку с датой вида 11.12.1111 11:11
        /// </summary>
        /// <param name="dateTime">Объект DateTime? для преобразования</param>
        /// <param name="isNeedTime">Надо ли включать в строку время</param>
        /// <returns></returns>
        public static string GetRightDateString(DateTime? dateTime, bool isNeedTime)
        {
            if (dateTime.HasValue)
            {
                return GetRightDateString(dateTime.Value, isNeedTime);
            }

            return null;
        }

        /// <summary>
        /// Преобразует объект типа DateTime в строку с датой вида 11.12.1111 11:11
        /// </summary>
        /// <param name="dateTime">Объект DateTime для преобразования</param>
        /// <param name="isNeedTime">Надо ли включать в строку время</param>
        /// <returns></returns>
        public static string GetRightDateString(DateTime dateTime, bool isNeedTime)
        {
            var res = new StringBuilder();

            res.Append(dateTime.Day.ToString("D2") + "." + dateTime.Month.ToString("D2") + "." + dateTime.Year.ToString("D4"));

            if (isNeedTime)
            {
                res.Append(" " + dateTime.Hour.ToString("D2") + ":" + dateTime.Minute.ToString("D2"));
            }

            return res.ToString();
        }

        /// <summary>
        /// Преобразует объект типа DateTime в строку со временем вида 11:11
        /// </summary>
        /// <param name="dateTime">Объект DateTime для преобразования</param>
        /// <returns></returns>
        public static string GetRightTimeString(DateTime dateTime)
        {
            return dateTime.Hour.ToString("D2") + ":" + dateTime.Minute.ToString("D2");
        }

        /// <summary>
        /// Возвращает объект DateTime из строки вида 11.12.1111 11:11 или 12/11/1111 11:11
        /// </summary>
        /// <param name="dateTimeStr">Строка с датой</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromString(string dateTimeStr)
        {
            string[] dateAndTime = dateTimeStr.Split(' ');
            if (dateAndTime.Length == 0)
            {
                throw new Exception("Строка " + dateTimeStr + " не распознана как тип DateTime");
            }

            int day, month, year, hour = 0, minut = 0;

            string[] dayMonthYear = dateAndTime[0].Split('.');
            if (dayMonthYear.Length == 3)
            {
                day = Convert.ToInt32(dayMonthYear[0]);
                month = Convert.ToInt32(dayMonthYear[1]);
                year = Convert.ToInt32(dayMonthYear[2]);
            }
            else
            {
                dayMonthYear = dateAndTime[0].Split('/');
                if (dayMonthYear.Length != 3)
                {
                    throw new Exception("Строка " + dateTimeStr + " не распознана как тип DateTime");
                }

                month = Convert.ToInt32(dayMonthYear[0]);
                day = Convert.ToInt32(dayMonthYear[1]);
                year = Convert.ToInt32(dayMonthYear[2]);
            }

            if (dateAndTime.Length == 2)
            {
                string[] hourMinut = dateAndTime[1].Split(':');
                if (dayMonthYear.Length < 2)
                {
                    throw new Exception("Строка " + dateTimeStr + " не распознана как тип DateTime");
                }

                hour = Convert.ToInt32(hourMinut[0]);
                minut = Convert.ToInt32(hourMinut[1]);
            }

            return new DateTime(year, month, day, hour, minut, 0);
        }

        /// <summary>
        /// Перевести строку, разделённую listSplitStr в список строк
        /// </summary>
        /// <param name="data">Данные, разделённые listSplitStr</param>
        /// <returns></returns>
        public static List<string> StringToList(string data, StringSplitOptions splitOption = StringSplitOptions.RemoveEmptyEntries)
        {
            string[] parts = data.Split(new[] { DbEngine.ListSplitStr }, splitOption);
            return new List<string>(parts);
        }

        /// <summary>
        /// Перевести строку, разделённую listSplitStr в массив строк
        /// </summary>
        /// <param name="data">Данные, разделённые listSplitStr</param>
        /// <returns></returns>
        public static string[] StringToArray(string data)
        {           
            List<string> list = StringToList(data);
            return list.ToArray();
        }

        /// <summary>
        /// Перевести строку, разделённую listSplitStr в массив с bool-ами
        /// </summary>
        /// <param name="data">Данные, разделённые listSplitStr</param>
        /// <returns></returns>
        public static bool[] StringToArrayBool(string data)
        {
            string[] parts = data.Split(new[] { DbEngine.ListSplitStr }, StringSplitOptions.RemoveEmptyEntries);

            var list = new List<bool>();

            foreach (string part in parts)
            {
                list.Add(Convert.ToBoolean(part));
            }

            return list.ToArray();
        }


        /// <summary>
        /// Перевести строку, разделённую listSplitStr в массив с числами
        /// </summary>
        /// <param name="data">Данные, разделённые listSplitStr</param>
        /// <returns></returns>
        public static int[] StringToArrayInt(string data)
        {
            string[] parts = data.Split(new[] { DbEngine.ListSplitStr }, StringSplitOptions.RemoveEmptyEntries);

            var list = new List<int>();

            foreach (string part in parts)
            {
                list.Add(Convert.ToInt32(part));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Перевести список строк в строку, разделённую listSplitStr
        /// </summary>
        /// <param name="list">Массив строк</param>
        /// <returns></returns>
        public static string ListToString(IEnumerable<string> list)
        {
            return ListToString(list, DbEngine.ListSplitStr);
        }

        /// <summary>
        /// Перевести список строк в строку, разделённую listSplitStr
        /// </summary>
        /// <param name="list">Массив строк</param>
        /// <param name="separator">Строка, через которую будут записаны элементы массива</param>
        /// <returns></returns>
        public static string ListToString(IEnumerable<string> list, string separator)
        {
            var listStr = new StringBuilder();
            foreach (string str in list)
            {
                listStr.Append(str + separator);
            }

            if (listStr.Length > separator.Length)
            {
                return listStr.ToString().Substring(0, listStr.Length - separator.Length);
            }

            return listStr.ToString();

        }

        /// <summary>
        /// Перевести список bool-ов в строку, разделённую listSplitStr
        /// </summary>
        /// <param name="list">Массивbool-ов</param>
        /// <returns></returns>
        public static string ListBoolToString(IEnumerable<bool> list)
        {
            var listStr = new StringBuilder();
            foreach (bool str in list)
            {
                listStr.Append(str + DbEngine.ListSplitStr);
            }

            return listStr.ToString();
        }

        /// <summary>
        /// Перевести список чисел в строку, разделённую listSplitStr
        /// </summary>
        /// <param name="list">Массив чисел</param>
        /// <returns></returns>
        public static string ListIntToString(IEnumerable<int> list)
        {
            var listStr = new StringBuilder();
            foreach (int str in list)
            {
                listStr.Append(str + DbEngine.ListSplitStr);
            }

            return listStr.ToString();
        }

        /// <summary>
        /// Вернуть разницу в днях между двумя датами. Конечная дата должна быть больше, чем начальная. В противном случае возвращается -1.
        /// </summary>
        /// <param name="dateTimeEnd">Конечная дата</param>
        /// <param name="dateTimeStart">Начальная дата</param>
        /// <returns></returns>
        public static int GetDiffInDays(DateTime dateTimeEnd, DateTime dateTimeStart)
        {
            if (DateTime.Compare(dateTimeEnd, dateTimeStart) == -1)
            {
                return -1;
            }

            return (dateTimeEnd - dateTimeStart).Days;
        }

        /// <summary>
        /// Сделать копию переменной с датой и временем
        /// </summary>
        /// <param name="value">Переменная с датой и временем</param>
        /// <returns></returns>
        public static DateTime CopyDateTime(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }

        /// <summary>
        ///  Сделать копию переменной с датой и временем
        /// </summary>
        /// <param name="fromObj">Переменная с датой и временем</param>
        /// <returns></returns>
        public static DateTime? CopyDateTime(DateTime? fromObj)
        {
            if (fromObj.HasValue)
            {
                return CopyDateTime(fromObj.Value);
            }

            return null;
        }

        /// <summary>
        /// Получить возраст из даты рождения
        /// </summary>
        /// <param name="dateBirthday">Дата рождения</param>
        /// <returns></returns>
        public static string GetAge(DateTime dateBirthday)
        {
            DateTime temp = CopyDateTime(dateBirthday);
            int yearCnt = 0;
            while (temp.Date < DateTime.Now.Date)
            {
                temp = temp.AddYears(1);
                yearCnt++;
            }

            if (yearCnt > 0)
            {
                return (--yearCnt).ToString();
            }

            return "0";
        }

        public static string GetAgeString(string ageStr)
        {
            int age;
            if (!int.TryParse(ageStr, out age))
            {
                return "лет";
            }

            int rem = age > 10 ? age % 10 : age;

            if ((age >= 5 && age <= 20) || (age >= 105 && age <= 120) || rem == 0 || rem >= 5)
            {
                return "лет";
            }

            if (rem == 1)
            {
                return "год";
            }

            return "года";
        }

        public static string GetBallString(string ballStr)
        {
            int ball;
            if (!int.TryParse(ballStr, out ball))
            {
                return " баллов";
            }

            if (ball > 1 && ball < 5)
            {
                return " балла";
            }

            if (ball == 1)
            {
                return " балл";
            }

            return " баллов";
        }

        public static void SetValues<T>(T[] targetArray, T[] values)
        {
            for (int i = 0; i < Math.Min(values.Length, targetArray.Length); i++)
            {
                targetArray[i] = values[i];
            }
        }

        /// <summary>
        /// Производит замену значений в targetArray на основании старого и нового значений в changeInfo
        /// </summary>
        /// <param name="targetArray">Массив, в котором нужно провести замену значений</param>
        /// <param name="indexes">Список индексов в массиве, которые надо просмотреть</param>
        /// <param name="changeInfo">Дикшинари, в котором ключом является старое значение, а значенем - новое</param>
        public static void UpdateValues(string[] targetArray, int[] indexes, Dictionary<string, string> changeInfo)
        {
            foreach (int i in indexes)
            {
                if (changeInfo.ContainsKey(targetArray[i]))
                {
                    targetArray[i] = changeInfo[targetArray[i]];
                }
            }
        }

        /// <summary>
        /// Сконвертировать временной отрезок в строку вида 'X ч ХХ мин'. Если часов нет - то только 'ХХ мин'
        /// </summary>
        /// <param name="timeLength">Временной отрезок</param>
        /// <returns></returns>
        public static string GetRightTimeLengthString(TimeSpan timeLength)
        {
            string result = string.Empty;
            if (timeLength.Hours > 0)
            {
                result += timeLength.Hours + " ч ";
            }

            return result + timeLength.Minutes + " мин";
        }

        /// <summary>
        /// Вернуть последние использованные записи в комбобоксе
        /// </summary>
        /// <param name="comboBox">Комбобокс</param>
        /// <param name="maxCnt">Максимальное количество последних записей. 20 по умолчанию</param>
        /// <returns></returns>
        public static string[] GetLastUsedValues(ComboBox comboBox, int maxCnt = 20)
        {
            var lastValuesList = new List<string> { comboBox.Text };
            foreach (string mkb in comboBox.Items)
            {
                if (lastValuesList.Count >= maxCnt)
                {
                    break;
                }

                if (!lastValuesList.Contains(mkb) && !string.IsNullOrEmpty(mkb))
                {
                    lastValuesList.Add(mkb);
                }
            }

            return lastValuesList.ToArray();
        }

        /// <summary>
        /// Конвертируем строку из нескольких строк в список строк
        /// </summary>
        /// <param name="multilineStr"></param>
        /// <returns></returns>
        public static List<string> MultilineStringToList(string multilineStr)
        {
            string[] arrOfStrings = multilineStr.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            return new List<string>(arrOfStrings);
        }

        /// <summary>
        /// Конвертируем список из строк в строку из нескольких строк
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ListToMultilineString(IEnumerable<string> list)
        {
            var multilineStr = new StringBuilder();
            foreach (string str in list)
            {
                multilineStr.Append(str + "\r\n");
            }

            if (multilineStr.Length > 2)
            {
                multilineStr.Remove(multilineStr.Length - 2, 2);
            }

            return multilineStr.ToString();
        }
    }
}
