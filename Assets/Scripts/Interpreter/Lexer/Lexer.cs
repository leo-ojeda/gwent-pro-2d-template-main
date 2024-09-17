using System.Collections.Generic;
using System.Text;
using System;
using DSL.Parser;
using UnityEngine;
using Unity.VisualScripting;


namespace DSL.Lexer
{
    internal class Lexer
    {
        private readonly string _text;
        private int _currentCharIndex;
        private int _col;
        private int _line;
        private Stack<Token> groupTokens = new Stack<Token>();

        // Rastreador de la posición actual del carácter
        private Position CurrentPos => new Position(_line, _col);

        // Mapeo de palabras clave a tipos de tokens
        private readonly Dictionary<string, TokenType> _keyWordsTokens = new Dictionary<string, TokenType>()
        {
              {"card", TokenType.Card},
              {"effect", TokenType.Effect},
              {"Type", TokenType.Type},
              {"Name", TokenType.Name},
              {"Faction", TokenType.Faction},
              {"Power", TokenType.Power},
              {"Range", TokenType.Range},
              {"OnActivation", TokenType.OnActivation},
              {"Params", TokenType.Params},
              {"Action", TokenType.Action},
            //  {"Amount", TokenType.Amount},
              {"Source", TokenType.Source},
              {"Single", TokenType.Single},
              {"Predicate", TokenType.Predicate},
              {"Number", TokenType.Number},
              {"for", TokenType.For},
              {"in", TokenType.In},
              {"while", TokenType.While},
              {"Effect",TokenType.effect},
              {"return", TokenType.Return},
              {"triggerplayer", TokenType.TriggerPlayer},
              {"HandOfPlayer", TokenType.HandOfPlayer},
              {"FieldOfPlayer", TokenType.FieldOfPlayer},
              {"GraveyardOfPlayer", TokenType.GraveyardOfPlayer},
              {"DeckOfPlayer", TokenType.DeckOfPlayer},
              {"Hand", TokenType.Hand},
              {"Field", TokenType.Field},
              {"Graveyard", TokenType.Graveyard},
              {"Deck", TokenType.Deck},
              {"Board", TokenType.Board},
              {"Push", TokenType.Push},
              {"sendbottom", TokenType.SendBottom},
              {"Pop", TokenType.Pop},
              {"Remove", TokenType.Remove},
              {"Shuffle", TokenType.Shuffle},
              {"Find", TokenType.Find},
              {"Add",TokenType.Add},
              {"bool", TokenType.Bool},
              {"false", TokenType.False},
              {"true", TokenType.True},
              {"PostAction", TokenType.PostAction},
              {"Owner", TokenType.Owner},
              //{"unit", TokenType.Unit},
              {"Selector", TokenType.Selector},
        };

        // Carácter actual que se está procesando
        private char _currentChar => _currentCharIndex >= _text.Length ? '\0' : _text[_currentCharIndex];

        // El token que se está procesando actualmente
        internal Token CurrentToken { get; set; }

        // Constructor para inicializar el lexer con el texto fuente
        public Lexer(string text)
        {
            _text = text;
            CurrentToken = new Token(TokenType.SOF, "", new Position(0, -1));
        }

        // Avanza al siguiente token en el texto de entrada
        public void NextToken()
        {
            try
            {
                SkipWhiteSpaces(); // Omitir cualquier carácter de espacio en blanco
                if (_currentChar == '\0')
                {
                    CurrentToken = new Token(TokenType.EOF, "", CurrentPos);
                    if (groupTokens.Count != 0)
                    {
                        ThrowLexerError($"Caracter sin emparejar {groupTokens.Peek().Value} en {groupTokens.Peek().Pos}");
                    }
                    return;
                }

                switch (_currentChar)
                {

                    case '{':
                        CurrentToken = new Token(TokenType.OpenBrace, "{", CurrentPos);
                        CheckBalance(CurrentToken);
                        AdvanceChar();
                        break;
                    case '}':
                        CurrentToken = new Token(TokenType.CloseBrace, "}", CurrentPos);
                        CheckBalance(CurrentToken);
                        AdvanceChar();
                        break;
                    case '[':
                        CurrentToken = new Token(TokenType.OpenBracket, "[", CurrentPos);
                        CheckBalance(CurrentToken);
                        AdvanceChar();
                        break;
                    case ']':
                        CurrentToken = new Token(TokenType.CloseBracket, "]", CurrentPos);
                        CheckBalance(CurrentToken);
                        AdvanceChar();
                        break;
                    case '(':
                        // Contador de paréntesis avanzados
                        int parenthesesCount = 0;


                        // Avanza mientras siga encontrando '('
                        while (_currentChar == '(')
                        {
                            AdvanceChar();
                            parenthesesCount++;
                        }

                        // Verifica si el siguiente carácter es un número
                        if (char.IsDigit(_currentChar))
                        {
                            // Retrocede tantos caracteres como hayas avanzado
                            for (int i = 0; i < parenthesesCount; i++)
                            {
                                RetornoChar();
                            }

                            // Procesa el número completo
                            CurrentToken = NumberToken();
                             //AdvanceChar();
                        }
                        else
                        {
                            // Retrocede todos los caracteres avanzados
                            for (int i = 1; i < parenthesesCount; i++)
                            {
                                RetornoChar();
                            }

                            // Crea el token de paréntesis abierto y verifica el balance
                            CurrentToken = new Token(TokenType.OpenParen, "(", CurrentPos);
                            CheckBalance(CurrentToken);
                        }

                        break;

                    case ')':
                        // Captura el paréntesis de cierre
                        AdvanceChar();

                        // Verifica si después del paréntesis viene un número
                        if (char.IsDigit(_currentChar))
                        {
                            RetornoChar();
                            NumberToken(); // Procesa el número completo
                        }
                        else
                        {
                            CurrentToken = new Token(TokenType.CloseParen, ")", CurrentPos);
                            CheckBalance(CurrentToken);

                        }

                        break;
                    case ':':
                        CurrentToken = new Token(TokenType.Colon, ":", CurrentPos);
                        AdvanceChar();
                        break;
                    case ';':
                        CurrentToken = new Token(TokenType.SemiColon, ";", CurrentPos);
                        AdvanceChar();
                        break;
                    case ',':
                        CurrentToken = new Token(TokenType.Comma, ",", CurrentPos);
                        AdvanceChar();
                        break;
                    case '+':
                        if (LookAhead() == '+')
                        {
                            CurrentToken = new Token(TokenType.Increment, "++", CurrentPos);
                            AdvanceChar();
                            AdvanceChar();
                        }
                        else
                        {
                            CurrentToken = new Token(TokenType.Plus, "+", CurrentPos);
                            AdvanceChar();
                        }
                        break;
                    case '-':
                        CurrentToken = new Token(TokenType.Minus, "-", CurrentPos);
                        AdvanceChar();
                        break;
                    case '^':
                        CurrentToken = new Token(TokenType.Pow, "^", CurrentPos);
                        AdvanceChar();
                        break;
                    case '*':
                        CurrentToken = new Token(TokenType.Multiply, "*", CurrentPos);
                        AdvanceChar();
                        break;
                    case '/':
                        CurrentToken = new Token(TokenType.Slash, "/", CurrentPos);
                        AdvanceChar();
                        break;
                    // case '%':
                    //     CurrentToken = new Token(TokenType.Modulus, "%", CurrentPos);
                    //     AdvanceChar();
                    //     break;
                    case '.':
                        CurrentToken = new Token(TokenType.Dot, ".", CurrentPos);
                        AdvanceChar();
                        break;
                    case '=':
                        if (LookAhead() == '>')
                        {
                            CurrentToken = new Token(TokenType.Arrow, "=>", CurrentPos);
                            AdvanceChar();
                            AdvanceChar();
                        }
                        else if (LookAhead() == '=')
                        {
                            CurrentToken = new Token(TokenType.EqualsEquals, "==", CurrentPos);
                            AdvanceChar();
                            AdvanceChar();
                        }
                        else
                        {
                            CurrentToken = new Token(TokenType.Equals, "=", CurrentPos);
                            AdvanceChar();
                        }
                        break;
                    case '<':
                        if (LookAhead() == '=')
                        {
                            CurrentToken = new Token(TokenType.LessOrEqual, "<=", CurrentPos);
                            AdvanceChar();
                            AdvanceChar();
                        }
                        else
                        {
                            CurrentToken = new Token(TokenType.Less, "<", CurrentPos);
                            AdvanceChar();
                        }
                        break;
                    case '>':
                        if (LookAhead() == '=')
                        {
                            CurrentToken = new Token(TokenType.GreaterOrEqual, ">=", CurrentPos);
                            AdvanceChar();
                            AdvanceChar();
                        }
                        else
                        {
                            CurrentToken = new Token(TokenType.Greater, ">", CurrentPos);
                            AdvanceChar();
                        }
                        break;
                    case '&':
                        if (LookAhead() == '&')
                        {
                            CurrentToken = new Token(TokenType.And, "&&", CurrentPos);
                            AdvanceChar();
                            AdvanceChar();
                        }
                        else
                        {
                            ThrowLexerError($"Caracter inesperado: {_currentChar}");
                        }
                        break;
                    case '|':
                        if (LookAhead() == '|')
                        {
                            CurrentToken = new Token(TokenType.Or, "||", CurrentPos);
                            AdvanceChar();
                            AdvanceChar();
                        }
                        else
                        {
                            ThrowLexerError($"Caracter inesperado: {_currentChar}");
                        }
                        break;
                    case '@':
                        if (LookAhead() == '@')
                        {
                            // Verifica si el token anterior es un string
                            if (CurrentToken != null && CurrentToken.Type == TokenType.String)

                            {


                                AdvanceChar(); // Avanzar para saltar el primer '@'
                                AdvanceChar(); // Avanzar para saltar el segundo '@'

                                SkipWhiteSpaces(); // Omitir cualquier espacio en blanco antes del siguiente string

                                // Leer el siguiente string y concatenarlo
                                var nextStringToken = StringToken(); // Obtener el siguiente string
                                var concatenatedValue = CurrentToken.Value.Trim() + nextStringToken.Value.Trim(); // Concatenar eliminando espacios

                                // Crear un nuevo token con el valor concatenado
                                CurrentToken = new Token(TokenType.String, concatenatedValue, CurrentPos);
                            }
                            else
                            {
                                ThrowLexerError("Se esperaba un string antes de '@@'");
                            }
                        }
                        else
                        {
                            CurrentToken = new Token(TokenType.At, "@", CurrentPos);
                            AdvanceChar();
                        }
                        break;



                    case '!':
                        if (LookAhead() == '=')
                        {
                            CurrentToken = new Token(TokenType.NotEqual, "!=", CurrentPos);
                            AdvanceChar();
                            AdvanceChar();
                        }
                        else
                        {
                            ThrowLexerError($"Caracter inesperado: {_currentChar}");
                        }
                        break;
                    case '"':
                        // Leer la primera cadena
                        var stringToken = StringToken(); // Obtener el primer token de cadena

                        // Comprobar si hay `@@` para concatenar
                        while (_currentChar == '@' && LookAhead() == '@')
                        {

                            AdvanceChar(); // Saltar primer '@'
                            AdvanceChar(); // Saltar segundo '@'

                            SkipWhiteSpaces(); // Omitir cualquier espacio en blanco entre las cadenas

                            // Leer la siguiente cadena y concatenarla
                            string nextString = StringToken().Value.Trim(); // Eliminar espacios alrededor de la cadena

                            // Concatenar el valor de ambas cadenas
                            string concatenatedValue = stringToken.Value.Trim() + nextString; // También eliminar espacios de la primera cadena

                            // Crear un nuevo token con el valor concatenado
                            stringToken = new Token(TokenType.String, concatenatedValue, CurrentPos);
                        }

                        // Asignar el token final con el valor concatenado a CurrentToken
                        CurrentToken = stringToken;
                        break;



                    default:
                        if (IsDigit(_currentChar))
                        {
                            CurrentToken = NumberToken();
                        }
                        else if (IsLetter(_currentChar))
                        {
                            CurrentToken = VariableOrIdentifierToken();
                        }
                        else
                        {
                            ThrowLexerError($"Caracter desconocido: {_currentChar}");
                        }
                        break;
                }
            }
            catch (Error ex)
            {
                Debug.LogError($"Lexer Error: {ex.Message}");
                AdvanceChar();
            }
        }


        private void ThrowLexerError(string message)
        {
            string errorMessage = $"Error léxico: {message}. Se encontró '{_currentChar}' en la posición {CurrentPos}.";
            Debug.LogError(errorMessage);
            throw new Error(errorMessage, CurrentPos);
        }


        // Método para obtener el próximo carácter sin avanzar la posición actual.
        private char LookAhead()
        {
            int peekPosition = _currentCharIndex + 1;
            return peekPosition < _text.Length ? _text[peekPosition] : '\0';
        }

        // Procesa un token de identificador o palabra clave
        private Token IdentifierToken()
        {
            StringBuilder sb = new StringBuilder();

            while (IsAlphaNumeric(_currentChar))
            {
                sb.Append(_currentChar);
                AdvanceChar();
            }

            string tokenString = sb.ToString();


            if (_keyWordsTokens.ContainsKey(tokenString))
            {
                return new Token(_keyWordsTokens[tokenString], tokenString, CurrentPos);
            }
            else
            {
                return new Token(TokenType.Identifier, tokenString, CurrentPos);
            }
        }
        private Token VariableOrIdentifierToken()
        {
            StringBuilder sb = new StringBuilder();

            while (IsAlphaNumeric(_currentChar))
            {
                sb.Append(_currentChar);
                AdvanceChar();
            }

            string tokenString = sb.ToString();

            // Si el siguiente carácter es '=' y el token actual no está vacío, es una variable
            if (_currentChar == '=' && tokenString.Length > 0)
            {
                AdvanceChar(); // Consumir '='
                return new Token(TokenType.Variable, tokenString, CurrentPos);
            }

            if (_keyWordsTokens.ContainsKey(tokenString))
            {
                return new Token(_keyWordsTokens[tokenString], tokenString, CurrentPos);
            }
            else
            {
                return new Token(TokenType.Identifier, tokenString, CurrentPos);
            }
        }

        // Procesa un token numérico
        private ExpressionEvaluator _expressionEvaluator = new ExpressionEvaluator();

        private Token NumberToken()
        {
            
            StringBuilder sb = new StringBuilder();
            
            int parenthesesBalance = 0; // Para controlar el balance de paréntesis

            // Captura la expresión completa, incluyendo operadores, paréntesis y números
            while (IsDigit(_currentChar) || _currentChar == '+' || _currentChar == '-' ||
                   _currentChar == '*' || _currentChar == '/' || _currentChar == '^' ||
                   _currentChar == '.' || _currentChar == ' ' || _currentChar == '(' || _currentChar == ')')
            {
                // Controlar el balance de paréntesis
                if (_currentChar == '(') parenthesesBalance++;
                if (_currentChar == ')') parenthesesBalance--;

                sb.Append(_currentChar);
                AdvanceChar();

                // Salir si hay un desbalance en los paréntesis
                if (parenthesesBalance < 0)
                {
                    ThrowLexerError("Desbalance de paréntesis: más paréntesis de cierre que apertura.");
                    return new Token(TokenType.Number, "0", CurrentPos); // Retorna un valor por defecto en caso de error
                }
            }

            // Finaliza el procesamiento si el balance de paréntesis no es cero
            if (parenthesesBalance != 0)
            {
                ThrowLexerError("Desbalance de paréntesis en la expresión.");
                return new Token(TokenType.Number, "0", CurrentPos); // Retorna un valor por defecto en caso de error
            }

            string expression = sb.ToString().Trim();
            Debug.Log(expression);

            // Evaluar la expresión aritmética capturada y obtener el resultado final
            string evaluatedResult;
            try
            {
                evaluatedResult = _expressionEvaluator.EvaluateExpression(expression);
            }
            catch (Exception ex)
            {
                ThrowLexerError("Error al evaluar la expresión: " + ex.Message);
                return new Token(TokenType.Number, "0", CurrentPos); // Retorna un valor por defecto en caso de error
            }
            Debug.Log(evaluatedResult);

            // Crear el token usando el valor evaluado

            return new Token(TokenType.Number, evaluatedResult, CurrentPos);
        }






        // Procesa un token de cadena (string)
        private Token StringToken()
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Asumo que la cadena comienza con comillas
            if (_currentChar == '"')
            {
                AdvanceChar(); // Omitir la comilla de apertura

                while (_currentChar != '"' && _currentChar != '\0')
                {
                    stringBuilder.Append(_currentChar);
                    AdvanceChar();
                }

                if (_currentChar == '"')
                {
                    AdvanceChar(); // Omitir la comilla de cierre
                    SkipWhiteSpaces();
                }
                else
                {
                    ThrowLexerError("Cadena no cerrada.");
                }
            }
            else
            {
                ThrowLexerError("Se esperaba una comilla para la cadena.");
            }

            return new Token(TokenType.String, stringBuilder.ToString(), CurrentPos);
        }


        // Verifica si un carácter es alfanumérico (letra o número)
        private bool IsAlphaNumeric(char c) => IsLetter(c) || IsDigit(c);

        // Verifica si un carácter es una letra
        private bool IsLetter(char c) => char.IsLetter(c);

        // Verifica si un carácter es un dígito
        private bool IsDigit(char c) => char.IsDigit(c);

        // Avanza al siguiente carácter en la entrada
        private void AdvanceChar()
        {
            if (_currentChar == '\n')
            {
                _line++;
                _col = 0;
            }
            else
            {
                _col++;
            }
            _currentCharIndex++;
        }
        private void RetornoChar()
        {
            if (_currentChar == '\n')
            {
                _line--;
                _col = 0;
            }
            else
            {
                _col--;
            }
            _currentCharIndex--;
        }
        private char LookAhead(int offset = 1)
        {
            // Guardar el estado actual
            int currentIndex = _currentCharIndex;

            // Mover el índice al próximo carácter sin cambiar el estado actual
            int lookAheadIndex = _currentCharIndex + offset;

            // Asegurarse de que el índice está dentro del rango válido
            if (lookAheadIndex >= _text.Length)
            {
                return '\0'; // Retorna un carácter nulo si se excede el rango
            }

            // Obtener el carácter en el índice de adelanto
            char lookAheadChar = _text[lookAheadIndex];

            // Restaurar el estado original
            return lookAheadChar;
        }


        // Omitir cualquier carácter de espacio en blanco (incluye saltos de línea y espacios)
        private void SkipWhiteSpaces()
        {
            while (char.IsWhiteSpace(_currentChar))
            {
                AdvanceChar();
            }
        }

        // Verifica el balanceo de los delimitadores ({}, [], (), etc.)
        private void CheckBalance(Token token)
        {
            if (token.Type == TokenType.OpenBrace || token.Type == TokenType.OpenBracket || token.Type == TokenType.OpenParen)
            {
                groupTokens.Push(token);
            }
            else if (token.Type == TokenType.CloseBrace || token.Type == TokenType.CloseBracket || token.Type == TokenType.CloseParen)
            {
                if (groupTokens.Count == 0)
                {
                    ThrowLexerError($"Caracter sin emparejar {token.Value} en {token.Pos}");
                }

                Token lastOpen = groupTokens.Pop();

                if (!MatchingPairs(lastOpen, token))
                {
                    ThrowLexerError($"Caracter sin emparejar {token.Value} en {token.Pos}");
                }
            }
        }


        // Verifica si los delimitadores coinciden ({}, [], (), etc.)
        private bool MatchingPairs(Token open, Token close)
        {
            return (open.Type == TokenType.OpenBrace && close.Type == TokenType.CloseBrace)
                || (open.Type == TokenType.OpenBracket && close.Type == TokenType.CloseBracket)
                || (open.Type == TokenType.OpenParen && close.Type == TokenType.CloseParen);
        }




    }
}
