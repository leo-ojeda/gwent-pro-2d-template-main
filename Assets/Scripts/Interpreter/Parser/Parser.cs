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
                    SkipToNextCard();
                }
                catch (Exception)
                {
                    SkipToNextCard();
                }
            }
            return cards;
        }
        private void Synchronize()
        {
            // Saltar tokens hasta encontrar el inicio de la próxima carta o el final del archivo.
            while (_lexerStream.CurrentToken.Type != TokenType.Card &&
                   _lexerStream.CurrentToken.Type != TokenType.EOF)
            {
                _lexerStream.Advance();
            }
        }
        public bool IsEndOfInput()
        {
            return _lexerStream.CurrentToken.Type == TokenType.EOF;
        }

        public void SkipToNextCard()
        {
            while (!IsEndOfInput() && _lexerStream.CurrentToken.Type != TokenType.Card)
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

                // Verificamos si hay una coma para continuar
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

                _lexerStream.Advance(); // Avanza al siguiente token después de procesar uno.
            }

            Consume(TokenType.CloseBrace); // Espera el final del bloque de activación

            // Retorna el objeto EffectActivation con los valores finales
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

                parameters.Add(new Parameter(ParseParamType(paramType), paramValue));

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

            Consume(TokenType.CloseBrace);

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
                        token.Type == TokenType.Faction || token.Type == TokenType.Power) // <-- Aseguramos que Faction es válido
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
                    // case TokenType.Params:
                    //     parameters = ParseParameters();
                    //     break;
                    case TokenType.Selector:
                        Selector selector = ParseSelector();
                        action = (targets, context) =>
                        {
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
        private Effect ParseSimpleEffect()
        {
            Consume(TokenType.OpenBrace); // Consume la llave de apertura

            if (Match(TokenType.Effect))
            {
                string effectName = ParseProperty(TokenType.Effect);

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
            else
            {
                ThrowSyntaxError("Expected 'Effect' after '{'.");
            }

            return null; // Retorno por defecto si hay un error
        }

    }
}
