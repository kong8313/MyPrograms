using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using zlib;

namespace LinesG
{
    public class Packer
    {
        /// <summary>
        /// Вызывает поток для запаковки данных в файл
        /// </summary>
        /// <param name="content">Строка с данными</param>
        /// <param name="path">Путь до файла</param>
        public static void SaveData(string content, string path)
        {
            try
            {
                byte[] bytesBuffer = Encoding.GetEncoding("windows-1251").GetBytes(content);
                byte[] rez = PackXml(bytesBuffer);

                using (var fs = new FileStream(path, FileMode.Create))
                {
                    fs.Write(rez, 0, rez.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении файла " + path + ":\r\n" + ex);
            }
        }

        /// <summary>
        /// Получает текст из запакованного файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns></returns>
        public static string LoadData(string path)
        {
            string unpackedString = string.Empty;
            try
            {
                if (!File.Exists(path))
                {
                    return unpackedString;
                }

                byte[] bytesBuffer;
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    bytesBuffer = new byte[fs.Length];
                    fs.Read(bytesBuffer, 0, bytesBuffer.Length);
                }

                byte[] rez = UnpackXml(bytesBuffer);
                if (rez.Length == 0)
                {
                    return unpackedString;
                }

                unpackedString = Encoding.GetEncoding("windows-1251").GetString(rez);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чтении файла " + path + ":\r\n" + ex);
            }

            return unpackedString;
        }


        /// <summary>
        /// Запаковать массив байт
        /// </summary>
        /// <param name="byteBuffer">Массив для запаковки</param>
        /// <returns></returns>
        private static byte[] PackXml(byte[] byteBuffer)
        {
            byte[] rez;

            using (var stream = new MemoryStream())
            {
                var zStream = new ZOutputStream(stream, zlibConst.Z_DEFAULT_COMPRESSION);
                zStream.Write(byteBuffer, 0, byteBuffer.Length);
                zStream.Close();

                rez = stream.ToArray();
            }

            return rez;
        }


        /// <summary>
        /// Распаковать массив байт
        /// </summary>
        /// <param name="byteBuffer">Массив для распаковки</param>
        /// <returns></returns>
        private static byte[] UnpackXml(byte[] byteBuffer)
        {
            byte[] rez;

            using (var stream = new MemoryStream())
            {
                var zStream = new ZOutputStream(stream);

                try
                {
                    zStream.Write(byteBuffer, 0, byteBuffer.Length);
                }
                catch
                {
                    return new byte[0];
                }

                zStream.Close();

                rez = stream.ToArray();
            }

            return rez;
        }
    }
}
