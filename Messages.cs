using System;
using System.Text.Json;

namespace CYOA;

public class Messages
{
    private const string DefaultLanguage = "English";

    private readonly Dictionary<string, Dictionary<int, string>> _dictionaries = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<int, string> _languageDictionary = new();
    private string _currentLanguage = DefaultLanguage;

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set => _currentLanguage = string.IsNullOrWhiteSpace(value) ? DefaultLanguage : value.Trim();
    }

    public void ReadDictionary(string languageFilePath)
    {
        if (string.IsNullOrWhiteSpace(languageFilePath))
            throw new ArgumentException("Path must not be null or empty.", nameof(languageFilePath));

        if (!File.Exists(languageFilePath))
            throw new FileNotFoundException("Language file not found.", languageFilePath);

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
        _languageDictionary = messages;
    }

    public string GetMessage(int whichMessage)
    {
        if (_dictionaries.TryGetValue(CurrentLanguage, out var current) &&
            current.TryGetValue(whichMessage, out var text))
            return text;

        if (_dictionaries.TryGetValue(DefaultLanguage, out var fallback) &&
            fallback.TryGetValue(whichMessage, out text))
            return text;

        return $"[{whichMessage}]";
    }

    public string FormatMessage(int whichMessage, params (string Key, object Value)[] tokens)
    {
        var text = GetMessage(whichMessage);
        foreach (var (k, v) in tokens)
        {
            text = text.Replace($"{{{k}}}", v?.ToString() ?? string.Empty);
        }
        return text;
    }
}
