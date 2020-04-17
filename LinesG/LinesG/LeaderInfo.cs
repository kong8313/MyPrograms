using System;

namespace LinesG
{
    public class LeaderInfo
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public int TimeInSec { get; set; }

        public LeaderInfo(string name, int score, int timeInSec)
        {
            Name = name;
            Score = score;
            TimeInSec = timeInSec;
        }

        public LeaderInfo(string dataFromFile)
        {
            string[] data = dataFromFile.Split(new[] { "^^" }, System.StringSplitOptions.RemoveEmptyEntries);

            if (data.Length != 3)
            {
                throw new Exception("Неправильный формат данных: " + dataFromFile);
            }

            Name = data[0];

            if (!int.TryParse(data[1], out var score))
            {
                throw new Exception("Неправильное значение очков: " + dataFromFile);
            }
            else
            {
                Score = score;
            }

            if (!int.TryParse(data[2], out var timeInSec))
            {
                throw new Exception("Неправильное значение времени: " + dataFromFile);
            }
            else
            {
                TimeInSec = timeInSec;
            }
        }
    }
}
