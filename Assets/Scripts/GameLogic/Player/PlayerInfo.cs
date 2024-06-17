public struct PlayerInfo
{
    public PlayerSymbol PlayerSymbol { get; }
    public int PlayerId { get; }
    public bool IsAi { get; }

    public PlayerInfo(PlayerSymbol playerSymbol, int playerId, bool isAi)
    {
        PlayerSymbol = playerSymbol;
        PlayerId = playerId;
        IsAi = isAi;
    }
}
