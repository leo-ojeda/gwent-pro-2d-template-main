using UnityEngine;
using UnityEngine.UI;
using DSL.Lexer;
using DSL.Parser;
using System;
using System.Collections.Generic;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

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
        var cardDatabase = FindObjectOfType<CardDatabase>(); // Obtener la instancia de CardDatabase

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
                        // Agregar la carta solo si no está en la lista
                        if (!validCards.Any(c => c.Name == cardNode.Name))
                        {
                            validCards.Add(cardNode);
                        }
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

                    foreach (var effect in effects)
                    {
                        // Chequear si el efecto ya está en la lista de efectos válidos
                        if (!validEffects.Any(e => e.name == effect.name))
                        {
                            try
                            {
                                // Registrar el efecto en el EffectRegistry
                                EffectRegistry.Instance.RegisterEffect(effect);
                                validEffects.Add(effect);
                            }
                            catch (Exception ex)
                            {
                                errorMessages.Add($"Error registering effect '{effect.name}': {ex.Message}");
                            }
                        }
                        else
                        {
                            // Mensaje indicando que el efecto ya está en la lista
                            errorMessages.Add($"Effect '{effect.name}' is already added to the valid effects list.");
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
                errorMessages.Add($"{lexEx.Message}");
                parser.SkipToNextStatement();
            }
            catch (Exception ex)
            {
                errorMessages.Add($" {ex.Message}");
                parser.SkipToNextStatement();
            }
        }

        List<Card> cardsToUpdate = new List<Card>();

        foreach (var card in validCards.ToList())
        {
            bool hasValidEffects = true;

            foreach (var activation in card.OnActivation)
            {
                // Buscar el efecto en la lista de efectos válidos
                var validEffect = validEffects.FirstOrDefault(e => e.name == activation.effect.name);

                if (validEffect != null)
                {
                    // Crear una nueva instancia del efecto para esta activación
                    var effectInstance = new Effect(validEffect.name, validEffect.parameters, validEffect.action);

                    // Verificar si el efecto tiene parámetros antes de intentar asignarles valores
                    if (effectInstance.parameters != null)
                    {
                        foreach (var param in effectInstance.parameters)
                        {
                            Debug.Log($"Nombre del parámetro: {param.paramName}, Tipo del parámetro: {param.type}");

                            // Si el parámetro es "Amount", asignar el valor de activation.effect.parameters["Amount"]
                            if (param.paramName == "Amount" || param.paramName == "amount")
                            {
                                // Buscar el parámetro 'Amount' en los parámetros de la activación
                                var amountParam = activation.effect.parameters.FirstOrDefault(p => p.paramName == "Amount" || p.paramName == "amount");

                                if (amountParam != null)
                                {
                                    param.value = amountParam.value; // Asignar el valor de 'Amount' del efecto de la activación
                                    Debug.Log($"Valor de 'Amount' asignado: {param.value}");
                                }
                                else
                                {
                                    Debug.LogWarning("El parámetro 'Amount' no se encontró en la activación.");
                                }
                            }

                            Debug.Log(card.Name + $"Valor del parámetro: {param.value}");
                        }
                    }

                    // Asignar el efecto instanciado a la activación
                    activation.effect = effectInstance;

                    // Verificar y asignar PostAction de manera similar a Effect
                    if (activation.postAction != null)
                    {
                        var validPostAction = validEffects.FirstOrDefault(e => e.name == activation.postAction.name);

                        if (validPostAction != null)
                        {
                            // Crear una nueva instancia del PostAction para esta activación
                            var postActionInstance = new PostAction(validPostAction.name, validPostAction.parameters, validPostAction.action, activation.postAction.selector);

                            // Verificar si el PostAction tiene parámetros
                            if (postActionInstance.parameters != null)
                            {
                                foreach (var param in postActionInstance.parameters)
                                {
                                    Debug.Log($"Nombre del parámetro (PostAction): {param.paramName}, Tipo del parámetro: {param.type}");

                                    // Asignar valores de parámetros en PostAction
                                    if (param.paramName == "Amount" || param.paramName == "amount")
                                    {
                                        var amountParam = activation.postAction.parameters.FirstOrDefault(p => p.paramName == "Amount" || p.paramName == "amount");

                                        if (amountParam != null)
                                        {
                                            param.value = amountParam.value; // Asignar valor de 'Amount' del PostAction de la activación
                                            Debug.Log($"Valor de 'Amount' en PostAction asignado: {param.value}");
                                        }
                                        else
                                        {
                                            Debug.LogWarning("El parámetro 'Amount' no se encontró en el PostAction.");
                                        }
                                    }

                                    Debug.Log(card.Name + $" Valor del parámetro (PostAction): {param.value}");
                                }
                            }

                            // Asignar el PostAction instanciado a la activación
                            activation.postAction = postActionInstance;
                        }
                        else
                        {
                            errorMessages.Add($"PostAction '{activation.postAction.name}' is not in the list of valid postActions.");
                            hasValidEffects = false;
                            break;
                        }
                    }
                }
                else
                {
                    errorMessages.Add($"Effect '{activation.effect.name}' is not in the list of valid effects.");
                    hasValidEffects = false;
                    break;
                }
            }

            if (hasValidEffects)
            {
                // Si todos los efectos y postActions son válidos, añadir la carta a la lista de actualización
                cardsToUpdate.Add(card);
            }
        }





        // Actualizar la base de datos con las cartas válidas
        if (cardDatabase != null && cardsToUpdate.Count > 0)
        {
            cardDatabase.UpdateCardDatabase(cardsToUpdate);
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