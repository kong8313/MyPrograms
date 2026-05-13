namespace WaterSolver;

public class SolutionExplorer
{
    private readonly List<sbyte[]> _initialFlasks;
    private readonly List<FlaskCell> _flaskCells;
    private HashSet<int> _hashes;
    private Position _bestFinishPosition;

    public SolutionExplorer(List<sbyte[]> flasks, List<FlaskCell> flaskCells)
    {
        _initialFlasks = flasks;
        _flaskCells = flaskCells;
    }

    public int FirstPositionsCount { get; private set; }
    public int CurrentFirstPositionsNumber { get; private set; }
    public int FinishPositionsCount { get; private set; }
    public int MaxDepth { get; private set; }
    public int HashesCount => _hashes.Count;

    public List<Position> Find()
    {
        var firstStep = new StepInfo("", 0, 0);
        var firstPosition = new Position(_initialFlasks, new List<StepInfo> { firstStep });
        _hashes = new HashSet<int>();

        FirstPositionsCount = 0;
        CurrentFirstPositionsNumber = 0;
        MaxDepth = 0;
        FinishPositionsCount = 0;
        _bestFinishPosition = firstPosition;
        var winPosition = RecursiveLook(firstPosition, true);

        if (winPosition == null)
        {
            return FillSolution(firstPosition, _bestFinishPosition.Steps);
        }

        return FillSolution(firstPosition, winPosition.Steps);
    }

    private Position RecursiveLook(Position position, bool isFirstCall)
    {
        if (position.IsSolution)
            return position;

        if (position.Steps.Count > MaxDepth)
        {
            MaxDepth++;
        }

        var newPositions = FindPossiblePositions(position);
        if (newPositions.Count == 0)
        {
            FinishPositionsCount++;
            if (_bestFinishPosition.Steps.Count < position.Steps.Count)
            {
                _bestFinishPosition = position;
            }
            
            return null;
        }

        var isSolutionFound = newPositions.Any(x => x.IsSolution);
        if (isSolutionFound)
        {
            return newPositions.First(x => x.IsSolution);
        }

        if (isFirstCall)
        {
            FirstPositionsCount = newPositions.Count;
        }

        for (int i = 0; i < newPositions.Count; i++)
        {
            if (isFirstCall)
            {
                CurrentFirstPositionsNumber = i + 1;
            }

            var winPosition = RecursiveLook(newPositions[i], false);
            if (winPosition != null)
            {
                return winPosition;
            }
        }

        return null;
    }

    private List<Position> FillSolution(Position firstPosition, List<StepInfo> steps)
    {
        var result = new List<Position> { };
        var currentPosition = firstPosition;
        foreach (var step in steps)
        {
            var newFlasks = CopyFlasks(currentPosition.Flasks);
            if (step.FromFlaskNumber > 0 && step.ToFlaskNumber > 0)
            {
                Pour(newFlasks, step.FromFlaskNumber - 1, step.ToFlaskNumber - 1);
            }

            var newPosition = new Position(newFlasks, new List<StepInfo> { step });
            result.Add(newPosition);
            currentPosition = newPosition;
        }

        return result;
    }

    private sbyte GetLastColor(sbyte[] cells)
    {
        sbyte colorNumber = 0;
        int j = 3;
        while (j >= 0)
        {
            if (cells[j] > 0)
            {
                colorNumber = cells[j];
                break;
            }

            j--;
        }

        return colorNumber;
    }
    
    private void Pour(List<sbyte[]> flasks, int fromFlaskNumber, int toFlaskNumber)
    {
        int fromIndex = 3;
        while (fromIndex > 0 && flasks[fromFlaskNumber][fromIndex] == 0)
        {
            fromIndex--;
        }

        int toIndex = 2;
        while (toIndex >= 0 && flasks[toFlaskNumber][toIndex] == 0)
        {
            toIndex--;
        }

        sbyte colorNumber = flasks[fromFlaskNumber][fromIndex];
        toIndex++;
        while (fromIndex >= 0 && flasks[fromFlaskNumber][fromIndex] == colorNumber && toIndex < 4)
        {
            flasks[fromFlaskNumber][fromIndex] = 0;
            flasks[toFlaskNumber][toIndex] = colorNumber;
            fromIndex--;
            toIndex++;
        }
    }

    private List<Position> FindPossiblePositions(Position position)
    {
        var positions = new List<Position>();
        for (byte flaskNumber = 0; flaskNumber < position.Flasks.Count; flaskNumber++)
        {
            sbyte fromColorNumber = GetLastColor(position.Flasks[flaskNumber]);
            if (fromColorNumber == 0)
            {
                continue;
            }

            for (byte i = 0; i < position.Flasks.Count; i++)
            {
                // Skip the same flask and if toFlask is full
                if (i == flaskNumber || position.Flasks[i][3] != 0)
                {
                    continue;
                }

                // Skip if the last cell in toFlask has a different color and not empty
                sbyte toColorNumber = GetLastColor(position.Flasks[i]);
                if (toColorNumber != fromColorNumber && toColorNumber != 0)
                {
                    continue;
                }

                // Skip if the toFlask is empty and fromFlask contains the same colors
                if (toColorNumber == 0 && IsAllColorsTheSame(position.Flasks[flaskNumber]))
                {
                    continue;
                }

                // Skip if it is possible to pour just part of cells to pour and no other places to pour other cells with the same color
                if (IsUselessPour(position, flaskNumber, i, fromColorNumber))
                {
                    continue;
                }

                var cellName = _flaskCells.First(x => x.Number == fromColorNumber).Name;
                var step = new StepInfo(cellName, (byte)(flaskNumber + 1), (byte)(i + 1));
                var newSteps = new List<StepInfo>(position.Steps) { step };

                var newFlasks = CopyFlasks(position.Flasks);
                Pour(newFlasks, flaskNumber, i);

                var newPosition = new Position(newFlasks, newSteps);
                if(!_hashes.Contains(newPosition.Hash))
                {
                    _hashes.Add(newPosition.Hash);
                    positions.Add(newPosition);
                }
            }
        }

        return positions;
    }

    private bool IsUselessPour(Position position, byte fromFlaskNumber, byte toFlaskNumber, sbyte colorNumber)
    {
        if (AllCellsWillBePoured(position.Flasks[fromFlaskNumber], position.Flasks[toFlaskNumber]))
        {
            return false;
        }

        if (IsThereOtherFlaskToPour(position, colorNumber, new int[]{ fromFlaskNumber, toFlaskNumber }))
        {
            return false;
        }

        return true;
    }

    private bool AllCellsWillBePoured(sbyte[] fromFlask, sbyte[] toFlask)
    {
        int emptyCellsCnt = 0;
        int i = 3;
        while (i > 0 && toFlask[i] == 0)
        {
            i--;
            emptyCellsCnt++;
        }

        int sameColorCnt = 0;
        i = 3;
        while (fromFlask[i] == 0)
        {
            i--;
        }

        int colorNumber = fromFlask[i];
        while (i >= 0 && fromFlask[i] == colorNumber)
        {
            i--;
            sameColorCnt++;
        }

        return sameColorCnt <= emptyCellsCnt;
    }

    private bool IsThereOtherFlaskToPour(Position position, sbyte colorNumber, int[] ignoredFlaskNumbers)
    {
        for (int i = 0; i < position.Flasks.Count; i++)
        {
            if (ignoredFlaskNumbers.Contains(i))
            {
                continue;
            }

            if (GetLastColor(position.Flasks[i]) == colorNumber)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsAllColorsTheSame(sbyte[] positionFlask)
    {
        if (positionFlask[1] == 0 && positionFlask[2] == 0 && positionFlask[3] == 0)
        {
            return true;
        }

        return (positionFlask[3] == 0 || positionFlask[3] == positionFlask[0]) &&
               (positionFlask[2] == 0 || positionFlask[2] == positionFlask[0]) &&
               (positionFlask[1] == 0 || positionFlask[1] == positionFlask[0]);
    }

    private List<sbyte[]> CopyFlasks(List<sbyte[]> flasks)
    {
        var newFlasks = new List<sbyte[]>();
        foreach (var flask in flasks)
        {
            var newCells = new [] { flask[0], flask[1], flask[2], flask[3] };
            newFlasks.Add(newCells);
        }

        return newFlasks;
    }
}