using UnityEngine;

namespace DependencyInjection
{
    public class GameLogicProvider : MonoBehaviour, IDependencyProvider 
    { 
        [Provide]
        public IGameLogic ProvideGameLogic()
        {
            return new TicTacToeGameLogic();
        }

        [Provide]
        public IPlayerAssigner ProvidePlayerAssigner()
        {
            var toReturn = new PlayerIdsAssigner();
            return toReturn;
        }
    }
}