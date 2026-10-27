
namespace CYOA;

public class Weapon
{
    public string type { get; set; }
    public int maxDamage { get; set; }
    public string asciiArt { get; set; }

    public Weapon(string type, int maxDamage, string asciiArt)
    {
        this.type = type;
        this.maxDamage = maxDamage;
        this.asciiArt = asciiArt;
    }
}
