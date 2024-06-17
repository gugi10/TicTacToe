using System.Collections.Generic;
using Auxiliary.Observables;

public interface IPlayerAssigner 
{
    public ObservableProperty<List<PlayerInfo>> CurrentPlayers { get; }
    public void AssignLocalPlayers();
    public void AssingLocalVsAi();
}
