using System.Text.Json;
using System.IO;
using System.Collections.Generic;

namespace CYOA;
public class Messages
{
    private const string DefaultLanguage = "English";

    private readonly Dictionary<string, Dictionary<int, string>> _dictionaries = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<int, string> languageDictionary = new();
    private string currentLanguage = DefaultLanguage;

    public string CurrentLanguage
    {
        get => currentLanguage;
        set => currentLanguage = string.IsNullOrWhiteSpace(value) ? DefaultLanguage : value.Trim();
    }

    /// <summary>
 
    /// </summary>
    public void ReadDictionary(string languageFilePath)
    {
        if (string.IsNullOrWhiteSpace(languageFilePath))
            throw new ArgumentException("Path must not be null or empty.", nameof(languageFilePath));
        if (!File.Exists(languageFilePath))
            throw new FileNotFoundException("Language file not found.", languageFilePath);

        var loaded = new Dictionary<int, string>();
        var lineNo = 0;
        foreach (var raw in File.ReadLines(languageFilePath))
        {
            lineNo++;
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;

            var eq = line.IndexOf('=');
            if (eq <= 0 || !int.TryParse(line[..eq].Trim(), out var id))
                throw new FormatException($"Line {lineNo}: expected 'id=text'.");

            loaded[id] = line[(eq + 1)..].Trim();
        }

        _dictionaries[CurrentLanguage] = loaded;
        languageDictionary = loaded;
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
}