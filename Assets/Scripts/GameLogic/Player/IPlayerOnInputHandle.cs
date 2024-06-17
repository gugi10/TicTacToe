using System;

public interface IPlayerOnInputHandle
{
    public void SetCallback(Action<IPlayer, GameBoardPosition> onSuccessfullInput);
}
