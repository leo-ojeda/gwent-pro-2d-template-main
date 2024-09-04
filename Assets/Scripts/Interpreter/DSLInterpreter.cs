using UnityEngine;
using UnityEngine.UI;
using DSL.Lexer;
using DSL.Parser;
using System;
using System.Collections.Generic;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;

public class DSLInterpreter : MonoBehaviour
{
    public TMP_InputField inputField;
    public Text outputText;

    public void InterpretDSL()
    {
        string userInput = inputField.text;

        // Preprocesar la entrada para eliminar espacios y saltos de línea innecesarios
        userInput = PreprocessInput(userInput);

        LexerStream lexerStream = new LexerStream(userInput);
        Parser parser = new Parser(lexerStream);

        List<Card> validCards = new List<Card>();
        List<Effect> validEffects = new List<Effect>();
        List<string> errorMessages = new List<string>();

        while (!parser.IsEndOfInput())
        {
            try
            {
                if (parser.Match(TokenType.Card))
                {
                    var cardNode = parser.ParseCard();
                    validCards.Add(cardNode);
                }
                else if (parser.Match(TokenType.Effect))
                {
                    var effects = parser.ParseEffects();
                    validEffects.AddRange(effects);
                }
                else
                {
                    throw new Exception($"Unexpected token '{lexerStream.CurrentToken.Value}'");
                }
            }
            catch (LexerError lexEx)
            {
                errorMessages.Add($"Lexer Error: {lexEx.Message}\nPosition: {lexEx.Position}");
                parser.SkipToNextStatement();
            }
            catch (Exception ex)
            {
                errorMessages.Add($"Error parsing input: {ex.Message}");
                parser.SkipToNextStatement();
            }
        }

        // Mostrar resultados
        DisplayResults(validCards, validEffects, errorMessages);
    }

    private static string PreprocessInput(string input)
    {
        // Reemplazar múltiples espacios con un solo espacio, pero conservar saltos de línea
        return Regex.Replace(input, @"[ \t]+", " ").Trim();
    }

    private void DisplayResults(List<Card> validCards, List<Effect> validEffects, List<string> errorMessages)
    {
        StringBuilder resultBuilder = new StringBuilder();

        // Mostrar cartas válidas
        if (validCards.Count > 0)
        {
            resultBuilder.AppendLine("Cards parsed successfully:");
            foreach (var card in validCards)
            {
                resultBuilder.AppendLine($"Name: {card.Name}");
               // resultBuilder.AppendLine($"Type: {card.Type}");
               // resultBuilder.AppendLine($"Faction: {card.Faction}");
               // resultBuilder.AppendLine($"Power: {card.Power}");
               // resultBuilder.AppendLine($"Range: {string.Join(", ", card.Range)}");
               // resultBuilder.AppendLine($"Owner: {card.Owner}");

                // Mostrar detalles de OnActivation
               // resultBuilder.AppendLine("On Activation:");
               // if (card.OnActivation.Count > 0)
               // {
               //     foreach (var activation in card.OnActivation)
               //     {
               //         // Mostrar detalles del efecto
               //         resultBuilder.AppendLine($"  Effect: {activation.effect.name}");
//
               //         // Mostrar detalles del selector
               //         if (activation.selector != null)
               //         {
               //             resultBuilder.AppendLine($"  Selector Source: {activation.selector.source}");
               //             resultBuilder.AppendLine($"  Selector Single: {activation.selector.single}");
               //             resultBuilder.AppendLine($"  Selector Predicate: {activation.selector.predicate}");
               //         }
//
               //         // Mostrar detalles de la acción posterior (PostAction)
               //         if (activation.postAction != null)
               //         {
               //             resultBuilder.AppendLine($"  Post Action Type: {activation.postAction.Type}");
               //             if (activation.postAction.Selector != null)
               //             {
               //                 resultBuilder.AppendLine($"  Post Action Selector Source: {activation.postAction.Selector.source}");
               //                 resultBuilder.AppendLine($"  Post Action Selector Single: {activation.postAction.Selector.single}");
               //                 resultBuilder.AppendLine($"  Post Action Selector Predicate: {activation.postAction.Selector.predicate}");
               //             }
               //         }
//
               //         resultBuilder.AppendLine(); // Línea en blanco para separar cada EffectActivation
               //     }
               // }
               // else
               // {
               //     resultBuilder.AppendLine("  No effects defined.");
               // }

               // resultBuilder.AppendLine(); // Línea en blanco para separar cada carta
            }
        }
        else
        {
            resultBuilder.AppendLine("No valid cards found.");
        }

        // Mostrar efectos válidos
        if (validEffects.Count > 0)
        {
            resultBuilder.AppendLine("\nEffects parsed successfully:");
            foreach (var effect in validEffects)
            {
                resultBuilder.AppendLine($"Name: {effect.name}");

                // Mostrar parámetros del efecto
              //  if (effect.parameters != null && effect.parameters.Count > 0)
              //  {
              //      resultBuilder.AppendLine("  Parameters:");
              //      foreach (var parameter in effect.parameters)
              //      {
              //          resultBuilder.AppendLine($"    Param Name: {parameter.paramName}, Type: {parameter.type}, Value: {parameter.value}");
              //      }
              //  }
               // else
               // {
               //     resultBuilder.AppendLine("  No parameters defined.");
               // }
               //
               // if (effect.action != null)
               // {
               //     resultBuilder.AppendLine("  Action: Defined"); 
               // }
               // else
               // {
               //     resultBuilder.AppendLine("  Action: Not defined");
               // }

                //resultBuilder.AppendLine(); // Línea en blanco para separar cada efecto
            }
        }
        else
        {
            resultBuilder.AppendLine("No valid effects found.");
        }

        // Mostrar errores
        if (errorMessages.Count > 0)
        {
            resultBuilder.AppendLine("\nErrors found:");
            foreach (var error in errorMessages)
            {
                resultBuilder.AppendLine(error);
            }
        }

        outputText.text = resultBuilder.ToString();
    }

}