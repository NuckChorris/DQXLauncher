using System.Collections.Immutable;
using System.Xml.Linq;
using System.Xml.XPath;
using DQXLauncher.Core.StreamObfuscator;

namespace DQXLauncher.Core.Game.ConfigFile;

public class PlayerListXml : ConfigFile
{
    private PlayerListXml() : base("dqxPlayerList.xml", 0x11, UsernameObfuscator.Factory)
    {
    }

    protected override string DefaultContents => """
                                                 <?xml version="1.0" encoding="UTF-8"?>
                                                 <DragonQuestX>
                                                     <PlayerList Version="0.9.0" LastSelect="0">
                                                     </PlayerList>
                                                 </DragonQuestX>
                                                 """;

    private XElement PlayerListNode => Document?.XPathSelectElement("//DragonQuestX/PlayerList")!;
    private XElement? TrialInfoNode => PlayerListNode.XPathSelectElement("//TrialInfo");

    private Dictionary<string, SavedPlayer> _players = new();
    public ImmutableDictionary<string, SavedPlayer> Players => _players.ToImmutableDictionary();

    public TrialPlayer? Trial
    {
        get => TrialInfoNode is null ? null : new TrialPlayer(TrialInfoNode);
        set
        {
            TrialInfoNode?.Remove();
            PlayerListNode.Add(value?.Element);
        }
    }

    /// <summary>
    ///     Remove a player from the XML file.
    /// </summary>
    /// <param name="player">The player to be removed</param>
    /// <exception cref="InvalidConfigException">The config file is structured incorrectly</exception>
    public void Add(SavedPlayer player)
    {
        if (PlayerListNode is null) throw Invalid();

        _players.Add(player.Token, player);

        if (TrialInfoNode is null)
            PlayerListNode.Add(player.Element);
        else
            TrialInfoNode.AddBeforeSelf(player.Element);
    }

    /// <summary>
    ///     Add a player to the XML file.
    /// </summary>
    /// <param name="player">The player info to be added</param>
    /// <exception cref="InvalidConfigException">The config file is structured incorrectly</exception>
    public void Remove(SavedPlayer player)
    {
        if (PlayerListNode is null) throw Invalid();
        _players.Remove(player.Token);
        player.Element.Remove();
    }

    /// <summary>
    ///     Load, parse, and validate the dqxPlayerList.xml file.
    /// </summary>
    /// <exception cref="InvalidConfigException">The config file is structured incorrectly</exception>
    public static async Task<PlayerListXml> LoadAsync()
    {
        var instance = new PlayerListXml();
        await instance._LoadAsync();
        return instance;
    }

    protected override async Task _LoadAsync()
    {
        await base._LoadAsync();
        if (Document is null) throw Invalid();
        if (PlayerListNode is null) throw Invalid();
        _players = PlayerListNode
            .XPathSelectElements("//Player")
            .Select(el => new SavedPlayer(el))
            .ToDictionary(p => p.Token);
    }

    public abstract class Player(XElement element)
    {
        public XElement Element { get; } = element;

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

    public class TrialPlayer(XElement element) : Player(element)
    {
        public TrialPlayer() : this(new XElement("TrialInfo"))
        {
        }

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
}