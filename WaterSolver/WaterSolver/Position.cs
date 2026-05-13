using System.Text;

namespace WaterSolver;

public class Position
{
    public List<sbyte[]> Flasks { get; set; }
    public List<StepInfo> Steps { get; set; }
    public bool IsSolution { get; set; }
    public int Hash { get; set; }

    public Position(List<sbyte[]> flasks, List<StepInfo> steps)
    {
        Flasks = flasks;
        Steps = steps;
        IsSolution = SetIsSolution();
        Hash = CreateHash();
    }

    private int CreateHash()
    {
        var sb = new StringBuilder();
        foreach (var flask in Flasks)
        {
            foreach (var cell in flask)
            {
                sb.Append(cell.ToString());
            }
        }

        return sb.ToString().GetHashCode();
    }

    private bool SetIsSolution()
    {
        foreach (var flask in Flasks)
        {
            var lastColor = GetLastColor(flask);
            if (lastColor == -1)
            {
                return true;
            }
        }

        foreach (var flask in Flasks)
        {
            var firstColor = flask[0];
            for (int i = 1; i < 4; i++)
            {
                if (flask[i] != firstColor)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private sbyte GetLastColor(sbyte[] cells)
    {
        sbyte j = 3;
        while (j >= 0)
        {
            if (cells[j] != 0)
            {
                return cells[j];
            }

            j--;
        }

        return 0;
    }
}

public class StepInfo
{
    public string CellName { get; set; }
    public byte FromFlaskNumber { get; set; }
    public byte ToFlaskNumber { get; set; }

    public StepInfo(string cellName, byte fromFlaskNumber, byte toFlaskNumber)
    {
        CellName = cellName;
        FromFlaskNumber = fromFlaskNumber;
        ToFlaskNumber = toFlaskNumber;
    }

    public override string ToString()
    {
        if (FromFlaskNumber == 0 && ToFlaskNumber == 0)
            return "Start position";

        return $"{CellName} from {FromFlaskNumber} to {ToFlaskNumber}";
    }
}