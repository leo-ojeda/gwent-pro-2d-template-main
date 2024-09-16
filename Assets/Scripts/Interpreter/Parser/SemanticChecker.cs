using System;
using System.Collections.Generic;

public class SemanticChecker
{
    public List<string> ValidateCard(Card card)
    {
        List<string> errors = new List<string>();

        // Verificar que el tipo sea válido
        if (!ValidType(card.Type))
        {
            errors.Add($"Invalid type '{card.Type}'. Expected one of: Oro, Plata, Lider, Clima, Aumento.");
        }

        // Verificar que el nombre no sea null o vacío
        if (string.IsNullOrEmpty(card.Name))
        {
            errors.Add("The 'Name' property cannot be null or empty.");
        }

        // Verificar que la facción no sea null
        if (string.IsNullOrEmpty(card.Faction))
        {
            errors.Add("The 'Faction' property cannot be null.");
        }

        // Verificar que el poder sea mayor que 0
        if (card.Power <= 0)
        {
            errors.Add("The 'Power' property must be greater than 0.");
        }

        // Verificar que el rango contenga al menos un valor
        if (card.Range == null || card.Range.Length == 0)
        {
            errors.Add("The 'Range' property must have at least one value.");
        }
        else
        {
            foreach (var range in card.Range)
            {
                if (range != "M" && range != "R" && range != "S")
                {
                    errors.Add($"Invalid range '{range}'. Expected one of: Melee, Ranged, Siege.");
                }
            }
        }

        // Validar cada EffectActivation
        foreach (var activation in card.OnActivation)
        {
            errors.AddRange(ValidateEffectActivation(activation));
        }

        return errors;
    }

    public List<string> ValidateEffectActivation(EffectActivation activation)
    {
        List<string> errors = new List<string>();

        // Verificar que el efecto esté registrado
        if (activation.effect == null || !EffectRegistry.Instance.IsEffectRegistered(activation.effect.name))
        {
            errors.Add($"Effect '{activation.effect?.name}' is not registered.");
        }
        else
        {
            errors.AddRange(ValidateEffect(activation.effect));
        }

        // Verificar que si hay un selector, esté correctamente definido
        if (activation.selector != null)
        {
            errors.AddRange(ValidateSelector(activation.selector));
        }

        // Verificar que si hay una post-action, esté correctamente definida
        if (activation.postAction != null)
        {
            errors.AddRange(ValidatePostAction(activation.postAction));
        }

        return errors;
    }

    public List<string> ValidateEffect(Effect effect)
    {
        List<string> errors = new List<string>();

        // Verificar que el nombre del efecto no sea null o vacío
        if (string.IsNullOrEmpty(effect.name))
        {
            errors.Add("Effect name cannot be null or empty.");
        }

        // Verificar que los parámetros no sean null
        foreach (var param in effect.parameters)
        {
            if (param == null)
            {
                errors.Add($"Parameter in effect '{effect.name}' cannot be null.");
            }
        }

        return errors;
    }

    public List<string> ValidateSelector(Selector selector)
    {
        List<string> errors = new List<string>();

        // Verificar que la fuente del selector no sea null
        if (string.IsNullOrEmpty(selector.source))
        {
            errors.Add("Selector source cannot be null.");
        }

        // Si es único o tiene predicado, asegurarse que estén correctamente definidos
        if (selector.single && string.IsNullOrEmpty(selector.predicate))
        {
            errors.Add("If 'single' is true, predicate must be defined.");
        }

        return errors;
    }

    public List<string> ValidatePostAction(PostAction postAction)
    {
        List<string> errors = new List<string>();

        // Verificar que el tipo de la post-action no sea null
        if (string.IsNullOrEmpty(postAction.Type))
        {
            errors.Add("PostAction type cannot be null.");
        }

        // Verificar que el selector esté correctamente definido
        if (postAction.Selector == null)
        {
            errors.Add("PostAction must have a valid selector.");
        }
        else
        {
            errors.AddRange(ValidateSelector(postAction.Selector));
        }

        return errors;
    }

    // Verifica que el tipo de carta sea válido
    private bool ValidType(string type)
    {
        return type == "Oro" || type == "Plata" || type == "Lider" || type == "Clima" || type == "Aumento";
    }

    // Verifica tipos en expresiones, asignaciones, y operadores
    public void CheckAssignation(string variableType, string expressionType)
    {
        if (variableType != expressionType)
        {
            Console.WriteLine($"Type mismatch: cannot assign {expressionType} to {variableType}");
        }
    }

    public string CheckBinaryOperator(string leftType, string rightType, string operatorType)
    {
        if (operatorType == "+" || operatorType == "-")
        {
            if ((leftType == "int" || leftType == "float") && (rightType == "int" || rightType == "float"))
            {
                return leftType;  // Retornar el tipo del operador (supongamos que coinciden)
            }
            else
            {
                return "error";  // Error de tipos
            }
        }
        return "unknown";
    }
}