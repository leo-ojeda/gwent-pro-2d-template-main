using System;
using System.Collections.Generic;

[System.Serializable]
public class Card
{
    public string Name;
    public string Type;
    public string Faction;
    public int Power;
    public string[] Range;
    public string Owner;
    public List<EffectActivation> OnActivation;

    public Card() { }

    public Card(string name, int power, string type, string[] range, string faction, List<EffectActivation> onActivation, string owner)
    {
        Name = name;
        Power = power;
        Type = type;
        Range = range;
        Faction = faction;
        OnActivation = onActivation;
        Owner = owner;
    }
}

[System.Serializable]
public class EffectActivation
{
    public Effect effect;
    public Selector selector;
    public PostAction postAction;

    //public EffectActivation() { }

    public EffectActivation(Effect effect, Selector selector, PostAction postAction)
    {
        this.effect = effect;
        this.selector = selector;
        this.postAction = postAction;
    }

    public void Activate(List<Card> targets, Context context)
    {
        effect.action?.Invoke(targets, context);
    }
}

[System.Serializable]
public class Effect
{
    public string name;
    public List<Parameter> parameters;
    public Action<List<Card>, Context> action;


    public Effect()
    {
        parameters = new List<Parameter>();
    }

    public Effect(string name, List<Parameter> parameters, Action<List<Card>, Context> action)
    {
        this.name = name;
        this.parameters = parameters ?? new List<Parameter>();
        this.action = action;
    }
}

[System.Serializable]
public class Parameter
{
    public string paramName;
    public ParamType type;
    public object value;

    public Parameter(string paramName,ParamType type, object value)
    {
        this.paramName = paramName;
        this.type = type;
        this.value = value;
    }
}

public enum ParamType
{
    Number,
    String,
    Bool
}

[System.Serializable]
public class Selector
{
    public string source;
    public bool single;
    public string predicate;

    public Selector() { }

    public Selector(string source, bool single, string predicate)
    {
        this.source = source;
        this.single = single;
        this.predicate = predicate;
    }
}

[System.Serializable]
public class PostAction
{
    public string Type { get; set; }
    public Selector Selector { get; set; }

    public PostAction(string type, Selector selector)
    {
        Type = type;
        Selector = selector;
    }
}