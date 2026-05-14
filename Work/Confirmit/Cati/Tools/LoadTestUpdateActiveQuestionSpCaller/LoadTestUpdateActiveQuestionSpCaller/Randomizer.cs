namespace LoadTestUpdateActiveQuestionSpCaller;

public class Randomizer
{
    private static readonly System.Random Random = new System.Random((int)DateTime.UtcNow.Ticks);

    public static int Next(int minValue, int maxValue)
    {
        lock (Random)
        {
            return Random.Next(minValue, maxValue);
        }
    }

    public static int Next(int maxValue)
    {
        lock (Random)
        {
            return Random.Next(maxValue);
        }
    }

    public static double NextDouble()
    {
        lock (Random)
        {
            return Random.NextDouble();
        }
    }

    public static int Next()
    {
        lock (Random)
        {
            return Random.Next();
        }
    }
}