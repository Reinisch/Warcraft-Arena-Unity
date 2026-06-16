using Core;

namespace Net
{
    /// <summary>Client → server: play an emote. (Bolt: PlayerEmoteRequestEvent)</summary>
    public readonly struct PlayerEmoteRequest : INetMessage
    {
        public EmoteType EmoteType { get; }

        public PlayerEmoteRequest(EmoteType emoteType)
        {
            EmoteType = emoteType;
        }
    }
}
