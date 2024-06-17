using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DependencyInjection
{
    /// <summary>
    /// Interface for dependency providers.
    /// </summary>
    public interface IDependencyProvider { }

    /// <summary>
    /// Attribute to mark fields or methods for dependency injection.
    /// </summary>
    [DefaultExecutionOrder(-10)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public sealed class InjectAttribute : Attribute { }

    /// <summary>
    /// Attribute to mark methods as providers of dependencies.
    /// </summary>
    [DefaultExecutionOrder(-10)]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : Attribute { }

    /// <summary>
    /// Handles dependency injection by scanning for providers and injectables, and resolving dependencies at runtime.
    /// </summary>
    [DefaultExecutionOrder(-10)]
    public class Injector : MonoBehaviour
    {
        /// <summary>
        /// Binding flags used to search for fields and methods.
        /// </summary>
        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Registry to store resolved dependencies.
        /// </summary>
        readonly Dictionary<Type, object> registry = new Dictionary<Type, object>();

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            var providers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID).OfType<IDependencyProvider>();
            foreach (var provider in providers)
            {
                RegisterProvider(provider);
            }

            var injectables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID).Where(IsInjectable);
            foreach (var injectable in injectables)
            {
                Inject(injectable);
            }
        }

        /// <summary>
        /// Checks if a MonoBehaviour has any injectable members.
        /// </summary>
        /// <param name="mono">The MonoBehaviour to check.</param>
        /// <returns>True if the MonoBehaviour has injectable members; otherwise, false.</returns>
        private bool IsInjectable(MonoBehaviour mono)
        {
            var members = mono.GetType().GetMembers(bindingFlags);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }

        /// <summary>
        /// Injects dependencies into the specified instance.
        /// </summary>
        /// <param name="instance">The instance to inject dependencies into.</param>
        private void Inject(object instance)
        {
            var type = instance.GetType();
            var injectibleFields = type.GetFields(bindingFlags).Where(val => Attribute.IsDefined(val, typeof(InjectAttribute)));
            foreach (var field in injectibleFields)
            {
                var fieldType = field.FieldType;
                var resolvedInstance = Resolve(field.FieldType);
                if (resolvedInstance == null)
                {
                    throw new Exception($"Failed to resolve {fieldType.Name} for {type.Name}");
                }
                field.SetValue(instance, resolvedInstance);
                Debug.Log($"Field injected {fieldType.Name} into {type.Name}");
            }

            var injectibleMethods = type.GetMethods(bindingFlags).Where(val => Attribute.IsDefined(val, typeof(InjectAttribute)));
            foreach (var method in injectibleMethods)
            {
                var parameters = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
                var resolvedInstance = parameters.Select(Resolve).ToArray();
                if (resolvedInstance.Any(val => val == null))
                {
                    throw new Exception($"Failed to resolve {type.Name}.{method.Name}");
                }
                method.Invoke(instance, resolvedInstance);
                Debug.Log($"Method injected {type.Name}.{method.Name}");
            }
        }

        /// <summary>
        /// Resolves the specified type from the registry.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The resolved instance, or null if not found.</returns>
        private object Resolve(Type type)
        {
            registry.TryGetValue(type, out var resolvedInstance);
            return resolvedInstance;
        }

        /// <summary>
        /// Registers a dependency provider by scanning its methods for those marked with the Provide attribute.
        /// </summary>
        /// <param name="provider">The dependency provider to register.</param>
        private void RegisterProvider(IDependencyProvider provider)
        {
            var methods = provider.GetType().GetMethods(bindingFlags);
            foreach (var method in methods)
            {
                if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;
                var returnType = method.ReturnType;
                var providedInstance = method.Invoke(provider, null);
                if (providedInstance != null)
                {
                    registry.Add(returnType, providedInstance);
                    Debug.Log($"Registered {returnType.Name} from {provider.GetType().Name}");
                }
                else
                {
                    throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
                }
            }
        }
    }
}
