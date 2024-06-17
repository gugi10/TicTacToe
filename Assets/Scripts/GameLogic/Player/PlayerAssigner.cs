using System.Collections.Generic;
using Auxiliary.Observables;

/// <summary>
/// Handles assignment of player IDs and information for a game session.
/// Implements the IPlayerAssigner interface.
/// </summary>
public partial class PlayerIdsAssigner : IPlayerAssigner
{
    private const int PLAYER_AMOUNT = 2;

    /// <summary>
    /// Gets the observable property containing the list of current players.
    /// </summary>
    public ObservableProperty<List<PlayerInfo>> CurrentPlayers { get; } = new();

    /// <summary>
    /// Initializes a new instance of the PlayerIdsAssigner class.
    /// </summary>
    public PlayerIdsAssigner() { }

    /// <summary>
    /// Assigns local players with randomly generated unique IDs and assigns them to CurrentPlayers.
    /// </summary>
    public void AssignLocalPlayers()
    {
        var randomizedIds = GenerateUniqueRandomIds(PLAYER_AMOUNT);
        var listToSet = new List<PlayerInfo>();
        for (int i = 1; i <= PLAYER_AMOUNT; i++)
        {
            listToSet.Add(new PlayerInfo((PlayerSymbol)i, randomizedIds[i - 1], false));
        }
        CurrentPlayers.Set(listToSet);
    }

    /// <summary>
    /// Assigns one local player and one AI player with randomly generated unique IDs and assigns them to CurrentPlayers.
    /// </summary>
    public void AssingLocalVsAi()
    {
        var randomizedIds = GenerateUniqueRandomIds(PLAYER_AMOUNT);
        var listToSet = new List<PlayerInfo>();
        for (int i = 1; i <= PLAYER_AMOUNT; i++)
        {
            listToSet.Add(new PlayerInfo((PlayerSymbol)i, randomizedIds[i - 1], randomizedIds[i - 1] == 2));
        }
        CurrentPlayers.Set(listToSet);
    }

    /// <summary>
    /// Generates a list of unique random IDs based on the specified count.
    /// </summary>
    /// <param name="count">The number of unique IDs to generate.</param>
    /// <returns>A list containing the generated unique random IDs.</returns>
    private List<int> GenerateUniqueRandomIds(int count)
    {
        List<int> availableIds = new List<int>();
        for (int i = 1; i <= PLAYER_AMOUNT; i++)
        {
            availableIds.Add(i);
        }
        availableIds.Shuffle(); // Shuffle the list to randomize IDs

        return availableIds;
    }
}
