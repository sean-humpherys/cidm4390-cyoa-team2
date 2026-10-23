using CYOA;
public class Combat
{
    public bool CheckHit(int strength)
    {
        int roll = Dice.Roll(20);

        Console.WriteLine("Attack Roll: " + roll);

        return roll <= strength;
    }

    public bool CheckDefense(int agility)
    {
        int roll = Dice.Roll(20);

        Console.WriteLine("Defense Roll: " + roll);

        return roll <= agility;
    }

    public int CalculateDamage(int maxWeaponDamage)
    {
        if (maxWeaponDamage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxWeaponDamage));
        }

        return Dice.Roll(maxWeaponDamage);
    }

    public void DragonTaunt()
    {
        string[] taunts =
        {
            "Ow! That actually stung a bit!",
            "Impressive! You might actually be worth toasting...",
            "That... was unexpected. But I assure you, it wont happen again.",
            "Well, I'll be! you have more bite than I thought!"
        };

        int randomTaunt = Dice.Roll(taunts.Length) - 1;

        Console.WriteLine(taunts[randomTaunt]);
    }

    public void DragonSarcasticReply()
    {
        string[] taunts =
        {
          "How adorable. You're actually trying to win.",
          "You're still standing? Your persistence is almost as annoying as your weapon.",
          "Still breating? You really are a stubborn, aren't you?",
          "I'm genuinely impressed. Most people have the common sense to die much faster than this."
        };

        int randomTaunt = Dice.Roll(taunts.Length) - 1;

        Console.WriteLine(taunts[randomTaunt]);
    }

    public int PlayerAttack(Character character, Dragon dragon)
    {
        //Character attempts to hit the dragon
        if (!CheckHit(character.Strength))
        {
            Console.WriteLine($"You missed{dragon.DragonName}!");

            return dragon.DragonHealth;
        }

        Console.WriteLine($"You hit {dragon.DragonName}!");

        //Dragon attempts to defend
        if (CheckDefense(dragon.DragonAgility))
        {
            Console.WriteLine($"{dragon.DragonName} successfully defends your attack");

            return dragon.DragonHealth;
        }

        //Calculate damage
        int damage = CalculateDamage(character.MaxWeaponDamage);

        dragon.DragonHealth -= damage;

        Console.WriteLine($"The dragon failed to defend your attack. Your weapon delivers {damage} damage points. The dragon's Health is now {dragon.DragonHealth}");

        if(dragon.DragonHealth <= 0)
        {
            Console.WriteLine($"with the last word of 'Argg!' you have defeated the {dragon.DragonName}");
        }
        else
        {
            DragonTaunt();
        }

        return dragon.DragonHealth;
    }

    public int DragonAttack(Dragon dragon, Character character)
    {
        //Dragon attempts to hit Character
        Console.WriteLine($"{dragon.DragonName} attacks!");

        if (!CheckHit(dragon.DragonStrength))
        {
            Console.WriteLine($"{dragon.DragonName} missed you!");

            return character.Health;
        }

        Console.WriteLine($"{dragon.DragonName} hits you!");

        if (CheckDefense(character.Agility))
        {
            Console.WriteLine("You successfully defend the attack!");

            return character.Health;
        }

        //Calculate damage
        int damage = CalculateDamage(character.MaxWeaponDamage);

        character.Health -= damage;

        Console.WriteLine($"The dragon's attack delivers {damage} damage points.");

        Console.WriteLine($"Your health points are now {character.Health}.");

        if (character.Health <= 0)
        {
            Console.WriteLine($"With the last words of 'Oh no!', you have been defeated by {dragon.DragonName}!");
        }
        else
        {
            DragonSarcasticReply();
        }

        return character.Health;
    }
}