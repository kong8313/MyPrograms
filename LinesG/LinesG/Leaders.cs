using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LinesG
{
    public class Leaders
    {
        public List<LeaderInfo> LeadersList { get; }
        public int MinScore { get; }
        public int MaxScore { get; }
        
        private string _leaderFilePath;

        public Leaders()
        {
            const string leadersFileName = "LeaderList.ldr";

            LeadersList = new List<LeaderInfo>();

            _leaderFilePath = Path.Combine(Application.StartupPath, leadersFileName);

            if (File.Exists(_leaderFilePath))
            {
                ReadLeadersListFromFile(); 
            }
            else
            {
                InitNewLeadersList();
            }

            MaxScore = LeadersList.First().Score;
            MinScore = LeadersList.Last().Score;
        }

        private int GetStableHashCode(string str)
        {
            unchecked
            {
                int hash1 = 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        private void InitNewLeadersList()
        {
            string importFilePath = _leaderFilePath.Replace(".ldr", ".txt");
            if (File.Exists(importFilePath))
            {
                try
                {
                    var newLeaderForm = new NewLeaderNameForm();
                    newLeaderForm.ShowDialog();
                    
                    if (GetStableHashCode(newLeaderForm.UserName) != -1686706976)
                    {
                        MessageBox.Show("Неправильное имя. Импорт лидеров невозможен. Удалите файл LeaderList.txt или введдите правильное имя после перезапуска программы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }

                    string[] contentLines = File.ReadAllLines(importFilePath);
                    if (contentLines.Length != 10)
                    {
                        throw new Exception();
                    }

                    foreach (string contentLine in contentLines)
                    {
                        string[] data = contentLine.Split('\t');
                        LeadersList.Add(new LeaderInfo(data[0], int.Parse(data[1]), (int)TimeSpan.Parse(data[2]).TotalSeconds));
                    }
                }
                catch
                {
                    MessageBox.Show("Найденный файл для импорта списка лидеров LeaderList.txt имеет неправильный формат. Удалите его или исправьте и перезапустите программу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    LeadersList.Add(new LeaderInfo("Игрок " + (i + 1), 10, 3600));
                }
            }

            SaveData();
        }

        private void ReadLeadersListFromFile()
        {
            string[] leadersdata = Packer.LoadData(_leaderFilePath).Split(new[] { "^;^" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < leadersdata.Length; i++)
            {
                LeadersList.Add(new LeaderInfo(leadersdata[i]));
            }
        }

        public void AddNewLeader(string userName, int score, int timeInSec)
        {
            int i = 8;
            while (i >= 0 && LeadersList[i].Score < score)
            {
                LeadersList[i + 1].Name = LeadersList[i].Name;
                LeadersList[i + 1].Score = LeadersList[i].Score;
                LeadersList[i + 1].TimeInSec = LeadersList[i].TimeInSec;

                i--;
            }

            i++;
            LeadersList[i].Name = userName;
            LeadersList[i].Score = score;
            LeadersList[i].TimeInSec = timeInSec;

            SaveData();
        }

        private void SaveData()
        {
            string dataStr = string.Join("^;^", LeadersList.Select(x => $"{x.Name}^^{x.Score}^^{x.TimeInSec}"));
            Packer.SaveData(dataStr, _leaderFilePath);
        }
    }
}
