using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASTNode
{
    public abstract void Accept(IVisitor visitor);
}

public class EffectAST : ASTNode
{
    public string EffectName { get; set; }
    public List<ParameterAST> Parameters { get; set; }
    public ActionAST Action { get; set; }

    public EffectAST(string effectName, List<ParameterAST> parameters, ActionAST action)
    {
        EffectName = effectName;
        Parameters = parameters;
        Action = action;
    }

    public override void Accept(IVisitor visitor)
    {
        visitor.VisitEffect(this);
    }
}

public class ParameterAST : ASTNode
{
    public string Name { get; set; }
    public string Type { get; set; }

    public ParameterAST(string name, string type)
    {
        Name = name;
        Type = type;
    }

    public override void Accept(IVisitor visitor)
    {
        visitor.VisitParameter(this);
    }
}

public class ActionAST : ASTNode
{
    public List<ASTNode> Statements { get; set; } = new List<ASTNode>();

    public override void Accept(IVisitor visitor)
    {
        visitor.VisitAction(this);
    }
}
public interface IVisitor
{
    void VisitEffect(EffectAST effect);
    void VisitParameter(ParameterAST parameter);
    void VisitAction(ActionAST action);
}
