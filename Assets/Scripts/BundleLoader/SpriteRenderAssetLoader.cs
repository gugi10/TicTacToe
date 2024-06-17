using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace BundleLoader
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRenderAssetLoader : MonoBehaviour
    {
        [SerializeField] private string spriteToLoadName;
        private SpriteRenderer spriteRenderer;

        private void OnValidate()
        {
            Assert.IsNotNull(spriteToLoadName);
        }

        private async void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if(spriteRenderer.sprite == null)
            {
                Debug.LogError($"In order to load asset from bundle set sprite name");
                return;
            }

            AssetBundleLoader.SubscribeToForceReload(LoadSprite);
            await LoadSprite();
        }

        private async Task LoadSprite()
        {
            var spriteToSet = await AssetBundleLoader.GetSprite(spriteToLoadName);

            if(spriteToSet == null)
            {
                return;
            }

            spriteRenderer.sprite = spriteToSet;
        }

        private void OnDestroy()
        {
            AssetBundleLoader.UnsubscribeToForceReload(LoadSprite);
        }

    }
}