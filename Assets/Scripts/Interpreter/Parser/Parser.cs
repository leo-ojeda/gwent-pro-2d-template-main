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

        public Card ParseCard()
        {
            try
            {
                LogCurrentToken("Parsing card");

                if (_lexerStream.CurrentToken.Type != TokenType.Card)
                {
                    ThrowSyntaxError($"Expected 'card' at the beginning, but found '{_lexerStream.CurrentToken.Value}'");
                }

                Consume(TokenType.Card);
                Consume(TokenType.OpenBrace);

                string type = null;
                string name = null;
                string faction = null;
                int power = 0;
                List<string> range = new List<string>();
                List<EffectActivation> onActivation = new List<EffectActivation>();

                int openBraces = 1; // Contador para las llaves

                while (openBraces > 0)
                {
                    if (Match(TokenType.OpenBrace))
                    {
                        openBraces++;
                    }
                    else if (Match(TokenType.CloseBrace))
                    {
                        openBraces--;
                    }
                    else if (Match(TokenType.Type))
                    {
                        type = ParseProperty(TokenType.Type);
                    }
                    else if (Match(TokenType.Name))
                    {
                        name = ParseProperty(TokenType.Name);
                    }
                    else if (Match(TokenType.Faction))
                    {
                        faction = ParseProperty(TokenType.Faction);
                    }
                    else if (Match(TokenType.Power))
                    {
                        power = ParseIntegerProperty(TokenType.Power);
                    }
                    else if (Match(TokenType.Range))
                    {
                        range = ParseListProperty(TokenType.Range);
                    }
                    else if (Match(TokenType.OnActivation))
                    {
                        onActivation = ParseEffectActivationList();
                    }
                    else if (Match(TokenType.Comma))
                    {
                        // Ignorar la coma y continuar
                        Consume(TokenType.Comma);
                    }
                    else
                    {
                        ThrowSyntaxError($"Unexpected token '{LookAhead().Value}'");
                    }
                }

                Consume(TokenType.CloseBrace);

                // Verificar que todos los delimitadores estén balanceados
                if (openBraces != 0)
                {
                    ThrowSyntaxError("Mismatched braces detected.");
                }

                return new Card(name, power, type, range.ToArray(), faction, onActivation, null);
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
            Token valueToken = ConsumeAny(TokenType.String, TokenType.Identifier);
            if (valueToken.Type == TokenType.String || valueToken.Type == TokenType.Identifier)
            {
                return valueToken.Value;
            }
            else
            {
                ThrowSyntaxError($"Expected 'String' or 'Identifier' for property '{propertyType}', but found '{valueToken.Type}'");
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

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }

            // Consume el corchete de cierre
            Consume(TokenType.CloseBracket);
            return activations;
        }

        private EffectActivation ParseEffectActivation()
        {
            Consume(TokenType.OpenBrace); // Espera el inicio del bloque de activación

            Effect effect = null;
            Selector selector = null;
            PostAction postAction = null;

            while (!Match(TokenType.CloseBrace))
            {
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
                    default:
                        ThrowSyntaxError($"Token inesperado '{_lexerStream.CurrentToken.Value}' en EffectActivation");
                        break;
                }

                _lexerStream.Advance(); // Avanza al siguiente token después de procesar uno.
            }

            Consume(TokenType.CloseBrace); // Espera el final del bloque de activación
            return new EffectActivation(effect, selector, postAction);
        }



        private Effect ParseEffect()
        {
            Consume(TokenType.Effect);
            Consume(TokenType.OpenBrace);

            string effectName = null;
            List<Parameter> parameters = new List<Parameter>();
            Action<List<Card>, Context> action = null;

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Name:
                        effectName = ParseProperty(TokenType.Name);
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

            Consume(TokenType.CloseBrace);
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
                
                Consume(TokenType.Colon);
                string paramType = Consume(TokenType.Identifier).Value;

                object paramValue = null;
                if (Match(TokenType.Equals)) // Asume que usas '=' para asignar un valor
                {
                    Consume(TokenType.Equals);
                    paramValue = ParseParameterValue(ParseParamType(paramType)); // Nuevo método para obtener el valor basado en el tipo
                }

                parameters.Add(new Parameter( ParseParamType(paramType), paramValue));

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }

            Consume(TokenType.CloseBrace);
            return parameters;
        }

        private object ParseParameterValue(ParamType type)
        {
            switch (type)
            {
                case ParamType.Number:
                    return double.Parse(Consume(TokenType.Number).Value);
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

            string actionBody = "";
            while (!Match(TokenType.CloseBrace))
            {
                var token = _lexerStream.CurrentToken;
                actionBody += token.Value + " ";
                _lexerStream.Advance();
            }

            Consume(TokenType.CloseBrace); // "}"

            return (targets, context) =>
            {
                Debug.Log($"Executing action: {actionBody}");
            };
        }

        private ParamType ParseParamType(string type)
        {
            return type.ToLower() switch
            {
                "number" => ParamType.Number,
                "string" => ParamType.String,
                "bool" => ParamType.Bool,
                _ => throw new LexerError($"Unrecognized parameter type: {type}")
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

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Source:
                        source = ParseProperty(TokenType.Source);
                        break;
                    case TokenType.Single:
                        string singleValue = ParseProperty(TokenType.Single);
                        if (bool.TryParse(singleValue, out bool parsedSingle))
                        {
                            single = parsedSingle;
                        }
                        else
                        {
                            ThrowSyntaxError($"Expected 'Bool' for property 'Single', but found '{singleValue}'");
                        }
                        break;
                    case TokenType.Predicate:
                        predicate = ParseProperty(TokenType.Predicate);
                        break;
                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in Selector");
                        break;
                }

                _lexerStream.Advance();
            }

            Consume(TokenType.CloseBrace);

            return new Selector(source, single, predicate);
        }
        private PostAction ParsePostAction()
        {
            Consume(TokenType.PostAction);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBrace);

            string type = null;
            Selector selector = null;

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Type:
                        type = ParseProperty(TokenType.Type);
                        break;
                    case TokenType.Selector:
                        selector = ParseSelector();
                        break;
                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in PostAction");
                        break;
                }

                _lexerStream.Advance();
            }

            Consume(TokenType.CloseBrace);

            return new PostAction(type, selector);
        }
        private string ParsePredicate()
        {
            Consume(TokenType.OpenParen);
            var predicate = new StringBuilder();

            while (!Match(TokenType.CloseParen))
            {
                var token = Consume(LookAhead().Type);

                if (token.Type == TokenType.Identifier || token.Type == TokenType.String || token.Type == TokenType.Number)
                {
                    predicate.Append(token.Value);
                }
                else if (IsOperator(token.Type))
                {
                    predicate.Append($" {token.Value} ");
                }
                else
                {
                    throw new LexerError($"Unexpected token '{token.Value}' at position {token.Pos}.");
                }
            }

            Consume(TokenType.CloseParen);
            return predicate.ToString();
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

        private bool Match(TokenType type)
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

        private void ThrowSyntaxError(string message, TokenType expected = TokenType.None)
        {
            var token = LookAhead();
            string errorMessage = expected == TokenType.None
                ? $"Syntax Error: {message} at position {token.Pos} (found '{token.Value}')"
                : $"Syntax Error: {message} at position {token.Pos} (found '{token.Value}', expected '{expected}')";

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
            Consume(TokenType.OpenBrace);

            string effectName = null;
            int? amount = null;
            List<Parameter> parameters = new List<Parameter>();
            Action<List<Card>, Context> action = null;
            PostAction postAction = null;

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Name:
                        effectName = ParseProperty(TokenType.Name);
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
                    case TokenType.Params:
                        parameters = ParseParameters();
                        break;
                    case TokenType.Selector:
                        Selector selector = ParseSelector();
                        action = (targets, context) =>
                        {
                            // Aquí puedes usar el selector para definir la lógica de la acción
                            Debug.Log($"Executing action with selector: {selector.source}");
                        };
                        break;
                    case TokenType.PostAction:
                        postAction = ParsePostAction();
                        // Aquí puedes incorporar postAction a tu lógica si es necesario
                        break;
                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in Effect within a card");
                        break;
                }

                _lexerStream.Advance();
            }

            Consume(TokenType.CloseBrace);

            // Crear el objeto Effect usando los datos parseados
            return new Effect(effectName, parameters, action); // action es de tipo Action<List<Card>, Context>
        }





    }
}
