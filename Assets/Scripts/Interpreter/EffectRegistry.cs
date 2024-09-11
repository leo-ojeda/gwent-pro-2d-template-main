using System;
using System.Collections.Generic;
using System.Linq;

public class EffectRegistry
{
    private static EffectRegistry _instance;
    private readonly Dictionary<string, Effect> _effects;

    private EffectRegistry()
    {
        _effects = new Dictionary<string, Effect>();
    }

    public static EffectRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EffectRegistry();
            }
            return _instance;
        }
    }

    public void RegisterEffect(Effect effect)
    {
        if (effect == null)
        {
            throw new ArgumentNullException(nameof(effect), "Effect cannot be null.");
        }

        if (string.IsNullOrEmpty(effect.name))
        {
            throw new ArgumentException("Effect name cannot be null or empty.", nameof(effect));
        }

       // if (_effects.ContainsKey(effect.name))
       // {
       //     throw new InvalidOperationException($"Effect with name '{effect.name}' is already registered.");
       // }

        _effects[effect.name] = effect;
    }

    public bool IsEffectRegistered(string effectName)
    {
        return _effects.ContainsKey(effectName);
    }

    public Effect GetEffect(string effectName)
    {
        if (_effects.TryGetValue(effectName, out var effect))
        {
            return effect;
        }
        throw new KeyNotFoundException($"Effect with name '{effectName}' not found.");
    }
}
