using UnityEngine;
using UnityEngine.UI;
using DSL.Lexer;
using DSL.Parser;
using System;
using TMPro;
using System.Text.RegularExpressions;

public class DSLInterpreter : MonoBehaviour
{
    public TMP_InputField inputField;
    public Text outputText;

    public void InterpretDSL()
    {
        string userInput = inputField.text;

        // Preprocesar la entrada para eliminar espacios y saltos de línea innecesarios
        userInput = PreprocessInput(userInput);

        try
        {
            LexerStream lexerStream = new LexerStream(userInput);
            Parser parser = new Parser(lexerStream);
            var cardNode = parser.ParseCard();

            DisplaySuccess(cardNode);
        }
        catch (LexerError lexEx)
        {
            DisplayError($"Lexer Error: {lexEx.Message}\nPosition: {lexEx.Position}", Color.red);
        }
        catch (Exception ex)
        {
            DisplayError($"Error parsing input: {ex.Message}", Color.red);
        }
    }

    private static string PreprocessInput(string input)
    {
        // Reemplazar múltiples espacios con un solo espacio, pero conservar saltos de línea
        return Regex.Replace(input, @"[ \t]+", " ").Trim();
    }

    private void DisplaySuccess(object result)
    {
        outputText.color = Color.green;
        outputText.text = $"Card parsed successfully: {result}";
    }

    private void DisplayError(string message, Color color)
    {
        outputText.color = color;
        outputText.text = $"{message}\nInput: {inputField.text}";
    }
}
