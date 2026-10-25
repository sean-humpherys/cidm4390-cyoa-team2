namespace CYOA;

public static class CharacterStatistics
{
    public static string Display(
        string name,
        string race,
        string occupation,
        int strength,
        int agility,
        int health,
        string weapon,
        int maxWeaponDamage)
    {
        return
            "===== CHARACTER STATISTICS =====\n" +
            $"Name: {name}\n" +
            $"Race: {race}\n" +
            $"Occupation: {occupation}\n" +
            $"Strength: {strength}\n" +
            $"Agility: {agility}\n" +
            $"Health: {health}\n" +
            $"Weapon: {weapon}\n" +
            $"Maximum Weapon Damage: {maxWeaponDamage}\n" +
            "================================";
    }
}
