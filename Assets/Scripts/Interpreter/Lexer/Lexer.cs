using System.Collections.Generic;
using System.Text;

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
              {"type", TokenType.Type},
              {"name", TokenType.Name},
              {"faction", TokenType.Faction},
              {"power", TokenType.Power},
              {"range", TokenType.Range},
              {"onactivation", TokenType.OnActivation},
              {"params", TokenType.Params},
              {"action", TokenType.Action},
              {"amount", TokenType.Amount},
              {"source", TokenType.Source},
              {"single", TokenType.Single},
              {"predicate", TokenType.Predicate},
              {"number", TokenType.Number},
              {"for", TokenType.For},
              {"in", TokenType.In},
              {"while", TokenType.While},
              {"return", TokenType.Return},
             // {"triggerplayer", TokenType.TriggerPlayer},
             // {"handoffplayer", TokenType.HandOfPlayer},
             // {"fieldofplayer", TokenType.FieldOfPlayer},
             // {"graveyardofplayer", TokenType.GraveyardOfPlayer},
             // {"deckofplayer", TokenType.DeckOfPlayer},
             // {"hand", TokenType.Hand},
             // {"field", TokenType.Field},
             // {"graveyard", TokenType.Graveyard},
             // {"deck", TokenType.Deck},
             // {"board", TokenType.Board},
             // {"push", TokenType.Push},
             // {"sendbottom", TokenType.SendBottom},
             // {"pop", TokenType.Pop},
             // {"remove", TokenType.Remove},
             // {"shuffle", TokenType.Shuffle},
             // {"find", TokenType.Find},
              {"bool", TokenType.Bool},
              {"false", TokenType.False},
              {"true", TokenType.True},
              {"postaction", TokenType.PostAction},
              {"owner", TokenType.Owner},
              {"unit", TokenType.Unit},
              {"selector", TokenType.Selector},
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
            SkipWhiteSpaces(); // Omitir cualquier carácter de espacio en blanco

            switch (_currentChar)
            {
                case '\0':
                    CurrentToken = new Token(TokenType.EOF, "", CurrentPos);
                    if (groupTokens.Count != 0)
                    {
                        throw new LexerError($"Caracter sin emparejar {groupTokens.Peek().Value} en {groupTokens.Peek().Pos}", CurrentPos);
                    }
                    AdvanceChar();
                    break;
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
                    CurrentToken = new Token(TokenType.OpenParen, "(", CurrentPos);
                    CheckBalance(CurrentToken);
                    AdvanceChar();
                    break;
                case ')':
                    CurrentToken = new Token(TokenType.CloseParen, ")", CurrentPos);
                    CheckBalance(CurrentToken);
                    AdvanceChar();
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
                case '*':
                    CurrentToken = new Token(TokenType.Multiply, "*", CurrentPos);
                    AdvanceChar();
                    break;
                case '/':
                    CurrentToken = new Token(TokenType.Slash, "/", CurrentPos);
                    AdvanceChar();
                    break;
                case '%':
                    CurrentToken = new Token(TokenType.Modulus, "%", CurrentPos);
                    AdvanceChar();
                    break;
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
                        throw new LexerError($"Caracter inesperado: {_currentChar} en {CurrentPos}", CurrentPos);
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
                        throw new LexerError($"Caracter inesperado: {_currentChar} en {CurrentPos}", CurrentPos);
                    }
                    break;
                case '@':
                    if (LookAhead() == '@')
                    {
                        CurrentToken = new Token(TokenType.DoubleAt, "@@", CurrentPos);
                        AdvanceChar();
                        AdvanceChar();
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
                        throw new LexerError($"Caracter inesperado: {_currentChar} en {CurrentPos}", CurrentPos);
                    }
                    break;
                case '"':
                    CurrentToken = StringToken();
                    break;
                default:
                    if (IsDigit(_currentChar))
                    {
                        CurrentToken = NumberToken();
                    }
                    else if (IsLetter(_currentChar))
                    {
                        CurrentToken = IdentifierToken();
                    }
                    else
                    {
                        throw new LexerError($"Caracter desconocido: {_currentChar} en {CurrentPos}", CurrentPos);
                    }
                    break;
            }
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
            string normalizedTokenString = tokenString.ToLower(); // Convertir a minúsculas

            if (_keyWordsTokens.ContainsKey(normalizedTokenString))
            {
                return new Token(_keyWordsTokens[normalizedTokenString], tokenString, CurrentPos);
            }
            else
            {
                return new Token(TokenType.Identifier, tokenString, CurrentPos);
            }
        }


        // Procesa un token numérico
        private Token NumberToken()
        {
            StringBuilder sb = new StringBuilder();
            while (IsDigit(_currentChar))
            {
                sb.Append(_currentChar);
                AdvanceChar();
            }
            return new Token(TokenType.Number, sb.ToString(), CurrentPos);
        }

        // Procesa un token de cadena (string)
        private Token StringToken()
        {
            StringBuilder sb = new StringBuilder();
            AdvanceChar(); // omite el primer '"'

            while (_currentChar != '\0' && _currentChar != '"')
            {
                sb.Append(_currentChar);
                AdvanceChar();
            }

            if (_currentChar == '"')
            {
                AdvanceChar(); // omite el segundo '"'
                return new Token(TokenType.String, sb.ToString(), CurrentPos);
            }
            else
            {
                throw new LexerError("Cadena no terminada", CurrentPos);
            }
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
                    throw new LexerError($"Caracter sin emparejar {token.Value} en {token.Pos}", token.Pos);
                }

                Token lastOpen = groupTokens.Pop();

                if (!MatchingPairs(lastOpen, token))
                {
                    throw new LexerError($"Caracter sin emparejar {token.Value} en {token.Pos}", token.Pos);
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
