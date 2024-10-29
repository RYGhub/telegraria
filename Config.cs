using Tomlyn;
using Tomlyn.Model;

namespace Telegraria
{
    public class Config
    {
        public string Token { get; }
        public string ChatId { get; }

        public Config(string configText)
        {
            var document = Toml.ToModel(configText);
            Token = (string)document["token"];
            ChatId = (string)document["chat_id"];
        }
    }
}