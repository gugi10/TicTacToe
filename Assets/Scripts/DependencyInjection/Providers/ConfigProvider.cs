using DependencyInjection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigProvider : MonoBehaviour, IDependencyProvider
{
    [SerializeField] private List<Config> configs;

    [Provide]
    public IConfigService ProvideConfigService()
    {
        return new ConfigService(configs);
    }

}