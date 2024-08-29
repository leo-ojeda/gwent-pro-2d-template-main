using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DSL.Lexer;

namespace DSL.Parser
{

    public class Parser
    {
        private readonly LexerStream _lexerStream;

        public Parser(LexerStream lexerStream)
        {
            _lexerStream = lexerStream;
        }

        public List<Card> ParseCards()
        {
            List<Card> cards = new List<Card>();
            while (!IsEndOfInput())
            {
                try
                {
                    var card = ParseCard();
                    cards.Add(card);
                }
                catch (LexerError)
                {
                    SkipToNextStatement();
                }
                catch (Exception)
                {
                    SkipToNextStatement();
                }
            }
            return cards;
        }
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

                while (!Match(TokenType.CloseBrace))
                {
                    if (Match(TokenType.Type))
                    {
                        type = ParseProperty(TokenType.Type);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Name))
                    {
                        name = ParseProperty(TokenType.Name);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Faction))
                    {
                        faction = ParseProperty(TokenType.Faction);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Power))
                    {
                        power = ParseIntegerProperty(TokenType.Power);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Range))
                    {
                        range = ParseListProperty(TokenType.Range);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.OnActivation))
                    {
                        onActivation = ParseEffectActivationList();
                    }
                    else
                    {
                        ThrowSyntaxError($"Unexpected token '{LookAhead().Value}'");
                    }
                }

                Consume(TokenType.CloseBrace);

                return new Card(name, power, type, range.ToArray(), faction, onActivation, "");
            }
            catch (LexerError ex)
            {
                Debug.LogError(ex.ToString());
                throw;
            }
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
                return null; // Este retorno nunca se alcanzará, es solo para evitar errores de compilación
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

            // Consume 'OnActivation' y ':'
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

                // Si encontramos un CloseBracket aquí, terminamos el bucle
                if (Match(TokenType.CloseBracket))
                {
                    break; // Termina el bucle si se encuentra CloseBracket
                }
            }

            // Consume el corchete de cierre
            Consume(TokenType.CloseBracket);
            return activations;
        }

        private EffectActivation ParseEffectActivation()
        {
            // Consume el token de apertura del bloque de activación (si no se ha consumido en ParseCardEffect)
            Consume(TokenType.OpenBrace);

            Effect effect = null;
            Selector selector = null;
            PostAction postAction = null;

            while (!Match(TokenType.CloseBrace))
            {
                // Verificar si se encuentra un CloseBracket prematuramente y terminar el bucle
                if (Match(TokenType.CloseBracket))
                {
                    // Terminar el bucle y retornar el estado actual
                    return new EffectActivation(effect, selector, postAction);
                }

                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Effect:
                        effect = ParseCardEffect(); // Usa el nuevo método para analizar efectos dentro de una carta
                        break;
                    case TokenType.Selector:
                        selector = ParseSelector();
                        break;
                    case TokenType.PostAction:
                        postAction = ParsePostAction();
                        break;
                    case TokenType.OpenBrace:
                        effect = ParseSimpleEffect();
                        break;
                    default:
                        ThrowSyntaxError($"Token inesperado el pepe '{_lexerStream.CurrentToken.Value}' en EffectActivation");
                        break;
                }

                //_lexerStream.Advance(); // Avanza al siguiente token después de procesar uno.
                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }
            if (Match(TokenType.CloseBrace))
            {
                Consume(TokenType.CloseBrace);
                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }

            }

            // Retorna el objeto EffectActivation con los valores finales
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
                    }
                    else
                    {
                        ThrowSyntaxError("Expected 'Effect' keyword", TokenType.Effect);
                    }
                }
                catch (Exception ex)
                {
                    // Captura cualquier excepción en el parsing de efectos
                    ThrowSyntaxError($"Error parsing effect: {ex.Message}");
                }
            }

            Consume(TokenType.CloseBrace);
            return effects;
        }
        public Effect ParseEffect()
        {
            //Consume(TokenType.Effect);
            //Consume(TokenType.OpenBrace);

            string effectName = null;
            List<Parameter> parameters = new List<Parameter>();
            Action<List<Card>, Context> action = null;

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Name:
                        effectName = ParseProperty(TokenType.Name);
                        Consume(TokenType.Comma);
                        break;
                    case TokenType.Params:
                        parameters = ParseParameters();
                        break;
                    case TokenType.Action:
                        action = ParseAction();
                        break;
                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in Effect");
                        break;
                }
            }

            //Consume(TokenType.CloseBrace);

            if (effectName == null)
            {
                ThrowSyntaxError("Effect name is required", TokenType.Name);
            }

            return new Effect(effectName, parameters, action);
        }




        private List<Parameter> ParseParameters()
        {
            List<Parameter> parameters = new List<Parameter>();

            Consume(TokenType.Params); // Consume "Params"
            Consume(TokenType.Colon);  // Consume ":"
            Consume(TokenType.OpenBrace); // Consume "{"

            while (!Match(TokenType.CloseBrace))
            {


                string paramName = Consume(TokenType.Amount).Value;
                Consume(TokenType.Colon);

                ParamType paramType;
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
                    return null; // Para evitar más errores en tiempo de compilación, aunque normalmente ThrowSyntaxError lanzará una excepción
                }

                // Crear el parámetro y añadirlo a la lista
                parameters.Add(new Parameter(paramType));

                // Si hay una coma, consumirla y continuar al siguiente parámetro
            }
            if (Match(TokenType.CloseBrace))
            {
                Consume(TokenType.CloseBrace); // Consume "}"
                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }

            return parameters;
        }

        private object ParseParameterValue(ParamType type)
        {
            switch (type)
            {
                case ParamType.Number:
                    return int.Parse(Consume(TokenType.Number).Value);
                case ParamType.String:
                    return Consume(TokenType.String).Value;
                case ParamType.Bool:
                    return bool.Parse(Consume(TokenType.Bool).Value);
                default:
                    throw new Exception("Tipo de parámetro desconocido.");
            }
        }


        private Action<List<Card>, Context> ParseAction()
        {
            Consume(TokenType.Action);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenParen); // "("
            var targetsParam = Consume(TokenType.Identifier).Value; // "targets"
            Consume(TokenType.Comma);
            var contextParam = Consume(TokenType.Identifier).Value; // "context"
            Consume(TokenType.CloseParen); // ")"
            Consume(TokenType.Arrow); // "=>"
            Consume(TokenType.OpenBrace); // "{"

            List<Action<List<Card>, Context>> actions = new List<Action<List<Card>, Context>>();
            Dictionary<string, object> localVariables = new Dictionary<string, object>();

            while (!Match(TokenType.CloseBrace))
            {
                if (Match(TokenType.For))
                {
                    Consume(TokenType.For);
                    var targetVar = Consume(TokenType.Identifier).Value; // e.g., "target"
                    Consume(TokenType.In);
                    var listName = Consume(TokenType.Identifier).Value; // e.g., "targets"
                    Consume(TokenType.OpenBrace); // "{"

                    List<Action<Card, Context>> loopActions = new List<Action<Card, Context>>();

                    while (!Match(TokenType.CloseBrace))
                    {
                        if (Match(TokenType.Identifier))
                        {
                            var variable = Consume(TokenType.Identifier).Value; // e.g., "i" or "owner"
                            if (Match(TokenType.Equals))
                            {
                                Consume(TokenType.Equals);
                                var value = int.Parse(Consume(TokenType.Number).Value); // e.g., "0"
                                Consume(TokenType.SemiColon); // ";"
                                localVariables[variable] = value; // Store variable
                            }
                            else if (Match(TokenType.Dot))
                            {
                                Consume(TokenType.Dot); // "."
                                var propertyOrMethod = Consume(TokenType.Identifier).Value; // e.g., "Power" or "Shuffle"

                                if (Match(TokenType.Minus))
                                {
                                    Consume(TokenType.Minus); // "-"
                                    Consume(TokenType.Equals); // "="
                                    var value = int.Parse(Consume(TokenType.Number).Value); // e.g., "1"
                                    Consume(TokenType.SemiColon); // ";"
                                    loopActions.Add((targetCard, ctx) => targetCard.Power -= value);
                                }
                            }
                            else if (Match(TokenType.Increment))
                            {
                                Consume(TokenType.Increment); // "++"
                                Consume(TokenType.SemiColon); // ";"
                                                              // loopActions.Add((targetCard, ctx) => localVariables[variable]++);
                            }
                        }
                        else if (Match(TokenType.While))
                        {
                            Consume(TokenType.While);
                            Consume(TokenType.OpenParen); // "("
                            var loopVar = Consume(TokenType.Identifier).Value; // e.g., "i"
                            Consume(TokenType.Increment); // "++"
                            Consume(TokenType.Less); // "<"
                            var loopConditionValue = Consume(TokenType.Amount).Value; // e.g., "Amount"
                            Consume(TokenType.CloseParen); // ")"

                            List<Action<Card, Context>> whileActions = new List<Action<Card, Context>>();

                            if (Match(TokenType.Identifier))
                            {
                                var target = Consume(TokenType.Identifier).Value; // e.g., "target"
                                if (Match(TokenType.Dot))
                                {
                                    Consume(TokenType.Dot); // "."
                                    var property = Consume(TokenType.Power).Value; // "Power"
                                    if (Match(TokenType.Minus))
                                    {
                                        Consume(TokenType.Minus); // "-"
                                        Consume(TokenType.Equals); // "="
                                        var value = int.Parse(Consume(TokenType.Number).Value); // e.g., "1"
                                        Consume(TokenType.SemiColon); // ";"
                                        whileActions.Add((targetCard, ctx) => targetCard.Power -= value);
                                    }
                                }
                            }

                            actions.Add((targets, ctx) =>
                            {
                                foreach (var target in targets)
                                {
                                    int currentValue = 0; // Inicializa la variable de control

                                    // Usa el valor de 'loopConditionValue' directamente
                                    while (currentValue < 1)
                                    {
                                        currentValue++; // Incrementa después de la comparación

                                        // Ejecuta las acciones dentro del bucle 'while'
                                        foreach (var action in whileActions)
                                        {
                                            action(target, ctx);
                                        }
                                    }
                                }
                            });
                        }
                        else if (Match(TokenType.Identifier))
                        {
                            // Handle general assignments like 'owner = target.Owner'
                            var variable = Consume(TokenType.Identifier).Value; // e.g., "owner"
                            if (Match(TokenType.Equals))
                            {
                                Consume(TokenType.Equals); // "="
                                var valueExpression = Consume(TokenType.Identifier).Value; // e.g., "target.Owner"
                                Consume(TokenType.SemiColon); // ";"

                                // Add the assignment action
                                loopActions.Add((targetCard, ctx) =>
                                {
                                    // Example: ctx.GetValue(variable, valueExpression);
                                    // Implement logic to handle assignment if necessary
                                });
                            }
                            else
                            {
                                ThrowSyntaxError($"Unexpected token in for loop after '{variable}'");
                            }
                        }
                        else
                        {
                            ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in Action block");
                        }
                    }

                    Consume(TokenType.CloseBrace); // End of for loop
                    if (Match(TokenType.SemiColon))
                    {
                        Consume(TokenType.SemiColon);
                    }

                    actions.Add((cards, ctx) =>
                    {
                        foreach (var card in cards)
                        {
                            foreach (var loopAction in loopActions)
                            {
                                loopAction(card, ctx);
                            }
                        }
                    });
                }
                else if (Match(TokenType.Identifier))
                {
                    var contextObject = Consume(TokenType.Identifier).Value; // e.g., "context" o una variable local

                    if (Match(TokenType.Dot))
                    {
                        Consume(TokenType.Dot); // "."
                        var memberName = Consume(TokenType.Identifier).Value; // e.g., "Deck" o "Hand"

                        if (Match(TokenType.OpenParen))
                        {
                            Consume(TokenType.OpenParen);
                            Consume(TokenType.CloseParen);

                            if (Match(TokenType.Equals))
                            {
                                var resultVarName = contextObject; // El nombre de la variable a la que se asignará
                                Consume(TokenType.Equals);

                                var rightHandSideBuilder = new StringBuilder();
                                rightHandSideBuilder.Append(contextObject);
                                rightHandSideBuilder.Append('.');
                                rightHandSideBuilder.Append(memberName);
                                rightHandSideBuilder.Append('(');
                                rightHandSideBuilder.Append(')');

                                Consume(TokenType.SemiColon); // Final de la instrucción

                                var rightHandSide = rightHandSideBuilder.ToString();

                                actions.Add((cards, ctx) =>
                                {
                                    if (memberName == "Pop")
                                    {
                                        var card = ctx.Deck.Pop(); // Llama al método
                                        localVariables[resultVarName] = card; // Asigna a la variable (ahora de tipo object)
                                    }
                                    else
                                    {
                                        ThrowSyntaxError($"Método desconocido '{memberName}' para la llamada de asignación.");
                                    }
                                });
                            }
                            else
                            {
                                Consume(TokenType.SemiColon); // Final de la instrucción
                                actions.Add((cards, ctx) =>
                                {
                                    if (memberName == "Shuffle")
                                    {
                                        ctx.Hand.Shuffle();
                                    }
                                    else
                                    {
                                        ThrowSyntaxError($"Método desconocido '{memberName}' para llamada sin asignación.");
                                    }
                                });
                            }
                        }
                        else if (Match(TokenType.SemiColon))
                        {
                            var methodCalls = new List<Action<List<Card>, Context>>();

                            do
                            {
                                var localContextObject = contextObject; // Guardar el objeto de contexto actual
                                Consume(TokenType.Dot);
                                var localMemberName = Consume(TokenType.Identifier).Value;

                                if (Match(TokenType.OpenParen))
                                {
                                    Consume(TokenType.OpenParen);
                                    Consume(TokenType.CloseParen);

                                    methodCalls.Add((cardList, ctx) =>
                                    {
                                        if (localMemberName == "Add")
                                        {
                                            if (localVariables.ContainsKey(localContextObject))
                                            {
                                                var card = localVariables[localContextObject] as Card;
                                                if (card != null)
                                                {
                                                    ctx.Hand.Add(card);
                                                }
                                                else
                                                {
                                                    ThrowSyntaxError($"Variable '{localContextObject}' no es del tipo correcto.");
                                                }
                                            }
                                            else
                                            {
                                                ThrowSyntaxError($"Variable '{localContextObject}' no encontrada.");
                                            }
                                        }
                                        else if (localMemberName == "Shuffle")
                                        {
                                            ctx.Hand.Shuffle();
                                        }
                                        else
                                        {
                                            ThrowSyntaxError($"Método desconocido '{localMemberName}' para llamada en secuencia.");
                                        }
                                    });

                                    if (Match(TokenType.Dot))
                                    {
                                        Consume(TokenType.Dot);
                                    }
                                }
                            } while (Match(TokenType.Dot)); // Continuar mientras haya métodos encadenados

                            Consume(TokenType.SemiColon); // Final de la instrucción

                            actions.Add((cardList, ctx) =>
                            {
                                foreach (var methodCall in methodCalls)
                                {
                                    methodCall(cardList, ctx); // Pasar `cardList` en lugar de `cards`
                                }
                            });
                        }
                    }


                    else if (Match(TokenType.Equals))
                    {
                        // Declaración de asignación, e.g., topCard = algúnValor;
                        Consume(TokenType.Equals);

                        // Crear un StringBuilder para construir el lado derecho de la asignación
                        var rightHandSideBuilder = new StringBuilder();
                        rightHandSideBuilder.Append(contextObject);
                        while (!Match(TokenType.SemiColon))
                        {
                            if (Match(TokenType.Identifier))
                            {
                                rightHandSideBuilder.Append(Consume(TokenType.Identifier).Value);
                            }
                            else if (Match(TokenType.Dot))
                            {
                                rightHandSideBuilder.Append('.');
                                Consume(TokenType.Dot);
                            }
                            else if (Match(TokenType.OpenParen))
                            {
                                rightHandSideBuilder.Append('(');
                                Consume(TokenType.OpenParen);
                                if (Match(TokenType.CloseParen))
                                {
                                    rightHandSideBuilder.Append(')');
                                    Consume(TokenType.CloseParen);
                                }
                            }
                            else if (Match(TokenType.CloseParen))
                            {
                                rightHandSideBuilder.Append(')');
                                Consume(TokenType.CloseParen);
                            }
                            else
                            {
                                ThrowSyntaxError($"Token inesperado '{_lexerStream.CurrentToken.Value}' en el lado derecho de la asignación.");
                            }
                        }
                        Consume(TokenType.SemiColon); // Final de la instrucción

                        var rightHandSide = rightHandSideBuilder.ToString();

                        actions.Add((cards, ctx) =>
                        {
                            if (localVariables.ContainsKey(rightHandSide))
                            {
                                localVariables[contextObject] = localVariables[rightHandSide]; // Asignación de variable
                            }
                            else if (rightHandSide == $"{contextObject}.Deck.Pop()")
                            {
                                var card = ctx.Deck.Pop();
                                localVariables[contextObject] = card; // Asignación de la carta
                            }
                            else
                            {
                                ThrowSyntaxError($"No se puede resolver '{rightHandSide}' para asignación.");
                            }
                        });
                    }
                }

                else
                {
                    ThrowSyntaxError($"Token inesperado '{_lexerStream.CurrentToken.Value}' en Action block");
                }
            }

            Consume(TokenType.CloseBrace); // Fin del bloque de acción

            return (cards, ctx) =>
            {
                foreach (var action in actions)
                {
                    action(cards, ctx);
                }
            };
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

                // Verifica si hay más propiedades y si se debe consumir una coma
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
                    case TokenType.Amount:
                        string amountValue = ParseProperty(TokenType.Amount);
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

                // Verifica si hay más propiedades y si se debe consumir una coma
                if (!Match(TokenType.CloseBrace) && !Match(TokenType.Comma))
                {
                    ThrowSyntaxError($"Expected a comma or closing brace in PostAction, but found '{_lexerStream.CurrentToken.Value}'");
                }

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }
            if (Match(TokenType.CloseBrace))
            {
                Consume(TokenType.CloseBrace);

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }

            return new PostAction(type, selector);
        }


        private string ParsePredicate()
        {
            // Confirmamos que hemos encontrado el token 'Predicate'
            Consume(TokenType.Predicate);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenParen); // Consume '('

            var predicate = new StringBuilder();

            // Consumimos el token Unit (ej. 'unit')
            var paramToken = Consume(TokenType.Unit); // Esto debería ser 'unit' o cualquier otro parámetro específico
            predicate.Append(paramToken.Value);

            Consume(TokenType.CloseParen); // Consume ')'

            // Verificamos si sigue una expresión lambda
            if (Match(TokenType.Arrow)) // TokenType.Arrow es '=>'
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
                        throw new LexerError($"Unexpected token '{token.Value}' at position {token.Pos}.");
                    }
                }
            }
            else
            {
                throw new LexerError("Expected '=>' after the parameter list in the predicate.");
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
                ThrowSyntaxError($"Expected token of type '{type}', but found '{LookAhead().Type}'", type);
            }
            Token token = _lexerStream.CurrentToken;
            _lexerStream.Advance();
            LogTokenConsumed(token);
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

        private void LogTokenConsumed(Token token)
        {
            Debug.Log($"Consumed Token: {token}");
        }

        private void ThrowSyntaxError(string message, TokenType expectedType = TokenType.None)
        {
            string errorMessage = $"Syntax Error: {message}. Found '{_lexerStream.CurrentToken.Value}' at position {_lexerStream.CurrentToken.Pos}.";
            Debug.LogError(errorMessage);
            throw new LexerError(errorMessage);
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

        private bool ParseBooleanProperty(TokenType propertyType)
        {
            string value = ParseProperty(propertyType);
            if (!bool.TryParse(value, out bool result))
            {
                ThrowSyntaxError($"Invalid boolean value '{value}' for property '{propertyType}'.");
            }
            return result;
        }

        private Token ConsumeAny(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Match(type))
                {
                    Token token = _lexerStream.CurrentToken;
                    _lexerStream.Advance();
                    LogTokenConsumed(token);
                    return token;
                }
            }

            ThrowSyntaxError($"Expected one of the following types: {string.Join(", ", types)} but found '{LookAhead().Type}'");
            return null; // Este retorno nunca se alcanzará, es solo para evitar errores de compilación
        }
        private Effect ParseCardEffect()
        {
            Consume(TokenType.Effect);
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
                            effectName = ParseProperty(TokenType.Name);
                            Consume(TokenType.Comma);
                            break;
                        case TokenType.String:
                            Token valueToken = ConsumeAny(TokenType.String);
                            effectName = valueToken.Value;
                            Consume(TokenType.Comma);
                            break;
                        case TokenType.Amount:
                            string amountValue = ParseProperty(TokenType.Amount);
                            Consume(TokenType.Comma);
                            if (int.TryParse(amountValue, out int parsedAmount))
                            {
                                amount = parsedAmount;
                            }
                            else
                            {
                                ThrowSyntaxError($"Expected 'Number' for property 'Amount', but found '{amountValue}'");
                            }
                            break;
                        default:
                            ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in Effect within a card");
                            break;
                    }

                    // _lexerStream.Advance();
                }
                if (Match(TokenType.CloseBrace))
                {
                    Consume(TokenType.CloseBrace);
                }

                // Crear el objeto Effect usando los datos parseados
                return new Effect(effectName, parameters, action); // action es de tipo Action<List<Card>, Context>
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
                Consume(TokenType.OpenBrace); // Consume la llave de apertura
            }

            if (Match(TokenType.Effect))
            {
                string effectName = ParseProperty(TokenType.Effect);
                Consume(TokenType.Comma);
                // Consume el posible cierre de llave si el efecto es un nombre simple
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
                string effectName = "";
                if (valueToken.Type == TokenType.String)
                {
                    effectName = valueToken.Value;
                    if (Match(TokenType.Comma))
                    {
                        Consume(TokenType.Comma);
                    }
                    if (!Match(TokenType.Amount))
                    {
                        return new Effect(effectName, new List<Parameter>(), null);
                    }
                    else if (Match(TokenType.Amount))
                    {

                        int? amount = null;
                        List<Parameter> parameters = new List<Parameter>();
                        Action<List<Card>, Context> action = null;
                        string amountValue = ParseProperty(TokenType.Amount);
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
                    return null; // Este retorno nunca se alcanzará, es solo para evitar errores de compilación
                }
            }

            return null; // Retorno por defecto si hay un error
        }

    }
}
