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

        // Preprocesar la entrada
        userInput = PreprocessInput(userInput);

        LexerStream lexerStream = new LexerStream(userInput);
        Parser parser = new Parser(lexerStream);
        SemanticChecker semanticChecker = new SemanticChecker();

        List<Card> validCards = new List<Card>();
        List<Effect> validEffects = new List<Effect>();
        List<string> errorMessages = new List<string>();

        while (!parser.IsEndOfInput())
        {
            try
            {
                if (parser.Match(TokenType.Card))
                {
                    // Parsear carta
                    Card cardNode = parser.ParseCard();

                    // Chequeo semántico de la carta
                    List<string> semanticErrors = semanticChecker.ValidateCard(cardNode);
                    if (semanticErrors.Count == 0)
                    {
                        validCards.Add(cardNode);
                    }
                    else
                    {
                        errorMessages.AddRange(semanticErrors);
                    }
                }
                else if (parser.Match(TokenType.Effect))
                {
                    // Parsear efectos
                    var effects = parser.ParseEffects();
                    validEffects.AddRange(effects);

                    // Registrar cada efecto en el singleton EffectRegistry
                    foreach (var effect in effects)
                    {
                        try
                        {
                            EffectRegistry.Instance.RegisterEffect(effect);
                        }
                        catch (Exception ex)
                        {
                            errorMessages.Add($"Error registering effect '{effect.name}': {ex.Message}");
                        }
                    }
                }
                else
                {
                    throw new Exception($"Unexpected token '{lexerStream.CurrentToken.Value}' at position {lexerStream.CurrentToken.Pos}");
                }
            }
            catch (Error lexEx)
            {
                errorMessages.Add($" {lexEx.Message}");
                parser.SkipToNextStatement();
            }
            catch (Exception ex)
            {
                errorMessages.Add($" {ex.Message} ");//at position {lexerStream.CurrentToken.Pos}");
                parser.SkipToNextStatement();
            }
        }

        // Validar efectos de cada carta después del parseo y registro
        foreach (var card in validCards)
        {
            var cardErrors = semanticChecker.ValidateCard(card);
            errorMessages.AddRange(cardErrors);
        }

        // Mostrar los resultados o errores en la UI
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
                resultBuilder.AppendLine($"- Name: {card.Name}");
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
                resultBuilder.AppendLine($"- Name: {effect.name}");
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
            for (int i = 0; i < errorMessages.Count; i++)
            {
                resultBuilder.AppendLine($"{i + 1}. {errorMessages[i]}");
            }
        }
        else
        {
            resultBuilder.AppendLine("\nNo errors found.");
        }

        // Mostrar resultados en la UI
        outputText.text = resultBuilder.ToString();
    }
}