using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Text;
using System.IO;

public class AssetBundleLoader
{
    private static string BUNDLE_NAME = "DefaultAssetBundle";
    private static AssetBundle loadedBundle;
    private static Func<Task> _onForceReload;
    private static string loadedBundleName;

    public static async Task<Sprite> GetSprite(string spriteName)
    {
        if(loadedBundle == null)
        {
            await LoadAssetBundle(string.IsNullOrEmpty(loadedBundleName) ? BUNDLE_NAME : loadedBundleName);
            if (loadedBundle == null)
            {
                return null;
            }
        }

        var spriteToReturn = await LoadSprite(loadedBundle, spriteName);
        return spriteToReturn;
    }

    public static void SubscribeToForceReload(Func<Task> onForceReload)
    {
        _onForceReload += onForceReload;
    }

    public static void UnsubscribeToForceReload(Func<Task> onForceReload)
    {
        _onForceReload -= onForceReload;
    }

    public static async Task LoadNewBundle(string newName)
    {
        await LoadAssetBundle(newName);
        if(loadedBundle == null)
        {
            return;
        }
        await _onForceReload?.Invoke();
    }

    private static async Task<AssetBundle> LoadAssetBundle(string bundleName)
    {
        await UnloadCurrentBundle();

        loadedBundleName = bundleName;
        string bundlePath = System.IO.Path.Combine(Application.streamingAssetsPath, bundleName);

        if(!File.Exists(bundlePath))
        {
            Debug.LogError($"Bundle doesnt exsits under provided path - {bundlePath}");
            return null;
        }

        try
        {
            AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            await bundleLoadRequest;
            loadedBundle = bundleLoadRequest.assetBundle;
            var assetsName = loadedBundle.GetAllAssetNames();
            StringBuilder allnames = new();
            foreach(var name in assetsName)
            {
                allnames.Append(name);
            }
            Debug.Log($"Loaded bundle {loadedBundle.name} with assets {allnames}");
        }

        catch(Exception e)
        {
            throw new Exception(e.Message);
        }

        if (loadedBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return null;
        }

        return loadedBundle;
    }

    private static async Task UnloadCurrentBundle()
    {
        if (loadedBundle != null)
        {
            Debug.Log($"Bundle unloaded {loadedBundle.name}");
            await loadedBundle?.UnloadAsync(true);
            loadedBundle = null;
        }
    }

    private static async Task<Sprite> LoadSprite(AssetBundle assetBundle, string spriteName)
    {
        try
        {
            AssetBundleRequest assetLoadRequest = assetBundle.LoadAssetAsync<Sprite>(spriteName);
            await assetLoadRequest;

            Sprite loadedSprite = assetLoadRequest.asset as Sprite;

            if (loadedSprite != null)
            {
                Debug.Log($"Successfully loaded sprite: {spriteName}");
                return loadedSprite;
            }
            else
            {
                Debug.LogError($"Failed to load sprite: {spriteName}");
                return null;
            }
        }

        catch(Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}

