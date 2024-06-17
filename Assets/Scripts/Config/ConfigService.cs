using System.Collections.Generic;
using UnityEngine;

public interface IConfigService 
{
    public T GetConfig<T>() where T : class, IConfig;
}

public class ConfigService : IConfigService
{
    private List<Config> _configs = new();

    public ConfigService(List<Config> configs)
    {
        _configs = configs;
    }

    public T GetConfig<T>() where T : class, IConfig
    {
        var config = _configs.Find(x => x is T) as T;

        if(config == null)
        {
            Debug.LogError($"Config: {typeof(T)} is not defined in ConfigProvider");
        }

        return config;
    }
}

