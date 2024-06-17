using UnityEngine;
using System.Threading.Tasks;

public class Token : MonoBehaviour
{
    private SpriteRenderer _tokenSprite;
    private GameBoardPosition _gridPos;
    private string _spriteToLoadName;
    private bool isSubscribed;

    public void Setup(GameBoardPosition gridPos)
    {
        if (!isSubscribed)
        {
            isSubscribed = true;
            AssetBundleLoader.SubscribeToForceReload(LoadSprite);
        }
        _tokenSprite = GetComponent<SpriteRenderer>();
        _gridPos = gridPos;
    }

    public void Hide()
    {
        _tokenSprite.enabled = false;
    }

    public async void SetSprite(string sprite)
    {
        _spriteToLoadName = sprite;
        await LoadSprite();
    }

    public GameBoardPosition TokenClicked()
    {
        return _gridPos;
    }

    private async Task LoadSprite()
    {
        if (string.IsNullOrEmpty(_spriteToLoadName))
        {
            return;
        }

        var spriteToSet = await AssetBundleLoader.GetSprite(_spriteToLoadName);

        if (spriteToSet == null)
        {
            return;
        }

        _tokenSprite.sprite = spriteToSet;
        _tokenSprite.enabled = true;
    }

    private void OnDestroy()
    {
        AssetBundleLoader.UnsubscribeToForceReload(LoadSprite);
    }
}
