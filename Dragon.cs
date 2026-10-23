namespace CYOA;
public class Dragon
{
    public string DragonName { get; set; }
    public int DragonStrength { get; set; }
    public int DragonHealth { get; set; }
    public int DragonAgility { get; set; }
    public string DragonWeapon { get; set; }

    public Dragon(string dragonName)
    {
        DragonName = dragonName;
        DragonStrength = 15;
        DragonHealth = 100;
        DragonAgility = 10;
        DragonWeapon = "Fire Breath";
    }

    public string DisplayStats()
    {
        return "--- Dragon Statistics ---\n" +
               $"Name: {DragonName}\n" +
               $"Strength: {DragonStrength}\n" +
               $"Health: {DragonHealth}\n" +
               $"Agility: {DragonAgility}\n" +
               $"Weapon: {DragonWeapon}";
    }
}