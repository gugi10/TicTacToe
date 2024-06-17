using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TokenConfig", menuName = "ScriptableObjects/TokenConfig", order = 1)]
public class TokenConfig : Config
{
    public string crossSpriteName;
    public string circleSpriteName;

    public string GetSpriteByPlayer(PlayerSymbol player)
    {
        if(player == PlayerSymbol.Undefined)
        {
            return null;
        }

        return player == PlayerSymbol.Cross ? crossSpriteName : circleSpriteName;
    }
}

public enum PlayerSymbol
{
    Undefined = 0,
    Cross = 1,
    Circle = 2,
}
