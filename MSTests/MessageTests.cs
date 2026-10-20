using CYOA;

namespace CYOA.Tests;

[TestClass]
public class MessagesTests
{
    private const string English = "English";
    private const string French = "French";
    private const string Spanish = "Spanish";

    private string _dir = null!;

    [TestInitialize]
    public void Setup()
    {
        _dir = Path.Combine(Path.GetTempPath(), "cyoa_" + Guid.NewGuid());
        Directory.CreateDirectory(_dir);
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_dir, true);

    private string Write(string name, params string[] lines)
    {
        var path = Path.Combine(_dir, name);
        File.WriteAllLines(path, lines);
        return path;
    }

    private Messages Loaded()
    {
        var m = new Messages();
        m.CurrentLanguage = English;
        m.ReadDictionary(Write("en.txt", "1=Welcome to the adventure!", "2=Game over.", "3=English only"));
        m.CurrentLanguage = French;
        m.ReadDictionary(Write("fr.txt", "1=Bienvenue dans l'aventure !", "2=Partie terminée."));
        m.CurrentLanguage = Spanish;
        m.ReadDictionary(Write("es.txt", "1=¡Bienvenido a la aventura!", "2=Fin del juego."));
        return m;
    }

    [TestMethod]
    [DataRow(English, "Welcome to the adventure!")]
    [DataRow(French, "Bienvenue dans l'aventure !")]
    [DataRow(Spanish, "¡Bienvenido a la aventura!")]
    public void GetMessage_ValidId_ReturnsLocalizedText(string lang, string expected)
    {
        var m = Loaded();
        m.CurrentLanguage = lang;
        Assert.AreEqual(expected, m.GetMessage(1));
    }

    [TestMethod]
    public void GetMessage_MissingInLanguage_FallsBackToEnglish()
    {
        var m = Loaded();
        m.CurrentLanguage = Spanish;
        Assert.AreEqual("English only", m.GetMessage(3));
    }

    [TestMethod]
    public void GetMessage_MissingEverywhere_ReturnsBracketedId()
    {
        var m = Loaded();
        Assert.AreEqual("[999]", m.GetMessage(999));
    }

    [TestMethod]
    public void GetMessage_LanguageNotLoaded_FallsBackToEnglish()
    {
        var m = Loaded();
        m.CurrentLanguage = "Klingon";
        Assert.AreEqual("Game over.", m.GetMessage(2));
    }

    [TestMethod]
    public void GetMessage_NothingLoaded_ReturnsBracketedId() =>
        Assert.AreEqual("[1]", new Messages().GetMessage(1));

    [TestMethod]
    public void CurrentLanguage_Blank_ResetsToEnglish()
    {
        var m = new Messages { CurrentLanguage = "  " };
        Assert.AreEqual("English", m.CurrentLanguage);
    }

    [TestMethod]
    public void ReadDictionary_IgnoresCommentsAndBlankLines()
    {
        var m = new Messages();
        m.ReadDictionary(Write("c.txt", "# comment", "", "1=Hi"));
        Assert.AreEqual("Hi", m.GetMessage(1));
    }

    [TestMethod]
    public void ReadDictionary_TextMayContainEquals()
    {
        var m = new Messages();
        m.ReadDictionary(Write("e.txt", "1=a=b"));
        Assert.AreEqual("a=b", m.GetMessage(1));
    }

    [TestMethod]
    public void ReadDictionary_MissingFile_Throws() =>
        Assert.ThrowsExactly<FileNotFoundException>(() =>
            new Messages().ReadDictionary(Path.Combine(_dir, "none.txt")));

    [TestMethod]
    public void ReadDictionary_MalformedLine_Throws()
    {
        var path = Write("bad.txt", "oops");
        Assert.ThrowsExactly<FormatException>(() => new Messages().ReadDictionary(path));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    public void ReadDictionary_BadPath_Throws(string? path) =>
        Assert.ThrowsExactly<ArgumentException>(() => new Messages().ReadDictionary(path!));
}