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
    /// Throws ArgumentException on a null/empty path, FileNotFoundException if the file is missing,
    /// FormatException on a malformed line.
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