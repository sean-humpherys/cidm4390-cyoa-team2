
namespace CYOA;

public class Character
{
    public string Name { get; set; }
    public string Race { get; set; }
    public string Occupation { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Health { get; set; }
    public string Weapon { get; set; }
    public int MaxWeaponDamage { get; set; }

    public Character(
        string name,
        string race,
        string occupation,
        int strength,
        int agility,
        int health,
        string weapon,
        int maxWeaponDamage)
    {
        Name = name;
        Race = race;
        Occupation = occupation;
        Strength = strength;
        Agility = agility;
        Health = health;
        Weapon = weapon;
        MaxWeaponDamage = maxWeaponDamage;
    }
}
