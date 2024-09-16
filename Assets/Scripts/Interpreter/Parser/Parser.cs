using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DSL.Lexer;
using System.Linq;

namespace DSL.Parser
{

    public class Parser
    {
        private Context Context;
        private readonly LexerStream _lexerStream;

        public Parser(LexerStream lexerStream)
        {
            _lexerStream = lexerStream;
        }

        //public List<Card> ParseCards()
        //{
        //    List<Card> cards = new List<Card>();
        //    while (!IsEndOfInput())
        //    {
        //        try
        //        {
        //            var card = ParseCard();
        //            cards.Add(card);
        //        }
        //        catch (Error)
        //        {
        //            SkipToNextStatement();
        //        }
        //        catch (Exception)
        //        {
        //            SkipToNextStatement();
        //        }
        //    }
        //
        //    // Crear una instancia de SemanticChecker para verificar las cartas
        //    SemanticChecker checker = new SemanticChecker();
        // checker.CheckCards(cards);
        //
        //    return cards;
        //}

        public bool IsEndOfInput()
        {
            return _lexerStream.CurrentToken.Type == TokenType.EOF;
        }

        public void SkipToNextStatement()
        {
            while (!IsEndOfInput() && _lexerStream.CurrentToken.Type != TokenType.Card && _lexerStream.CurrentToken.Type != TokenType.Effect)
            {
                _lexerStream.NextToken();
            }
        }


        public Card ParseCard()
        {
            try
            {
                if (_lexerStream.CurrentToken.Type != TokenType.Card)
                {
                    ThrowSyntaxError($"Expected 'card' but found '{_lexerStream.CurrentToken.Value}'");
                }

                Consume(TokenType.Card);
                Consume(TokenType.OpenBrace);

                string type = null;
                string name = null;
                string faction = null;
                int power = 0;
                List<string> range = new List<string>();
                List<EffectActivation> onActivation = new List<EffectActivation>();

                // Diccionario para controlar qué propiedades ya se han definido
                var definedProperties = new HashSet<TokenType>();

                while (!Match(TokenType.CloseBrace))
                {
                    if (Match(TokenType.Type))
                    {
                        if (definedProperties.Contains(TokenType.Type))
                        {
                            ThrowSyntaxError("The 'Type' property has already been defined.");
                        }
                        type = ParseProperty(TokenType.Type);
                        definedProperties.Add(TokenType.Type);

                        // Validar el valor de type
                        if (!ValidType(type))
                        {
                            ThrowSyntaxError($"Invalid type '{type}'. Expected one of: Oro, Plata, Lider, Clima, Aumento.");
                        }

                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Name))
                    {
                        if (definedProperties.Contains(TokenType.Name))
                        {
                            ThrowSyntaxError("The 'Name' property has already been defined.");
                        }
                        name = ParseProperty(TokenType.Name);
                        definedProperties.Add(TokenType.Name);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Faction))
                    {
                        if (definedProperties.Contains(TokenType.Faction))
                        {
                            ThrowSyntaxError("The 'Faction' property has already been defined.");
                        }
                        faction = ParseProperty(TokenType.Faction);
                        definedProperties.Add(TokenType.Faction);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Power))
                    {
                        if (definedProperties.Contains(TokenType.Power))
                        {
                            ThrowSyntaxError("The 'Power' property has already been defined.");
                        }
                        power = ParseIntegerProperty(TokenType.Power);
                        definedProperties.Add(TokenType.Power);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Range))
                    {
                        if (definedProperties.Contains(TokenType.Range))
                        {
                            ThrowSyntaxError("The 'Range' property has already been defined.");
                        }
                        range = ParseListProperty(TokenType.Range);
                        definedProperties.Add(TokenType.Range);

                        // Validar y mapear cada valor de range
                        for (int i = 0; i < range.Count; i++)
                        {
                            string r = range[i];
                            if (range[i] == "Melee")
                            {
                                range[i] = "M";
                            }
                            else if (range[i] == "Ranged")
                            {
                                range[i] = "R";
                            }
                            else if (range[i] == "Siege")
                            {
                                range[i] = "S";
                            }
                            else
                            {
                                ThrowSyntaxError($"Invalid range '{r}'. Expected one of: Melee, Ranged, Siege.");
                            }
                        }

                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.OnActivation))
                    {
                        if (definedProperties.Contains(TokenType.OnActivation))
                        {
                            ThrowSyntaxError("The 'OnActivation' property has already been defined.");
                        }
                        onActivation = ParseEffectActivationList();
                        definedProperties.Add(TokenType.OnActivation);
                    }
                    else
                    {
                        ThrowSyntaxError($"Unexpected token '{LookAhead().Value}'");
                    }
                }

                Consume(TokenType.CloseBrace);

                return new Card(name, power, type, range.ToArray(), faction, onActivation, "");
            }
            catch (Error ex)
            {
                Debug.LogError(ex.ToString());
                throw;
            }

        }


        private bool ValidType(string type)
        {
            var validTypes = new HashSet<string> { "Oro", "Plata", "Lider", "Clima", "Aumento" };
            return validTypes.Contains(type);
        }



        private string ParseProperty(TokenType propertyType)
        {
            Consume(propertyType);
            Consume(TokenType.Colon);

            // Usar ConsumeAny para manejar diferentes tipos de valores
            Token valueToken = ConsumeAny(TokenType.String, TokenType.Identifier, TokenType.Number, TokenType.False, TokenType.True);
            if (valueToken.Type == TokenType.String || valueToken.Type == TokenType.Identifier || valueToken.Type == TokenType.Number || valueToken.Type == TokenType.False || valueToken.Type == TokenType.True)
            {
                return valueToken.Value;
            }
            else
            {
                ThrowSyntaxError($"Expected 'String', 'Identifier', or 'Number' for property '{propertyType}', but found '{valueToken.Type}'");
                return null;
            }
        }


        private List<string> ParseListProperty(TokenType propertyType)
        {
            List<string> items = new List<string>();

            Consume(propertyType);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBracket);

            while (!Match(TokenType.CloseBracket))
            {
                Token token = ConsumeAny(TokenType.String, TokenType.Number);
                items.Add(token.Value);
                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }

            Consume(TokenType.CloseBracket);
            return items;
        }


        private List<EffectActivation> ParseEffectActivationList()
        {
            List<EffectActivation> activations = new List<EffectActivation>();

            Consume(TokenType.OnActivation);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBracket);

            // Procesar el contenido dentro del corchete
            while (!Match(TokenType.CloseBracket))
            {
                if (Match(TokenType.OpenBrace))
                {
                    activations.Add(ParseEffectActivation());
                }
                else
                {
                    ThrowSyntaxError($"Se esperaba '{{' pero se encontró '{_lexerStream.CurrentToken.Value}'");
                }
            }

            Consume(TokenType.CloseBracket);
            return activations;
        }

        private EffectActivation ParseEffectActivation()
        {
            Consume(TokenType.OpenBrace);

            Effect effect = null;
            Selector selector = null;
            PostAction postAction = null;

            // Diccionario para controlar qué propiedades ya se han definido
            var definedProperties = new HashSet<TokenType>();

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {

                    case TokenType.effect:
                        if (definedProperties.Contains(TokenType.effect))
                        {
                            ThrowSyntaxError("The 'effect' has already been defined.");
                        }
                        effect = ParseCardEffect();
                        definedProperties.Add(TokenType.effect);
                        break;
                    case TokenType.Selector:
                        if (definedProperties.Contains(TokenType.Selector))
                        {
                            ThrowSyntaxError("The 'selector' has already been defined.");
                        }
                        selector = ParseSelector();
                        definedProperties.Add(TokenType.Selector);
                        break;
                    case TokenType.PostAction:
                        if (definedProperties.Contains(TokenType.PostAction))
                        {
                            ThrowSyntaxError("The 'postAction' has already been defined.");
                        }
                        postAction = ParsePostAction();
                        definedProperties.Add(TokenType.PostAction);
                        break;

                    case TokenType.OpenBrace:
                        if (definedProperties.Contains(TokenType.effect))
                        {
                            ThrowSyntaxError("The 'effect' has already been defined.");
                        }
                        effect = ParseSimpleEffect();
                        definedProperties.Add(TokenType.effect);
                        break;

                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in EffectActivation");
                        break;
                }

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }

            Consume(TokenType.CloseBrace);

            if (Match(TokenType.Comma))
            {
                Consume(TokenType.Comma);
            }
            // si hay selector efecto no puede ser null
            //si hay postaction efect no puede ser null

            return new EffectActivation(effect, selector, postAction);
        }

        public List<Effect> ParseEffects()
        {
            List<Effect> effects = new List<Effect>();

            Consume(TokenType.Effect);
            Consume(TokenType.OpenBrace);

            while (!Match(TokenType.CloseBrace))
            {
                try
                {
                    if (Match(TokenType.Name))
                    {
                        var effect = ParseEffect();
                        effects.Add(effect);

                        // Registrar el efecto en EffectRegistry
                        EffectRegistry.Instance.RegisterEffect(effect);
                    }
                    else
                    {
                        ThrowSyntaxError("Expected 'Effect' keyword", TokenType.Effect);
                    }
                }
                catch (Exception ex)
                {
                    ThrowSyntaxError($"Error parsing effect: {ex.Message}");
                }
            }

            Consume(TokenType.CloseBrace); // Consumir la llave de cierre
            return effects; // Retornar la lista de efectos parseados
        }

        public Effect ParseEffect()
        {
            string effectName = null;
            List<Parameter> parameters = new List<Parameter>();
            Action<List<Card>, Context> action = null;

            // Diccionario para controlar qué partes ya se han definido
            var definedProperties = new HashSet<TokenType>();

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Name:
                        if (definedProperties.Contains(TokenType.Name))
                        {
                            ThrowSyntaxError("The 'Name' has already been defined.");
                        }
                        effectName = ParseProperty(TokenType.Name);
                        definedProperties.Add(TokenType.Name);
                        Consume(TokenType.Comma);
                        break;

                    case TokenType.Params:
                        if (definedProperties.Contains(TokenType.Params))
                        {
                            ThrowSyntaxError("The 'Params' have already been defined.");
                        }
                        parameters = ParseParameters();
                        definedProperties.Add(TokenType.Params);
                        break;

                    case TokenType.Action:
                        if (definedProperties.Contains(TokenType.Action))
                        {
                            ThrowSyntaxError("The 'Action' has already been defined.");
                        }
                        action = ParseAction(parameters);
                        definedProperties.Add(TokenType.Action);
                        break;

                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in Effect");
                        break;
                }
            }

            if (effectName == null)
            {
                ThrowSyntaxError("Effect name is required", TokenType.Name);
            }

            return new Effect(effectName, parameters, action);
        }



        private List<Parameter> ParseParameters()
        {
            List<Parameter> parameters = new List<Parameter>();

            Consume(TokenType.Params);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBrace);

            while (!Match(TokenType.CloseBrace))
            {
                string paramName = Consume(TokenType.Identifier).Value;
                Consume(TokenType.Colon);

                ParamType paramType;
                object paramValue = null;  // Dejar el valor nulo hasta que se asigne posteriormente

                if (Match(TokenType.Number))
                {
                    paramType = ParamType.Number;
                    Consume(TokenType.Number);
                }
                else if (Match(TokenType.String))
                {
                    paramType = ParamType.String;
                    Consume(TokenType.String);

                }
                else if (Match(TokenType.Bool))
                {
                    paramType = ParamType.Bool;
                    Consume(TokenType.Bool);

                }
                else
                {
                    ThrowSyntaxError($"Tipo de parámetro inesperado después de '{paramName}'");
                    return null;
                }

                // Crear el parámetro con el tipo definido, sin un valor inicial
                parameters.Add(new Parameter(paramName, paramType, paramValue));

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }

            Consume(TokenType.CloseBrace);
            Consume(TokenType.Comma);
            return parameters;
        }


        private Action<List<Card>, Context> ParseAction(List<Parameter> parameters)
        {
            Consume(TokenType.Action);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenParen);

            // Leer y verificar los parámetros 'targets' y 'context'
            var targetsParam = Consume(TokenType.Identifier).Value;  // Parametro 'targets'
            Consume(TokenType.Comma);
            var contextParam = Consume(TokenType.Identifier).Value;
            Consume(TokenType.CloseParen);
            Consume(TokenType.Arrow);
            Consume(TokenType.OpenBrace);

            // Inicializar listas y diccionarios para almacenar acciones y variables locales
            List<Action<List<Card>, Context>> actions = new List<Action<List<Card>, Context>>();
            Dictionary<string, object> localVariables = new Dictionary<string, object>();

            // Guardar los parámetros 'targets' y 'context' como variables locales
            localVariables[targetsParam] = new List<Card>();  // Aseguramos que 'targets' sea una lista de cartas
            localVariables[contextParam] = null;  // El valor de 'context' se llenará en la ejecución

            while (!Match(TokenType.CloseBrace))
            {
                var A = LookAhead();

                if (A.Type == TokenType.Identifier || A.Type == TokenType.For || A.Type == TokenType.While)
                {
                    if (A.Type == TokenType.For)
                    {
                        ParseForLoop(actions, localVariables, parameters);  // Pasar 'localVariables' al bucle for
                    }
                    else if (A.Type == TokenType.While)
                    {
                        ParseWhileLoop(actions, localVariables, parameters);
                    }
                    else
                    {
                        ParseAssignment(localVariables, actions, parameters);  // Procesar las asignaciones
                    }
                }
                else
                {
                    ThrowSyntaxError($"Token inesperado '{_lexerStream.CurrentToken.Value}' en el bloque de acción.");
                }
            }

            Consume(TokenType.CloseBrace);

            // Retornar la acción final que ejecutará todas las acciones parseadas
            return (cards, ctx) =>
            {
                // Verificar que 'targets' es una lista de cartas
                if (!(localVariables[targetsParam] is List<Card>))
                {
                    throw new ArgumentException($"El parámetro '{targetsParam}' debe ser una lista de cartas.");
                }

                foreach (var action in actions)
                {
                    action(cards, ctx);  // Ejecutar cada acción con las cartas y el contexto proporcionados
                }
            };
        }

        private void ParseForLoop(List<Action<List<Card>, Context>> actions, Dictionary<string, object> localVariables, List<Parameter> parameters)
        {
            Consume(TokenType.For);
            var targetVar = Consume(TokenType.Identifier).Value;  // Variable para cada carta
            Consume(TokenType.In);
            var listName = Consume(TokenType.Identifier).Value;  // Nombre de la lista sobre la cual iterar

            // Verificar que 'listName' sea una lista de cartas (que corresponde a 'targets')
            if (!localVariables.ContainsKey(listName) || !(localVariables[listName] is List<Card>))
            {
                throw new ArgumentException($"La lista '{listName}' no está definida como una lista de cartas.");
            }

            Consume(TokenType.OpenBrace);

            List<Action<List<Card>, Context>> loopActions = new List<Action<List<Card>, Context>>();
             localVariables[targetVar] = null; 

            // Parsear todas las acciones dentro del bloque 'for'
            while (!Match(TokenType.CloseBrace))
            {
                ParseAssignment(localVariables, loopActions, parameters);  // Parsear las asignaciones y agregar a 'loopActions'
            }

            Consume(TokenType.CloseBrace);
            Consume(TokenType.SemiColon);

            // Añadir las acciones para ser ejecutadas en el contexto del bucle 'for'
            actions.Add((cards, ctx) =>
            {
                var cardList = (List<Card>)localVariables[listName];  // Obtener la lista de cartas

                foreach (var card in cardList)
                {
                    localVariables[targetVar] = card;  // Asignar cada carta a la variable local 'target'
                    foreach (var action in loopActions)
                    {
                        action(cards, ctx);  // Ejecutar las acciones del bloque 'for'
                    }
                }
            });
        }

        private void ParseWhileLoop(List<Action<List<Card>, Context>> actions, Dictionary<string, object> localVariables, List<Parameter> parameters)
        {
            Consume(TokenType.While);
            Consume(TokenType.OpenParen);

            var loopVar = Consume(TokenType.Identifier).Value;  // Variable de bucle (por ejemplo, 'i')
            Consume(TokenType.Increment);
            Consume(TokenType.Less);

            // Verificamos si 'conditionVar' es un número literal o una variable/parámetro
            string conditionVar;
            if (Match(TokenType.Number))
            {
                conditionVar = Consume(TokenType.Number).Value;  // Si es un número literal
            }
            else
            {
                conditionVar = Consume(TokenType.Identifier).Value;  // Si es un parámetro o variable
            }

            Consume(TokenType.CloseParen);

            List<Action<List<Card>, Context>> whileActions = new List<Action<List<Card>, Context>>();

            // Parsear todas las acciones dentro del bloque 'while'
            while (!Match(TokenType.CloseBrace))
            {
                ParseAssignment(localVariables, whileActions, parameters);
            }

            
            //Consume(TokenType.CloseBrace);

            // Añadir las acciones del bucle 'while'
            actions.Add((cards, ctx) =>
            {
                int loopValue = (int)localVariables[loopVar];  // Inicializar valor de la variable del bucle
                int conditionValue;

                // Si 'conditionVar' es un número literal
                if (int.TryParse(conditionVar, out conditionValue))
                {
                    // 'conditionVar' es un número literal, ya lo tenemos
                }
                // Si 'conditionVar' es un parámetro, usamos su valor
                else if (parameters.Any(p => p.paramName == conditionVar))
                {
                    var parameter = parameters.First(p => p.paramName == conditionVar);
                    conditionValue = (int)parameter.value;
                }
                // Si 'conditionVar' es una variable local
                else if (localVariables.ContainsKey(conditionVar))
                {
                    conditionValue = (int)localVariables[conditionVar];
                }
                else
                {
                    throw new ArgumentException($"'{conditionVar}' no está definido como un número, variable local o parámetro.");
                }

                // Ejecutar el bucle 'while'
                while (loopValue < conditionValue)
                {
                    foreach (var action in whileActions)
                    {
                        action(cards, ctx);  // Ejecutar las acciones del bucle
                    }
                    loopValue++;
                    localVariables[loopVar] = loopValue;  // Actualizamos el valor en 'localVariables'
                }
            });
        }

        private void ParseAssignment(Dictionary<string, object> localVariables, List<Action<List<Card>, Context>> actions, List<Parameter> parameters)
        {
            if (Match(TokenType.Identifier))
            {
                var identifier = Consume(TokenType.Identifier).Value;


                // Manejo de 'context'
                if (identifier == "context")
                {
                    identifier = ContextAssignment(actions, localVariables, identifier);  // Pasamos el 'identifier' a ContextAssignment
                }
                // Manejo de 'target'
                else if (identifier == "target")
                {
                    identifier = TargetAssignment(actions, localVariables, identifier, parameters);  // Pasamos el 'identifier' a TargetAssignment
                }

                // Verificar si viene una asignación '='
                if (Match(TokenType.Equals))
                {

                    if (localVariables.ContainsKey(identifier))
                    {
                        throw new ArgumentException($"La variable local '{identifier}' ya está definida.");
                    }
                    Consume(TokenType.Equals);
                    var nextToken = LookAhead().Value;

                    // Asignación de 'context' a una variable local
                    if (nextToken == "context")
                    {
                        Consume(TokenType.Identifier);  // Consumimos 'context'
                        var result = ContextAssignment(actions, localVariables, identifier);  // Asignamos el resultado de 'ContextAssignment'
                        localVariables[identifier] = result;  // Guardar el resultado en las variables locales
                    }
                    // Asignación de 'target' a una variable local
                    else if (nextToken == "target")
                    {
                        Consume(TokenType.Identifier);  // Consumimos 'target'
                        var result = TargetAssignment(actions, localVariables, identifier, parameters);  // Asignamos el resultado de 'TargetAssignment'
                        localVariables[identifier] = result;  // Guardar el resultado en las variables locales
                    }
                    // Asignación de un número a la variable local
                    else if (Match(TokenType.Number))
                    {
                        var numberValue = Consume(TokenType.Number).Value;
                        int parsedValue = int.Parse(numberValue);

                        actions.Add((cards, ctx) =>
                        {
                            localVariables[identifier] = parsedValue;
                        });
                        Consume(TokenType.SemiColon);
                    }

                    else
                    {
                        throw new ArgumentException($"'{nextToken}' no está definido como variable local.");
                    }
                }
                else if (Match(TokenType.Dot)) // Métodos encadenados como 'Pop', 'Push'
                {
                    MethodChain(identifier, localVariables, actions);  // Manejar métodos encadenados
                }
            }
            else if (Match(TokenType.While))
            {
                ParseWhileLoop(actions, localVariables, parameters);  // Manejo del bucle 'while'
            }
            else if (Match(TokenType.For))
            {
                ParseForLoop(actions, localVariables, parameters);  // Manejo del bucle 'for'
            }
            else
            {
                throw new ArgumentException("Error en la asignación");
            }
        }

        private string ContextAssignment(List<Action<List<Card>, Context>> actions, Dictionary<string, object> localVariables, string identifier)
        {
            Consume(TokenType.Dot);
            var propertyOrMethod = ConsumePropertyOrMethod();

            switch (propertyOrMethod.Type)
            {
                case TokenType.DeckOfPlayer:
                case TokenType.HandOfPlayer:
                case TokenType.FieldOfPlayer:
                case TokenType.GraveyardOfPlayer:
                    Consume(TokenType.OpenParen);
                    var owner = Consume(TokenType.Identifier).Value;
                    Consume(TokenType.CloseParen);

                    // Verificar si el 'owner' ha sido previamente asignado desde 'target.Owner'
                    if (!localVariables.ContainsKey(owner))
                    {
                        throw new ArgumentException($"El identificador '{owner}' debe haber sido asignado desde 'target.Owner'.");
                    }

                    actions.Add((cards, ctx) =>
                    {
                        // Asignar el resultado del método 'DeckOfPlayer', 'HandOfPlayer', etc., a la variable local 'identifier'
                        var result = ctx.GetType().GetMethod(propertyOrMethod.Value)?.Invoke(ctx, new object[] { localVariables[owner] });
                        localVariables[identifier] = result;  // Asignar el resultado a la variable local identificada (e.g., deck)
                    });

                    Consume(TokenType.SemiColon);  // Consumir el ';' al final de la asignación
                    return identifier;

                // Manejo de otros métodos y propiedades como Deck, Hand, Field, etc.
                case TokenType.Deck:
                case TokenType.Hand:
                case TokenType.Field:
                case TokenType.Graveyard:
                case TokenType.Board:
                    if (Match(TokenType.Dot))
                    {
                        Consume(TokenType.Dot);
                        var method = ConsumeMethod();

                        switch (method.Type)
                        {
                            case TokenType.Pop:
                            case TokenType.Shuffle:
                                Consume(TokenType.OpenParen);
                                Consume(TokenType.CloseParen);

                                actions.Add((cards, ctx) =>
                                {
                                    ctx.GetType().GetMethod(method.Value)?.Invoke(ctx, null);
                                });
                                break;

                            case TokenType.Push:
                            case TokenType.Add:
                            case TokenType.Remove:
                            case TokenType.SemiColon:
                                Consume(TokenType.OpenParen);
                                var param = Consume(TokenType.Identifier).Value;
                                if (!localVariables.ContainsKey(param))
                                    throw new ArgumentException($"'{param}' no está definido.");
                                Consume(TokenType.CloseParen);

                                actions.Add((cards, ctx) =>
                                {
                                    ctx.GetType().GetMethod(method.Value)?.Invoke(ctx, new object[] { localVariables[param] });
                                });
                                break;
                        }
                    }
                    Consume(TokenType.SemiColon);  // Consumir el ';' al final de la asignación
                    break;

                default:
                    ThrowSyntaxError($"Propiedad inesperada '{propertyOrMethod.Value}' en context.");
                    break;
            }

            return propertyOrMethod.Value;
        }

        private string TargetAssignment(List<Action<List<Card>, Context>> actions, Dictionary<string, object> localVariables, string identifier, List<Parameter> parameters)
        {
            Consume(TokenType.Dot);
            var targetProperty = ConsumeTargetProperty();

            switch (targetProperty.Type)
            {
                case TokenType.Name:
                case TokenType.Faction:
                case TokenType.Owner:
                    // Asignación directa de una propiedad de 'target' a una variable local
                    actions.Add((cards, ctx) =>
                    {
                        foreach (var card in cards)
                        {
                            localVariables[identifier] = targetProperty.Type switch
                            {
                                TokenType.Name => card.Name,
                                TokenType.Faction => card.Faction,
                                TokenType.Owner => card.Owner,
                                _ => throw new ArgumentException($"Propiedad inesperada '{targetProperty.Value}' en target.")
                            };
                        }
                    });
                    Consume(TokenType.SemiColon);  // Consumir el ';' al final de la asignación
                    return targetProperty.Value;

                case TokenType.Power:
                    // Manejo de incremento o decremento (+=, -=) en target.Power
                    if (Match(TokenType.Plus) || Match(TokenType.Minus))
                    {
                        var operation = LookAhead().Type == TokenType.Plus ? 1 : -1;
                        Consume(LookAhead().Type); // Consume '+' o '-'
                        Consume(TokenType.Equals);

                        // Verificar si es un número o un parámetro/variable
                        int value = 0;

                        // Si es un número directamente
                        if (Match(TokenType.Number))
                        {
                            value = int.Parse(Consume(TokenType.Number).Value);
                        }
                        else
                        {
                            var paramOrVariable = Consume(TokenType.Identifier).Value;

                            // Verificar si está en localVariables o en los parámetros
                            if (localVariables.ContainsKey(paramOrVariable))
                            {
                                value = Convert.ToInt32(localVariables[paramOrVariable]);  // Convertir el valor a entero
                            }
                            else
                            {
                                // Buscar en los parámetros
                                var parameter = parameters.FirstOrDefault(p => p.paramName == paramOrVariable);
                                if (parameter != null)
                                {
                                    // Convertir el valor del parámetro dependiendo de su tipo
                                    if (parameter.type == ParamType.Number)
                                    {
                                        value = (int)(parameter.value ?? 0); // Tomar el valor del parámetro si está definido
                                    }
                                    else
                                    {
                                        throw new ArgumentException($"El parámetro '{paramOrVariable}' no es del tipo esperado 'Number'.");
                                    }
                                }
                                else
                                {
                                    throw new ArgumentException($"'{paramOrVariable}' no está definido como una variable o parámetro.");
                                }
                            }
                        }

                        // Agregar la acción que incrementa o decrementa el poder
                        actions.Add((cards, ctx) =>
                        {
                            foreach (var card in cards)
                            {
                                card.Power += operation * value;
                            }
                        });

                        Consume(TokenType.SemiColon);  // Consumir el ';' al final de la asignación
                        return "Power";
                    }
                    else
                    {
                        throw new ArgumentException($"Operación inesperada para target.Power.");
                    }

                default:
                    ThrowSyntaxError($"Propiedad inesperada '{targetProperty.Value}' en target.");
                    return null;
            }
        }
        private void MethodChain(string identifier, Dictionary<string, object> localVariables, List<Action<List<Card>, Context>> actions)
        {
            // Si el identificador no está en localVariables, lanzar un error
            if (identifier == null || !localVariables.ContainsKey(identifier))
            {
                throw new ArgumentException($"La variable '{identifier}' no está definida.");
            }

            // Consumir el punto y el método siguiente
            Consume(TokenType.Dot);
            var method = ConsumeMethod();

            // Verificar si el método es de un objeto que no requiere parámetros, como 'Pop' o 'Shuffle'
            switch (method.Type)
            {
                case TokenType.Pop:
                case TokenType.Shuffle:
                    Consume(TokenType.OpenParen);
                    Consume(TokenType.CloseParen);

                    // Añadir la acción para métodos sin parámetros
                    actions.Add((cards, ctx) =>
                    {
                        var target = localVariables[identifier];
                        // Invocar el método 'Pop' o 'Shuffle' en el objeto (por ejemplo, 'deck')
                        target.GetType().GetMethod(method.Value)?.Invoke(target, null);
                    });
                    break;

                // Métodos que requieren un parámetro como 'Push', 'Add', 'SendBottom', 'Remove', etc.
                case TokenType.Push:
                case TokenType.Add:
                case TokenType.SendBottom:
                case TokenType.Remove:
                case TokenType.Find:
                    Consume(TokenType.OpenParen);
                    var param = Consume(TokenType.Identifier).Value;
                    Consume(TokenType.CloseParen);

                    // Validamos que el parámetro esté definido previamente en las variables locales
                    if (!localVariables.ContainsKey(param))
                    {
                        throw new ArgumentException($"El parámetro '{param}' no está definido.");
                    }

                    // Validar que el parámetro sea del tipo esperado (por ejemplo, tipo 'Card')
                   // if (!(localVariables[param] is Card))
                   // {
                   //     throw new ArgumentException($"El parámetro '{param}' debe ser de tipo 'Card'.");
                   // }

                    // Añadir la acción para métodos con parámetros
                    actions.Add((cards, ctx) =>
                    {
                        var target = localVariables[identifier];
                        var card = (Card)localVariables[param];

                        // Invocar el método, como 'deck.Push(target)'
                        target.GetType().GetMethod(method.Value)?.Invoke(target, new object[] { card });
                    });
                    break;

                default:
                    ThrowSyntaxError($"Método inesperado '{method.Value}' encontrado.");
                    break;
            }
            Consume(TokenType.SemiColon);
        }



        private Token ConsumePropertyOrMethod()
        {
            // Consumo un token que es una propiedad o método específico de context
            var nextToken = LookAhead();
            if (PropertyContext(nextToken.Type))
            {
                return Consume(nextToken.Type);
            }
            else if (Match(TokenType.Identifier))
            {
                return Consume(TokenType.Identifier);
            }
            else
            {
                ThrowSyntaxError($"Unexpected token '{nextToken.Value}'. Expected a context property or method.");
                return null;
            }
        }

        private Token ConsumeMethod()
        {

            var nextToken = LookAhead();
            if (MethodContext(nextToken.Type))
            {
                return Consume(nextToken.Type);
            }
            else
            {
                ThrowSyntaxError($"Unexpected token '{nextToken.Value}'. Expected a method after context property.");
                return null; // Nunca se alcanza, solo para cumplir con el flujo de control
            }
        }

        private Token ConsumeTargetProperty()
        {

            var nextToken = LookAhead();
            if (PropertyCard(nextToken.Type))
            {
                return Consume(nextToken.Type);
            }
            else
            {
                ThrowSyntaxError($"Unexpected token '{nextToken.Value}'. Expected a target property.");
                return null;
            }
        }

        private bool PropertyContext(TokenType type)
        {
            // Verifica si el tipo de token es una propiedad o método de context
            return type == TokenType.DeckOfPlayer ||
                   type == TokenType.HandOfPlayer ||
                   type == TokenType.FieldOfPlayer ||
                   type == TokenType.GraveyardOfPlayer ||
                   type == TokenType.Deck ||
                   type == TokenType.Hand ||
                   type == TokenType.Field ||
                   type == TokenType.Graveyard ||
                   type == TokenType.Board ||
                   type == TokenType.Push;


        }

        private bool MethodContext(TokenType type)
        {
            // Verifica si el tipo de token es un método de context
            return type == TokenType.Pop ||
                   type == TokenType.Shuffle ||
                   type == TokenType.Push ||
                   type == TokenType.SendBottom ||
                   type == TokenType.Remove ||
                   type == TokenType.Find ||
                   type == TokenType.Add;
        }

        private bool PropertyCard(TokenType type)
        {
            // Verifica si el tipo de token es una propiedad de target
            return type == TokenType.Name ||
                   type == TokenType.Power ||
                   type == TokenType.Type ||
                   type == TokenType.Range ||
                   type == TokenType.Faction ||
                   type == TokenType.Owner;
        }



        private Selector ParseSelector()
        {
            Consume(TokenType.Selector);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBrace);

            string source = null;
            bool single = false;
            string predicate = null;

            bool hasSource = false;
            bool hasSingle = false;
            bool hasPredicate = false;

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Source:
                        if (hasSource) ThrowSyntaxError("Duplicate 'Source' property in Selector");

                        source = ParseProperty(TokenType.Source);

                        // Validar el valor de Source
                        if (!ValidSource(source))
                        {
                            ThrowSyntaxError($"Invalid source '{source}'. Expected one of: board, field, otherField, hand, otherHand, deck, otherDeck.");
                        }

                        hasSource = true;
                        break;

                    case TokenType.Single:
                        if (hasSingle) ThrowSyntaxError("Duplicate 'Single' property in Selector");

                        string singleValue = ParseProperty(TokenType.Single);
                        if (bool.TryParse(singleValue, out bool parsedSingle))
                        {
                            single = parsedSingle;
                        }
                        else
                        {
                            ThrowSyntaxError($"Expected 'Bool' for property 'Single', but found '{singleValue}'");
                        }

                        hasSingle = true;
                        break;

                    case TokenType.Predicate:
                        if (hasPredicate) ThrowSyntaxError("Duplicate 'Predicate' property in Selector");

                        predicate = ParsePredicate();
                        hasPredicate = true;
                        break;

                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in Selector");
                        break;
                }

                if (!Match(TokenType.CloseBrace) && !Match(TokenType.Comma))
                {
                    ThrowSyntaxError($"Expected a comma or closing brace in Selector, but found '{_lexerStream.CurrentToken.Value}'");
                }

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }

            if (Match(TokenType.CloseBrace))
            {
                Consume(TokenType.CloseBrace);
            }

            return new Selector(source, single, predicate);
        }

        private bool ValidSource(string source)
        {
            var validSources = new HashSet<string> { "board", "field", "otherField", "hand", "otherHand", "deck", "otherDeck", "parent" };
            return validSources.Contains(source);
        }


        private PostAction ParsePostAction()
        {
            Consume(TokenType.PostAction);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBrace);

            string type = null;
            Selector selector = null;
            // por arreglar
            int? amount = null;

            bool hasType = false;
            bool hasSelector = false;

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Type:
                        if (hasType) ThrowSyntaxError("Duplicate 'Type' property in PostAction");
                        type = ParseProperty(TokenType.Type);
                        hasType = true;
                        break;
                    case TokenType.Identifier://.Amount:
                        string amountValue = ParseProperty(TokenType.Identifier);//Amount
                        if (int.TryParse(amountValue, out int parsedAmount))
                        {
                            amount = parsedAmount;
                        }
                        else
                        {
                            ThrowSyntaxError($"Expected 'Number' for property 'Amount', but found '{amountValue}'");
                        }
                        break;
                    case TokenType.Selector:
                        if (hasSelector) ThrowSyntaxError("Duplicate 'Selector' property in PostAction");
                        selector = ParseSelector();
                        hasSelector = true;
                        break;
                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in PostAction");
                        break;
                }


                if (!Match(TokenType.CloseBrace) && !Match(TokenType.Comma))
                {
                    ThrowSyntaxError($"Expected a comma or closing brace in PostAction, but found '{_lexerStream.CurrentToken.Value}'");
                }

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }
            Consume(TokenType.CloseBrace);

            if (Match(TokenType.Comma))
            {
                Consume(TokenType.Comma);
            }

            return new PostAction(type, selector);
        }


        private string ParsePredicate() //Debe mejorarse
        {

            Consume(TokenType.Predicate);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenParen);

            var predicate = new StringBuilder();

            // Consumimos el token Unit (ej. 'unit')
            var paramToken = Consume(TokenType.Unit); // Esto debería ser 'unit' o cualquier otro parámetro específico
            predicate.Append(paramToken.Value);

            Consume(TokenType.CloseParen);

            if (Match(TokenType.Arrow))
            {
                Consume(TokenType.Arrow);
                predicate.Append(" => ");

                // Consumimos el cuerpo de la expresión lambda
                while (!Match(TokenType.CloseBrace))
                {
                    var token = Consume(LookAhead().Type);

                    // Maneja identificadores específicos y operadores
                    if (token.Type == TokenType.Unit || token.Type == TokenType.Identifier ||
                        token.Type == TokenType.String || token.Type == TokenType.Number ||
                        token.Type == TokenType.Faction || token.Type == TokenType.Power || token.Type == TokenType.False || token.Type == TokenType.True) // <-- Aseguramos que Faction es válido
                    {
                        predicate.Append(token.Value);
                    }
                    else if (IsOperator(token.Type))
                    {
                        predicate.Append($" {token.Value} ");
                    }
                    else if (token.Type == TokenType.EqualsEquals)
                    {
                        predicate.Append(" == ");
                    }
                    else if (token.Type == TokenType.And)
                    {
                        predicate.Append(" && ");
                    }
                    else if (token.Type == TokenType.Or)
                    {
                        predicate.Append(" || ");
                    }
                    else
                    {
                        throw new Error($"Unexpected token '{token.Value}' at position {token.Pos}.");
                    }
                }
            }
            else
            {
                throw new Error("Expected '=>' after the parameter list in the predicate.");
            }

            return predicate.ToString().Trim(); // Retorna el predicado como una cadena de texto
        }


        private bool IsOperator(TokenType type)
        {
            return type == TokenType.EqualsEquals || type == TokenType.NotEqual ||
                   type == TokenType.Less || type == TokenType.Greater ||
                   type == TokenType.LessOrEqual || type == TokenType.GreaterOrEqual ||
                   type == TokenType.And || type == TokenType.Or ||
                   type == TokenType.Plus || type == TokenType.Minus ||
                   type == TokenType.Multiply || type == TokenType.Slash ||
                   type == TokenType.Modulus || type == TokenType.Dot;
        }

        private Token Consume(TokenType type)
        {
            if (LookAhead().Type != type)
            {
                ThrowSyntaxError($"Se esperaba un token de tipo '{type}', pero se encontró '{LookAhead().Type}'");
            }
            Token token = _lexerStream.CurrentToken;
            _lexerStream.Advance();
            //LogTokenConsumed(token);
            return token;
        }

        public bool Match(TokenType type)
        {
            return LookAhead().Type == type;
        }

        private Token LookAhead()
        {
            return _lexerStream.CurrentToken;
        }

        private void LogCurrentToken(string context)
        {
            Debug.Log($"Current Token in {context}: {_lexerStream.CurrentToken}");
        }

        //  private void LogTokenConsumed(Token token)
        //  {
        //      Debug.Log($"Consumed Token: {token}");
        //  }

        private void ThrowSyntaxError(string message, TokenType expectedType = TokenType.None)
        {
            string errorMessage = $" {message}. Se encontró '{_lexerStream.CurrentToken.Value}' en la posición {_lexerStream.CurrentToken.Pos}.";
            Debug.LogError(errorMessage);
            throw new Error(errorMessage);
        }


        private int ParseIntegerProperty(TokenType propertyType)
        {
            Consume(propertyType);
            Consume(TokenType.Colon);

            // Usar ConsumeAny para manejar números
            Token valueToken = ConsumeAny(TokenType.Number);
            if (valueToken.Type == TokenType.Number)
            {
                if (!int.TryParse(valueToken.Value, out int result))
                {
                    ThrowSyntaxError($"Invalid integer value '{valueToken.Value}' for property '{propertyType}'.");
                }
                return result;
            }
            else
            {
                ThrowSyntaxError($"Expected 'Number' for property '{propertyType}', but found '{valueToken.Type}'");
                return 0; // Este retorno nunca se alcanzará, es solo para evitar errores de compilación
            }
        }

        private Token ConsumeAny(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Match(type))
                {
                    Token token = _lexerStream.CurrentToken;
                    _lexerStream.Advance();
                    // LogTokenConsumed(token);
                    return token;
                }
            }

            ThrowSyntaxError($"Expected one of the following types: {string.Join(", ", types)} but found '{LookAhead().Type}'");
            return null;
        }
        private Effect ParseCardEffect()
        {
            Consume(TokenType.effect);
            Consume(TokenType.Colon);

            if (Match(TokenType.OpenBrace))
            {
                Consume(TokenType.OpenBrace);

                string effectName = null;
                int? amount = null;
                List<Parameter> parameters = new List<Parameter>();
                Action<List<Card>, Context> action = null;

                while (!Match(TokenType.CloseBrace))
                {
                    switch (_lexerStream.CurrentToken.Type)
                    {
                        case TokenType.Name:
                            if (effectName != null)
                            {
                                ThrowSyntaxError("El nombre del efecto ('Name') ya ha sido definido.");
                            }
                            effectName = ParseProperty(TokenType.Name);
                            Consume(TokenType.Comma);
                            break;

                        case TokenType.Identifier: // .Amount
                            if (amount != null)
                            {
                                ThrowSyntaxError("El valor de 'Amount' ya ha sido definido.");
                            }
                            string amountValue = ParseProperty(TokenType.Identifier); // Amount
                            Consume(TokenType.Comma);

                            if (int.TryParse(amountValue, out int parsedAmount))
                            {
                                amount = parsedAmount;
                            }
                            else
                            {
                                ThrowSyntaxError($"Se esperaba un número para la propiedad 'Amount', pero se encontró '{amountValue}'.");
                            }
                            break;

                        default:
                            ThrowSyntaxError($"Token inesperado '{_lexerStream.CurrentToken.Value}' dentro del efecto de la carta.");
                            break;
                    }
                }

                if (Match(TokenType.CloseBrace))
                {
                    Consume(TokenType.CloseBrace);
                }

                return new Effect(effectName, parameters, action);
            }
            else
            {
                return ParseSimpleEffect();
            }
        }

        private Effect ParseSimpleEffect()
        {

            if (Match(TokenType.OpenBrace))
            {
                Consume(TokenType.OpenBrace);
            }

            if (Match(TokenType.Effect))
            {
                string effectName = ParseProperty(TokenType.Effect);
                Consume(TokenType.Comma);

                if (Match(TokenType.CloseBrace))
                {
                    return new Effect(effectName, new List<Parameter>(), null); // action es null porque no hay selector ni postAction
                }
                else
                {
                    ThrowSyntaxError("Expected '}' after simple effect name.");
                }
            }
            else if (Match(TokenType.String))
            {
                Token valueToken = ConsumeAny(TokenType.String);
                string effectName = valueToken.Value;
                if (valueToken.Type == TokenType.String)
                {
                    effectName = valueToken.Value;
                    if (Match(TokenType.Comma))
                    {
                        Consume(TokenType.Comma);
                    }
                    if (!Match(TokenType.Identifier))//.Amount))
                    {
                        return new Effect(effectName, new List<Parameter>(), null);
                    }
                    else if (Match(TokenType.Identifier))//.Amount))
                    {

                        int? amount = null;
                        List<Parameter> parameters = new List<Parameter>();
                        Action<List<Card>, Context> action = null;
                        string amountValue = ParseProperty(TokenType.Identifier);//.Amount))
                        Consume(TokenType.Comma);
                        if (int.TryParse(amountValue, out int parsedAmount))
                        {
                            amount = parsedAmount;
                        }
                        else
                        {
                            ThrowSyntaxError($"Expected 'Number' for property 'Amount', but found '{amountValue}'");
                        }
                        return new Effect(effectName, parameters, action);

                    }
                }
                else
                {
                    ThrowSyntaxError($"Expected 'Value' for property '{TokenType.String}', but found '{valueToken.Type}'");
                    return null;
                }
            }

            return null; // Retorno por defecto si hay un error
        }

    }
}
