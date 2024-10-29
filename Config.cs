using Tomlyn;

namespace Telegraria
{
    public class Config
    {
        public string Token { get; }
        public long ChatId { get; }

        public Config(string configText)
        {
            var document = Toml.ToModel(configText);
            Token = (string)document["token"];
            ChatId = (long)document["chat_id"];
        }
    }
}