using System.Text.RegularExpressions;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegraria;

[ApiVersion(2, 1)]
public class Telegraria : TerrariaPlugin
{
    public override string Author => "Steffo";
    public override string Name => "Telegraria";
    public override string Description =>
        "Bridge between Telegram and Terraria";
    public override Version Version => typeof(Telegraria).Assembly.GetName().Version!;

    private static readonly string ConfigPath = Path.Combine(TShock.SavePath, "telegraria.toml");

    private readonly Config _cfg;

    private readonly TelegramBotClient _bot;

    public Telegraria(Main game) : base(game)
    {
        var configText = System.IO.File.ReadAllText(ConfigPath);
        _cfg = new Config(configText);
        _bot = new TelegramBotClient(_cfg.Token);
    }

    public override void Initialize()
    {
        
        ServerApi.Hooks.ServerJoin.Register(this, OnTerrariaJoin);
        ServerApi.Hooks.ServerLeave.Register(this, OnTerrariaLeave);
        PlayerHooks.PlayerChat += OnTerrariaChat;
        _bot.OnMessage += OnTelegramChat;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerJoin.Deregister(this, OnTerrariaJoin);
            ServerApi.Hooks.ServerLeave.Deregister(this, OnTerrariaLeave);
            PlayerHooks.PlayerChat -= OnTerrariaChat;
            _bot.OnMessage -= OnTelegramChat;
        }

        base.Dispose(disposing);
    }

    private void OnTerrariaChat(PlayerChatEventArgs ev)
    {
        TShock.Utils.Broadcast($"<{ev.Player.Name}> {ev.RawText}", new Microsoft.Xna.Framework.Color(27, 226, 108));

        var task = _bot.SendTextMessageAsync(new ChatId(_cfg.ChatId), $"<b>&lt;{ev.Player.Name}&gt;</b> {ev.RawText}");
        task?.RunSynchronously();

        ev.Handled = true;
    }

    private void OnTerrariaJoin(JoinEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null) return;
        
        var task = _bot.SendTextMessageAsync(new ChatId(_cfg.ChatId), $"<b>&lt;{player.Name}&gt;</b> joined the game.");
        task?.RunSynchronously();
    }

    private void OnTerrariaLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null) return;

        var task = _bot.SendTextMessageAsync(new ChatId(_cfg.ChatId), $"<b>&lt;{player.Name}&gt;</b> left the game.");
        task?.RunSynchronously();
    }

    private async Task OnTelegramChat(Message message, UpdateType type)
    {
        if(type != UpdateType.Message) return;
        if(message.Text == null) return;
        if(message.From == null) return;
        if(message.From.Username == null) return;

        TShock.Utils.Broadcast($"<@{message.From.Username}> {message.Text}", new Microsoft.Xna.Framework.Color(40, 170, 236));
    }
}
