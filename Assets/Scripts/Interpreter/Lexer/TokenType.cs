namespace DSL.Lexer
{
    public enum TokenType
    {
        // Tokens especiales
        SOF, // Start of File
        EOF, // End of File

        // Delimitadores
        OpenCurlyBracket,     // {
        ClosedCurlyBracket,   // }
        OpenSquareBracket,    // [
        ClosedSquareBracket,  // ]
        OpenParenthesis,      // (
        ClosedParenthesis,    // )
        Colon,                // :
        Comma,                // ,
        Equals,               // =
        Plus,                 // +
        Minus,                // -
        Multiply,             // *
        Slash,                // /
        Modulus,              // %
        Less,                 // <
        Greater,              // >
        LessOrEqual,          // <=
        GreaterOrEqual,       // >=
        Not,                  // !
        NotEqual,             // !=

        // Tipos de datos
        String,
        Number,

        // Identificadores
        Identifier,

        // Palabras clave del DSL
        Card,
        Effect,
        Type,
        Name,
        Faction,
        Power,
        Range,
        OnActivation,
        Params,
        Action,
        Amount,
        Source,
        Single,
        Predicate,
        NumberType,
        For,
        In,
        While,
        Return,

        // Agregar otros tipos de tokens según se necesiten
    }
}
