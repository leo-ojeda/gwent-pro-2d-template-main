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

        // Verificar que la fuente del selector no sea null o vacía
        if (string.IsNullOrEmpty(selector.source))
        {
            errors.Add("Selector source cannot be null or empty.");
        }

        // Si 'single' es true, debe tener un predicado definido
        if (selector.single && selector.predicate == null)
        {
            errors.Add("If 'single' is true, a valid predicate must be defined.");
        }

        // Si hay un predicado, verificar que no sea null y sea una función válida
        if (selector.predicate != null)
        {
            // try
            // {
            //     
            //      var testCard = new Card { Faction = "Monster", Power = 0 };
            //     bool result = selector.predicate(testCard);
            //
            //     if (!result && !result) // Esto es una verificación básica, podrías personalizarla
            //     {
            //         errors.Add("The predicate function does not return a valid result.");
            //     }
            // }
            // catch (Exception ex)
            // {
            //     errors.Add($"Error evaluating predicate: {ex.Message}");
            // }
        }
        else
        {
            errors.Add("The predicate function Null.");
        }

        return errors;
    }


    public List<string> ValidatePostAction(PostAction postAction)
    {
        List<string> errors = new List<string>();

        // Verificar que el nombre del PostAction no sea null o vacío
        if (string.IsNullOrEmpty(postAction.name))
        {
            errors.Add("PostAction name cannot be null or empty.");
        }
      

        // Verificar que el selector esté correctamente definido
        if (postAction.selector == null)
        {
            errors.Add("PostAction must have a valid selector.");
        }
        else
        {
            // Validar el selector usando un método auxiliar si existe
            errors.AddRange(ValidateSelector(postAction.selector));
        }

       
        return errors;
    }


    // Verifica que el tipo de carta sea válido
    private bool ValidType(string type)
    {
        return type == "Golden" || type == "Silver" || type == "Leader" || type == "Clima" || type == "Increase";
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
