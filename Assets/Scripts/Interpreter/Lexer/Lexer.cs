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

        // Current character position tracker
        private Position CurrentPos => new Position(_line, _col);

        // Keywords to TokenType mapping
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
            {"return", TokenType.Return},
            {"TriggerPlayer", TokenType.TriggerPlayer},
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
            {"SendBottom", TokenType.SendBottom},
            {"Pop", TokenType.Pop},
            {"Remove", TokenType.Remove},
            {"Shuffle", TokenType.Shuffle},
            {"Find", TokenType.Find},
            {"Bool", TokenType.Bool},
            {"false", TokenType.False},
            {"true", TokenType.True},
            {"PostAction", TokenType.PostAction},
            {"Owner", TokenType.Owner},
            {"Selector", TokenType.Selector},
        };

        // Current character being processed
        private char _currentChar => _currentCharIndex >= _text.Length ? '\0' : _text[_currentCharIndex];

        // The token currently being processed
        internal Token CurrentToken { get; set; }

        // Constructor to initialize lexer with source text
        public Lexer(string text)
        {
            _text = text;
            CurrentToken = new Token(TokenType.SOF, "", new Position(0, -1));
        }

        // Advances to the next token in the input text
        public void NextToken()
        {
            SkipWhiteSpaces(); // Skip any whitespace characters

            switch (_currentChar)
            {
                case '\0':
                    CurrentToken = new Token(TokenType.EOF, "", CurrentPos);
                    if (groupTokens.Count != 0)
                    {
                        throw new LexerError($"Unmatched {groupTokens.Peek().Value} on {groupTokens.Peek().Pos}", CurrentPos);
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
                case ',':
                    CurrentToken = new Token(TokenType.Comma, ",", CurrentPos);
                    AdvanceChar();
                    break;
                case '=':
                    CurrentToken = new Token(TokenType.Equals, "=", CurrentPos);
                    AdvanceChar();
                    break;
                case '+':
                    CurrentToken = new Token(TokenType.Plus, "+", CurrentPos);
                    AdvanceChar();
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
                        throw new LexerError($"Unknown character: {_currentChar} at {CurrentPos}", CurrentPos);
                    }
                    break;
            }
        }

        // Processes an identifier or keyword token
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

        // Processes a numeric token
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

        // Processes a string token
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
                return new Token(TokenType.String, sb.ToString(), CurrentPos);
            }
            else
            {
                throw new LexerError("Unterminated string literal", CurrentPos);
            }
        }

        // Skips over any whitespace characters
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

        // Advances to the next character in the input text
        private void AdvanceChar()
        {
            _currentCharIndex++;
            _col++;
        }

        // Advances to the next line in the input text
        private void AdvanceLine()
        {
            _line++;
            _col = 0;
        }

        // Checks for matching parentheses, brackets, and braces
        private void CheckBalance(Token currentToken)
        {
            Dictionary<TokenType, TokenType> couples = new()
            {
                {TokenType.CloseParen, TokenType.OpenParen},
                {TokenType.CloseBracket, TokenType.OpenBracket},
                {TokenType.CloseBracket, TokenType.OpenBracket}
            };

            if (couples.ContainsValue(currentToken.Type))
            {
                groupTokens.Push(new Token(currentToken));
            }
            else
            {
                if (groupTokens.Count == 0 || couples[currentToken.Type] != groupTokens.Peek().Type)
                {
                    throw new LexerError($"Unmatched {currentToken.Value} at {currentToken.Pos}", CurrentPos);
                }
                groupTokens.Pop();
            }
        }

        // Processes tokens for '<' and '<=' operators
        private Token WithLessToken()
        {
            AdvanceChar(); // Skip '<'
            if (_currentChar == '=')
            {
                AdvanceChar(); // Skip '='
                return new Token(TokenType.LessOrEqual, "<=", CurrentPos);
            }
            else
            {
                return new Token(TokenType.Less, "<", CurrentPos);
            }
        }

        // Processes tokens for '>' and '>=' operators
        private Token WithGreaterToken()
        {
            AdvanceChar(); // Skip '>'
            if (_currentChar == '=')
            {
                AdvanceChar(); // Skip '='
                return new Token(TokenType.GreaterOrEqual, ">=", CurrentPos);
            }
            else
            {
                return new Token(TokenType.Greater, ">", CurrentPos);
            }
        }

        // Processes tokens for '!' and '!=' operators
        private Token WithExclamationToken()
        {
            AdvanceChar(); // Skip '!'
            if (_currentChar == '=')
            {
                AdvanceChar(); // Skip '='
                return new Token(TokenType.NotEqual, "!=", CurrentPos);
            }
            else
            {
                return new Token(TokenType.Exclamation, "!", CurrentPos);
            }
        }

        // Checks if a character is a digit
        private bool IsDigit(char c)
        {
            return char.IsDigit(c);
        }

        // Checks if a character is a letter
        private bool IsLetter(char c)
        {
            return char.IsLetter(c);
        }

        // Checks if a character is alphanumeric (letter or digit)
        private bool IsAlphaNumeric(char c)
        {
            return IsLetter(c) || IsDigit(c);
        }
    }
}
