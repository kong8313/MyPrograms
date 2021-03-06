using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SurgeryHelper.Tools
{
    public static class CCompareEngine
    {
        /// <summary>
        /// Сравнивает даты и времена двух переменных типа DateTime. 
        /// Возвращает 1, если первая больше второй, -1 - если первая меньше второй, 0 - если равны
        /// </summary>
        /// <param name="dateTime1">Первая дата</param>
        /// <param name="dateTime2">Вторая дата</param>
        /// <returns></returns>
        public static int CompareDateTime(DateTime dateTime1, DateTime dateTime2)
        {
            int dateCompareResult = CompareDate(dateTime1, dateTime2);
            if (dateCompareResult == 0)
            {
                return CompareTime(dateTime1, dateTime2);
            }

            return dateCompareResult;
        }


        /// <summary>
        /// Сравнивает даты двух переменных типа DateTime. 
        /// Возвращает 1, если первая больше второй, -1 - если первая меньше второй, 0 - если равны
        /// </summary>
        /// <param name="dateTime1">Первая дата</param>
        /// <param name="dateTime2">Вторая дата</param>
        /// <returns></returns>
        public static int CompareDate(DateTime dateTime1, DateTime dateTime2)
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

            return 0;
        }


        /// <summary>
        /// Сравнение времени для двух объектов типа DateTime
        /// Возвращает 1, если первая больше второй, -1 - если первая меньше второй, 0 - если равны
        /// </summary>
        /// <param name="dateTime1">Первая дата</param>
        /// <param name="dateTime2">Вторая дата</param>
        /// <returns></returns>
        public static int CompareTime(DateTime dateTime1, DateTime dateTime2)
        {
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

            return 0;
        }


        /// <summary>
        /// Сравнить два массива
        /// </summary>
        /// <param name="array1">Первый массив</param>
        /// <param name="array2">Второй массив</param>
        /// <param name="ownValue">Информация о различие для первого массива</param>
        /// <param name="foreignValue">Информация о различие для второго массива</param>
        /// <returns></returns>
        public static bool IsArraysEqual(int[] array1, int[] array2, out string ownValue, out string foreignValue)
        {
            ownValue = string.Empty;
            foreignValue = string.Empty;

            if (array1.Length != array2.Length)
            {
                ownValue = array1.Length + " строк";
                foreignValue = array2.Length + " строк";
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    ownValue += array1[i] + " в " + (i + 1) + "-м элементе, ";
                    foreignValue += array2[i] + " в " + (i + 1) + "-м элементе, ";
                }
            }

            if (string.IsNullOrEmpty(ownValue))
            {
                return true;
            }

            ownValue = ownValue.Substring(0, ownValue.Length - 2);
            foreignValue = foreignValue.Substring(0, foreignValue.Length - 2);
            return false;
        }


        /// <summary>
        /// Сравнить два массива
        /// </summary>
        /// <param name="array1">Первый массив</param>
        /// <param name="array2">Второй массив</param>
        /// <param name="ownValue">Информация о различие для первого массива</param>
        /// <param name="foreignValue">Информация о различие для второго массива</param>
        /// <returns></returns>
        public static bool IsArraysEqual(string[] array1, string[] array2, out string ownValue, out string foreignValue)
        {
            ownValue = string.Empty;
            foreignValue = string.Empty;

            if (array1.Length != array2.Length)
            {
                ownValue = array1.Length + " строк";
                foreignValue = array2.Length + " строк";
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    ownValue += array1[i] + " в " + (i + 1) + "-м элементе, ";
                    foreignValue += array2[i] + " в " + (i + 1) + "-м элементе, ";
                }
            }

            if (string.IsNullOrEmpty(ownValue))
            {
                return true;
            }

            ownValue = ownValue.Substring(0, ownValue.Length - 2);
            foreignValue = foreignValue.Substring(0, foreignValue.Length - 2);
            return false;
        }


        /// <summary>
        /// Сравнить два массива
        /// </summary>
        /// <param name="array1">Первый массив</param>
        /// <param name="array2">Второй массив</param>
        /// <param name="ownValue">Информация о различие для первого массива</param>
        /// <param name="foreignValue">Информация о различие для второго массива</param>
        /// <returns></returns>
        public static bool IsArraysEqual(bool[] array1, bool[] array2, out string ownValue, out string foreignValue)
        {
            ownValue = string.Empty;
            foreignValue = string.Empty;

            if (array1.Length != array2.Length)
            {
                ownValue = array1.Length + " строк";
                foreignValue = array2.Length + " строк";
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    ownValue += array1[i] + " в " + (i + 1) + "-м элементе, ";
                    foreignValue += array2[i] + " в " + (i + 1) + "-м элементе, ";
                }
            }

            if (string.IsNullOrEmpty(ownValue))
            {
                return true;
            }

            ownValue = ownValue.Substring(0, ownValue.Length - 2);
            foreignValue = foreignValue.Substring(0, foreignValue.Length - 2);
            return false;
        }
        

        /// <summary>
        /// Сравнить две картинки
        /// </summary>
        /// <param name="bmp1">Первая картинка</param>
        /// <param name="bmp2">Ворая картинка</param>
        /// <returns></returns>
        public static bool IsBitmapsDifferent(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height)
            {
                return true;
            }

            const ImageLockMode mode = ImageLockMode.ReadOnly;
            var range = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            BitmapData bmpd1 = bmp1.LockBits(range, mode, bmp1.PixelFormat);
            BitmapData bmpd2 = bmp2.LockBits(range, mode, bmp2.PixelFormat);

            try
            {
                unsafe
                {
                    var p1 = (byte*)(void*)bmpd1.Scan0;
                    var p2 = (byte*)(void*)bmpd2.Scan0;

                    int c = range.Height * bmpd1.Stride;
                    for (int i = 0; i < c; i++)
                    {
                        if (*p1 != *p2)
                        {
                            return true;
                        }

                        p1++;
                        p2++;
                    }
                }
            }
            finally
            {
                bmp1.UnlockBits(bmpd1);
                bmp2.UnlockBits(bmpd2);
            }

            return false;
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
    }
}
