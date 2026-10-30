namespace CYOA;
public class Die
{
    private static readonly Random random = new Random();

    public static int Roll(int sides)
    {
        return random.Next(1, sides + 1);
    }
}