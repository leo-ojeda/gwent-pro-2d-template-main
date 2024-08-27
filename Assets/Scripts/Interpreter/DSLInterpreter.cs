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
        List<string> errorMessages = new List<string>();

        while (!parser.IsEndOfInput())
        {
            try
            {
                // Intentar parsear una carta
                var cardNode = parser.ParseCard();
                validCards.Add(cardNode);
            }
            catch (LexerError lexEx)
            {
                // Capturar el error del lexer y añadirlo a la lista de mensajes de error
                errorMessages.Add($"Lexer Error: {lexEx.Message}\nPosition: {lexEx.Position}");
                
                // Saltar el resto del input hasta la próxima carta o final del input
                parser.SkipToNextCard();
            }
            catch (Exception ex)
            {
                // Capturar errores generales y añadirlos a la lista de mensajes de error
                errorMessages.Add($"Error parsing input: {ex.Message}");
                
                // Saltar el resto del input hasta la próxima carta o final del input
                parser.SkipToNextCard();
            }
        }

        // Mostrar cartas válidas y errores, si los hay
        DisplayResults(validCards, errorMessages);
    }

    private static string PreprocessInput(string input)
    {
        // Reemplazar múltiples espacios con un solo espacio, pero conservar saltos de línea
        return Regex.Replace(input, @"[ \t]+", " ").Trim();
    }

    private void DisplayResults(List<Card> validCards, List<string> errorMessages)
    {
        StringBuilder resultBuilder = new StringBuilder();

        // Mostrar cartas válidas
        if (validCards.Count > 0)
        {
            resultBuilder.AppendLine("Cards parsed successfully:");
            foreach (var cardNode in validCards)
            {
                resultBuilder.AppendLine(cardNode.Name); // Asumiendo que Card tiene un método ToString
            }
        }
        else
        {
            resultBuilder.AppendLine("No valid cards found.");
        }

        // Mostrar errores
        if (errorMessages.Count > 0)
        {
            resultBuilder.AppendLine("\nErrors found:");
            foreach (var error in errorMessages)
            {
                resultBuilder.AppendLine(error);
            }

            // Mostrar errores en color rojo
            outputText.color = Color.red;
        }
        else
        {
            // Si no hay errores, mostrar en color verde
            outputText.color = Color.green;
        }

        // Mostrar el resultado en la interfaz de usuario
        outputText.text = resultBuilder.ToString();
    }
}
