public interface IPlayer
{
    public int PlayerId { get; }
    public PlayerSymbol PlayerSymbol { get; }
    public IPlayer PassPriority();
    public IPlayer BlockPriority();
    public void SetPlayerData(int playerId, PlayerSymbol playerSymbol);
}
