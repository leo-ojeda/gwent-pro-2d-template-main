using System;

namespace DSL.Lexer
{
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public Position Pos { get; }

        public Token(TokenType type, string value, Position pos)
        {
            Type = type;
            Value = value;
            Pos = pos;
        }

        // Constructor de copia para clonar un token existente
        public Token(Token token)
        {
            Type = token.Type;
            Value = token.Value;
            Pos = new Position(token.Pos);  // Usar copia profunda para seguridad
        }

        public override string ToString()
        {
            return $"Token(Type: {Type}, Value: '{Value}', Position: {Pos})";
        }
    }

    public class Position
    {
        public int Line { get; private set; }
        public int Column { get; private set; }

        public Position(int line, int column)
        {
            Line = line;
            Column = column;
        }

        // Constructor de copia
        public Position(Position pos)
        {
            Line = pos.Line;
            Column = pos.Column;
        }

        // Método para avanzar la columna
        public void AdvanceColumn()
        {
            Column++;
        }

        // Método para avanzar la línea y reiniciar la columna
        public void AdvanceLine()
        {
            Line++;
            Column = 1;
        }

        public override string ToString()
        {
            return $"(Line: {Line}, Column: {Column})";
        }

        public override bool Equals(object obj)
        {
            if (obj is Position position)
            {
                return Line == position.Line && Column == position.Column;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Line, Column);
        }
    }
}
