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
                    var effects = parser.ParseEffects();  // Esto ahora captura cualquier excepción de efecto
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
            foreach (var cardNode in validCards)
            {
                resultBuilder.AppendLine($"Name: {cardNode.Name}"); // Asegúrate de que Card tenga una propiedad Name
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
            foreach (var effectNode in validEffects)
            {
                resultBuilder.AppendLine($"Name: {effectNode.name}"); // Asegúrate de que Effect tenga una propiedad Name
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

            
            //outputText.color = Color.red;
        }
       // else
       // {
       //     // Si no hay errores, mostrar en color verde
       //     outputText.color = Color.green;
       // }

        // Mostrar el resultado en la interfaz de usuario
        outputText.text = resultBuilder.ToString();
    }
}
