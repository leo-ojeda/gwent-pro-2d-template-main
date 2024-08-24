using System;
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
        Position currentPos { get => new Position(_line, _col); }
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
            {"Amount", TokenType.Amount},
            {"Source", TokenType.Source},
            {"Single", TokenType.Single},
            {"Predicate", TokenType.Predicate},
            {"Number", TokenType.NumberType},
            {"for", TokenType.For},
            {"in", TokenType.In},
            {"while", TokenType.While},
            {"return", TokenType.Return}
            // Agrega más palabras clave según sea necesario
        };

        private char _currentChar => _currentCharIndex >= _text.Length ? '\0' : _text[_currentCharIndex];

        internal Token CurrentToken { get; set; }

        public Lexer(string text)
        {
            _text = text;
            CurrentToken = new Token(TokenType.SOF, "", new Position(0, -1));
        }

        public void NextToken()
{
    SkipWhiteSpaces();

    switch (_currentChar)
    {
        case '\0':
            CurrentToken = new Token(TokenType.EOF, "", currentPos);
            if (groupTokens.Count != 0)
            {
                throw new Exception($"Unmatched {groupTokens.Peek().Value} on {groupTokens.Peek().Pos}");
            }
            AdvanceChar();
            break;
        case '{':
            CurrentToken = new Token(TokenType.OpenCurlyBracket, "{", currentPos);
            CheckBalance(CurrentToken);
            AdvanceChar();
            break;
        case '}':
            CurrentToken = new Token(TokenType.ClosedCurlyBracket, "}", currentPos);
            CheckBalance(CurrentToken);
            AdvanceChar();
            break;
        case '[':
            CurrentToken = new Token(TokenType.OpenSquareBracket, "[", currentPos);
            CheckBalance(CurrentToken);
            AdvanceChar();
            break;
        case ']':
            CurrentToken = new Token(TokenType.ClosedSquareBracket, "]", currentPos);
            CheckBalance(CurrentToken);
            AdvanceChar();
            break;
        case '(':
            CurrentToken = new Token(TokenType.OpenParenthesis, "(", currentPos);
            CheckBalance(CurrentToken);
            AdvanceChar();
            break;
        case ')':
            CurrentToken = new Token(TokenType.ClosedParenthesis, ")", currentPos);
            CheckBalance(CurrentToken);
            AdvanceChar();
            break;
        case ':':
            CurrentToken = new Token(TokenType.Colon, ":", currentPos);
            AdvanceChar();
            break;
        case ',':
            CurrentToken = new Token(TokenType.Comma, ",", currentPos);
            AdvanceChar();
            break;
        case '=':
            CurrentToken = new Token(TokenType.Equals, "=", currentPos);
            AdvanceChar();
            break;
        case '+':
            CurrentToken = new Token(TokenType.Plus, "+", currentPos);
            AdvanceChar();
            break;
        case '-':
            CurrentToken = new Token(TokenType.Minus, "-", currentPos);
            AdvanceChar();
            break;
        case '*':
            CurrentToken = new Token(TokenType.Multiply, "*", currentPos);
            AdvanceChar();
            break;
        case '/':
            CurrentToken = new Token(TokenType.Slash, "/", currentPos);
            AdvanceChar();
            break;
        case '%':
            CurrentToken = new Token(TokenType.Modulus, "%", currentPos);
            AdvanceChar();
            break;
        case '<':
            CurrentToken = WithLessToken();
            break;
        case '>':
            CurrentToken = WithGreaterToken();
            break;
        case '!':
            CurrentToken = WithExclamationToken();
            break;
        case '"':
            CurrentToken = StringToken();
            break;
        default:
            if (char.IsDigit(_currentChar))
            {
                CurrentToken = NumberToken();
            }
            else if (char.IsLetter(_currentChar))
            {
                CurrentToken = IdentifierToken();
            }
            else
            {
                throw new Exception($"Unknown character: {_currentChar} at {currentPos}");
            }
            break;
    }
}



        private Token IdentifierToken()
        {
            StringBuilder sb = new StringBuilder();
            while (char.IsLetter(_currentChar) || char.IsDigit(_currentChar))
            {
                sb.Append(_currentChar);
                AdvanceChar();
            }

            string tokenString = sb.ToString();
            if (_keyWordsTokens.ContainsKey(tokenString))
            {
                return new Token(_keyWordsTokens[tokenString], tokenString, currentPos);
            }
            else
            {
                return new Token(TokenType.Identifier, tokenString, currentPos);
            }
        }

        private Token NumberToken()
        {
            StringBuilder sb = new StringBuilder();
            while (char.IsDigit(_currentChar))
            {
                sb.Append(_currentChar);
                AdvanceChar();
            }
            return new Token(TokenType.Number, sb.ToString(), currentPos);
        }

        private Token StringToken()
        {
            StringBuilder sb = new StringBuilder();
            AdvanceChar(); // Skip the opening quote
            while (_currentChar != '"' && _currentChar != '\0')
            {
                sb.Append(_currentChar);
                AdvanceChar();
            }

            if (_currentChar == '"')
            {
                AdvanceChar(); // Skip the closing quote
                return new Token(TokenType.String, sb.ToString(), currentPos);
            }
            else
            {
                throw new Exception("Lexical error, expected closing \" ");
            }
        }

        private void SkipWhiteSpaces()
        {
            while (char.IsWhiteSpace(_currentChar))
            {
                if (_currentChar == '\n')
                {
                    AdvanceLine();
                }
                else
                {
                    AdvanceChar();
                }
            }
        }

        private void AdvanceChar()
        {
            _currentCharIndex++;
            _col++;
        }

        private void AdvanceLine()
        {
            _line++;
            _col = 0;
        }

        private void CheckBalance(Token currentToken)
        {
            Dictionary<TokenType, TokenType> couples = new()
            {
                {TokenType.ClosedParenthesis, TokenType.OpenParenthesis},
                {TokenType.ClosedSquareBracket, TokenType.OpenSquareBracket},
                {TokenType.ClosedCurlyBracket, TokenType.OpenCurlyBracket}
            };

            if (couples.ContainsValue(currentToken.Type))
            {
                groupTokens.Push(new Token(currentToken));
            }
            else
            {
                if (groupTokens.Count == 0 || couples[currentToken.Type] != groupTokens.Peek().Type)
                {
                    throw new Exception($"Unmatched {currentToken.Value} at {currentToken.Pos}");
                }
                groupTokens.Pop();
            }
        }
        private Token WithLessToken()
        {
            AdvanceChar(); // Skip '<'
            if (_currentChar == '=')
            {
                AdvanceChar(); // Skip '='
                return new Token(TokenType.LessOrEqual, "<=", currentPos);
            }
            else
            {
                return new Token(TokenType.Less, "<", currentPos);
            }
        }

        private Token WithGreaterToken()
        {
            AdvanceChar(); // Skip '>'
            if (_currentChar == '=')
            {
                AdvanceChar(); // Skip '='
                return new Token(TokenType.GreaterOrEqual, ">=", currentPos);
            }
            else
            {
                return new Token(TokenType.Greater, ">", currentPos);
            }
        }

        private Token WithExclamationToken()
        {
            AdvanceChar(); // Skip '!'
            if (_currentChar == '=')
            {
                AdvanceChar(); // Skip '='
                return new Token(TokenType.NotEqual, "!=", currentPos);
            }
            else
            {
                return new Token(TokenType.Not, "!", currentPos);
            }
        }

    }

}
