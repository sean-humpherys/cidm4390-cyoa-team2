public class Dice
{
    private static readonly Random random = new Random();

    public int Roll(int sides)
    {
        return random.Next(1, sides + 1);
    }
}