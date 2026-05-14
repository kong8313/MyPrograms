using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DialerWsLogParserLibrary
{
    public class FileReader
    {
        public List<string> FileNames { get; private set; }
        public List<string> Text { get; private set; }
        public List<string> RecentFileNames { get; private set; }

        public FileReader()
        {
            FileNames = new List<string>();
            RecentFileNames = new List<string>();
            Text = new List<string>();
        }

        public void SetRecentFileNames(List<string> files)
        {
            RecentFileNames = files;
        }

        public void ReadFile(string fileName)
        {
            if (Text == null)
                Text = new List<string>();

            FileInfo file = new FileInfo(fileName);

            using (StreamReader reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    Text.Add(line);
            }

            AddFileName(fileName);
            AddRecentFileName(fileName);
        }

        public void ReadFilesFromArchive(string archiveName)
        {
            if (Text == null)
                Text = new List<string>();

            var archive = ZipFile.OpenRead(archiveName);
            var entries = archive.Entries;
            foreach (var entry in entries)
                using (StreamReader reader = new StreamReader(entry.Open(), System.Text.Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        Text.Add(line);
                }

            AddFileName(archiveName);
        }

        public void Clean()
        {
            FileNames = new List<string>();
            Text = new List<string>();
        }

        private void AddFileName (string fileName)
        {
            if (FileNames == null)
                FileNames = new List<string>();
            FileNames.Add(fileName);
        }

        private void AddRecentFileName(string fileName)
        {
            if (RecentFileNames == null)
                RecentFileNames = new List<string>();

            int deletedIndex = RecentFileNames.FindIndex(f => f == fileName);

            if (deletedIndex != -1)
                RecentFileNames.RemoveAt(deletedIndex);
            else if (RecentFileNames.Count >= 10)
                RecentFileNames.RemoveAt(9);

            RecentFileNames.Insert(0, fileName);
        }
    }
}
