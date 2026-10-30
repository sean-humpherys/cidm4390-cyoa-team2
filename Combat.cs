using CYOA;
public class Combat
{
    private readonly Messages _messages;
    private readonly TextWriter _output;
    public Combat(Messages? messages = null, TextWriter? output = null)
    {
        _messages = messages ?? new Messages();
        _output = output ?? Console.Out;
    }
    public bool CheckHit(int strength)
    {
        int roll = Die.Roll(20);
        _output.WriteLine(_messages.FormatMessage(61, ("roll", roll)));
        return roll <= strength;
    }
    public bool CheckDefense(int agility)
    {
        int roll = Die.Roll(20);
        _output.WriteLine(_messages.FormatMessage(62, ("roll", roll)));
        return roll <= agility;
    }
    public int CalculateDamage(int maxWeaponDamage)
    {
        if (maxWeaponDamage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxWeaponDamage));
        }
        return Die.Roll(maxWeaponDamage);
    }
    public void DragonTaunt()
    {
        int randomTaunt = 44 + Die.Roll(4) - 1;
        _output.WriteLine(_messages.GetMessage(randomTaunt));
    }
    public void DragonSarcasticReply()
    {
        int randomTaunt = 57 + Die.Roll(4) - 1;
        _output.WriteLine(_messages.GetMessage(randomTaunt));
    }

    //Player attempts to attack the dragon
    public int PlayerAttack(Player character, Dragon dragon)
    {
        if (!CheckHit(character.Strength))
        {
            _output.WriteLine(_messages.FormatMessage(35, ("dragonName", dragon.DragonName)));
            return dragon.DragonHealth;
        }
        _output.WriteLine(_messages.FormatMessage(21, ("dragonName", dragon.DragonName)));
        if (CheckDefense(dragon.DragonAgility))
        {
            _output.WriteLine(_messages.FormatMessage(22, ("dragonName", dragon.DragonName)));
            return dragon.DragonHealth;
        }
        int damage = CalculateDamage(character.Weapon.maxDamage);
        dragon.DragonHealth -= damage;
        _output.WriteLine(_messages.FormatMessage(23,
            ("damage", damage),
            ("healthPoints", dragon.DragonHealth)));
        if(dragon.DragonHealth <= 0)
        {
            _output.WriteLine(_messages.GetMessage(24));
        }
        else
        {
            DragonTaunt();
        }
        return dragon.DragonHealth;
    }

    //Dragon attempts to attack
    public int DragonAttack(Dragon dragon, Player character)
    {
        _output.WriteLine(_messages.FormatMessage(63, ("dragonName", dragon.DragonName)));
        if (!CheckHit(dragon.DragonStrength))
        {
            _output.WriteLine(_messages.GetMessage(37));
            return character.Health;
        }
        _output.WriteLine(_messages.FormatMessage(36, ("dragonName", dragon.DragonName)));
        if (CheckDefense(character.Agility))
        {
            _output.WriteLine(_messages.GetMessage(38));
            return character.Health;
        }
        int damage = CalculateDamage(dragon.Weapon.maxDamage);
        character.Health -= damage;
        _output.WriteLine(_messages.FormatMessage(39,
            ("damage", damage),
            ("healthPoints", character.Health)));
        if (character.Health <= 0)
        {
            _output.WriteLine(_messages.GetMessage(40));
        }
        else
        {
            DragonSarcasticReply();
        }
        return character.Health;
    }
}


