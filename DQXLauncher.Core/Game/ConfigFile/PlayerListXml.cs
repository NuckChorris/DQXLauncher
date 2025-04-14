using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Xml.XPath;
using DQXLauncher.Core.StreamObfuscator;

namespace DQXLauncher.Core.Game.ConfigFile;

public class PlayerListXml : ConfigFile
{
    public abstract class Player(XElement element)
    {
        protected XElement Element { get; } = element;

        protected InvalidConfigException Invalid()
        {
            return new InvalidConfigException(Element.ToString());
        }
    }

    public class SavedPlayer(XElement element) : Player(element)
    {
        public SavedPlayer() : this(new XElement("Player"))
        {
        }

        public int Number
        {
            get
            {
                if (Element.Attribute("Number")?.Value is { } rawNumber &&
                    int.TryParse(rawNumber, out var number))
                    return number;

                throw Invalid();
            }
            set => Element.SetAttributeValue("Number", value);
        }

        public string Token
        {
            get
            {
                if (Element.Attribute("Token")?.Value is { } token) return token;

                throw Invalid();
            }
            set => Element.SetAttributeValue("Token", value);
        }
    }

    // We don't yet support Easy Play accounts, so this is mostly just a placeholder for now.
    // When signing in, the ID is passed as deviceid, and Token is passed as easyplayid. The purpose of Code is unknown,
    // as is how to generate these values initially. They do not seem to be provided in the HTTP exchanges in the
    // Launcher, and the Game also does not appear to be setting them. More research is needed.
    public class TrialPlayer(XElement element) : Player(element)
    {
        public string Id
        {
            get
            {
                if (Element.Attribute("ID")?.Value is { } id) return id;

                throw Invalid();
            }
            set => Element.SetAttributeValue("ID", value);
        }

        public string Token
        {
            get
            {
                if (Element.Attribute("Token")?.Value is { } token) return token;

                throw Invalid();
            }
            set => Element.SetAttributeValue("Token", value);
        }

        public string Code
        {
            get
            {
                if (Element.Attribute("Code")?.Value is { } code) return code;

                throw Invalid();
            }
            set => Element.SetAttributeValue("Code", value);
        }
    }


    protected override string DefaultContents => """
                                                 <?xml version="1.0" encoding="UTF-8"?>
                                                 <DragonQuestX>
                                                     <PlayerListXml Version="0.9.0" LastSelect="0">
                                                     </PlayerListXml>
                                                 </DragonQuestX>
                                                 """;

    private XElement? PlayerListNode => Document?.XPathSelectElement("//DragonQuestX/PlayerListXml");
    public ObservableCollection<Player> Players { get; private set; } = new();

    public void Add(Player player)
    {
        if (PlayerListNode is null) throw Invalid();
        PlayerListNode.Add(player);
        Players.Add(player);
    }

    public void Remove(Player player)
    {
        if (PlayerListNode is null) throw Invalid();
        Players.Remove(player);
    }

    public IReadOnlyList<Player> GetPlayers()
    {
        return Players.AsReadOnly();
    }

    private PlayerListXml() : base("dqxPlayerList.xml", 0x11, UsernameObfuscator.Factory)
    {
    }

    /// <summary>
    /// Load, parse, and validate the PlayerListXml.xml file.
    /// </summary>
    /// <exception cref="InvalidConfigException"></exception>
    public override async Task Load()
    {
        await base.Load();
        if (Document is null) throw Invalid();
        if (PlayerListNode is null) throw Invalid();

        // Build list of players from XML.
        foreach (var el in PlayerListNode.Elements())
        {
            Player player = el.Name.LocalName switch
            {
                "Player" => new SavedPlayer(el),
                "TrialInfo" => new TrialPlayer(el),
                _ => throw Invalid()
            };
            Players.Add(player);
        }
    }

    public static async Task<PlayerListXml> OpenAsync()
    {
        var instance = new PlayerListXml();
        await instance.Load();
        return instance;
    }
}