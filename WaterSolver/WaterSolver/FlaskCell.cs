namespace WaterSolver
{
    public class FlaskCell
    {
        public Bitmap CellPicture { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }

        public FlaskCell() 
        {
            CellPicture = new Bitmap(1, 1);
            Number = -1;
            Name = string.Empty;
        }

        public void Load(int number, string cellImageFilePath)
        {
            Number = number;
            Name = new FileInfo(cellImageFilePath).Name.Split('.').First().Split('_').Last();
            CellPicture = new Bitmap(cellImageFilePath);
        }
    }
}
