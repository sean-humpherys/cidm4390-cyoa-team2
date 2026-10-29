using System.Text.Json;
using System.IO;
using System.Collections.Generic;

namespace CYOA;

public class Messages
{
    // Message IDs. The text for each lives in MessageFiles/*.json.
    public const int WelcomeMessageId = 5;
    public const int MainMenuPromptMessageId = 6;
    public const int PathPromptMessageId = 7;
    public const int SouthPathMessageId = 8;
    public const int NorthPathMessageId = 9;
    public const int DragonSaysMessageId = 10;
    public const int MagicRingMessageId = 11;
    public const int RetreatOrAttackPromptMessageId = 12;
    public const int RacePromptMessageId = 13;
    public const int NamePromptMessageId = 14;
    public const int OccupationPromptMessageId = 15;
    public const int StrengthPromptMessageId = 16;
    public const int AgilityPromptMessageId = 17;
    public const int HealthPromptMessageId = 18;
    public const int AttackIntroMessageId = 20;        // {occupation}, {weapon}
    public const int AttackPromptMessageId = 25;
    public const int AttackRetreatMessageId = 26;
    public const int NameLabelId = 27;
    public const int RaceLabelId = 28;
    public const int OccupationLabelId = 29;
    public const int StrengthLabelId = 30;
    public const int AgilityLabelId = 31;
    public const int HealthLabelId = 32;
    public const int WeaponLabelId = 33;
    public const int MaxDamageLabelId = 34;
    public const int RetreatMessageId = 41;
    public const int DragonDefeatedMessageId = 42;
    public const int PlayerDefeatedMessageId = 43;
    public const int InvalidChoiceMessageId = 52;
    public const int FighterArtMessageId = 53;
    public const int MagicianArtMessageId = 54;
    public const int ThiefArtMessageId = 55;
    public const int ArcherArtMessageId = 56;

    // Combat
    public const int YouHitMessageId = 21;
    public const int DragonDefendsMessageId = 22;
    public const int PlayerDamageMessageId = 23;
    public const int DragonLastWordsMessageId = 24;
    public const int YouMissedMessageId = 35;
    public const int DragonHitsMessageId = 36;
    public const int DragonMissedMessageId = 37;
    public const int YouDefendedMessageId = 38;
    public const int DragonDamageMessageId = 39;
    public const int PlayerLastWordsMessageId = 40;
    public const int AttackRollMessageId = 61;
    public const int DefenseRollMessageId = 62;
    public const int DragonAttacksMessageId = 63;

    // Taunts (the dragon has four of each, starting at these IDs) and the dragon stats header
    public const int DragonWasHitTauntFirstId = 44;       // 44 to 47
    public const int DragonMissedMeTauntFirstId = 57;     // 57 to 60
    public const int PlayerMissedMeTauntId = 64;
    public const int PlayerWasHitTauntId = 65;
    public const int DragonStatisticsHeaderId = 66;

    private const string DefaultLanguage = "English";

    private readonly Dictionary<string, Dictionary<int, string>> _dictionaries = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<int, string> languageDictionary = new();
    private string currentLanguage = DefaultLanguage;

    public string CurrentLanguage
    {
        get => currentLanguage;
        set => currentLanguage = string.IsNullOrWhiteSpace(value) ? DefaultLanguage : value.Trim();
    }

    public void ReadDictionary(string languageFilePath)
    {
        if (string.IsNullOrWhiteSpace(languageFilePath))
            throw new ArgumentException("Path must not be null or empty.", nameof(languageFilePath));

        if (!File.Exists(languageFilePath))
            throw new FileNotFoundException("Language file not found.", languageFilePath);

        var loaded = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(languageFilePath))
            ?? throw new FormatException("Invalid JSON language file.");

        var messages = new Dictionary<int, string>();
        foreach (var entry in loaded)
        {
            if (!int.TryParse(entry.Key, out int id))
                throw new FormatException($"Invalid message ID: {entry.Key}");

            messages[id] = entry.Value;
        }

        _dictionaries[CurrentLanguage] = messages;
        languageDictionary = messages;
    }

    public string GetMessage(int whichMessage)
    {
        if (_dictionaries.TryGetValue(CurrentLanguage, out var current) &&
            current.TryGetValue(whichMessage, out var text))
            return text;

        if (_dictionaries.TryGetValue(DefaultLanguage, out var english) &&
            english.TryGetValue(whichMessage, out text))
            return text;

        return $"[{whichMessage}]";
    }

    public void ReadJsonDictionary(string languageFilePath)
    {
        if (string.IsNullOrWhiteSpace(languageFilePath))
            throw new ArgumentException(
                "Path must not be null or empty.",
                nameof(languageFilePath));

        if (!File.Exists(languageFilePath))
            throw new FileNotFoundException(
                "Language file not found.",
                languageFilePath);

        string json = File.ReadAllText(languageFilePath);

        var loaded = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
            ?? throw new FormatException("Invalid JSON language file.");

        var messages = new Dictionary<int, string>();

        foreach (var entry in loaded)
        {
            if (!int.TryParse(entry.Key, out int id))
                throw new FormatException($"Invalid message ID: {entry.Key}");

            messages[id] = entry.Value;
        }

        _dictionaries[CurrentLanguage] = messages;
        languageDictionary = messages;
    }
}