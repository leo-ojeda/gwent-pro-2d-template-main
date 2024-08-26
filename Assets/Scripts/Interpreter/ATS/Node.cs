
using System.Collections.Generic;
using DSL.Lexer;

namespace DSL.ATS
{
    // Nodo base para el AST
    public abstract class ASTNode
    {
        public Token Token { get; set; }

        public ASTNode(Token token)
        {
            Token = token;
        }
    }

    // Nodo para representar una carta
    public class CardNode : ASTNode
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Faction { get; set; }
        public int Power { get; set; }
        public List<string> Range { get; set; }
        public List<EffectNode> OnActivation { get; set; }

        public CardNode(Token token) : base(token)
        {
            Range = new List<string>();
            OnActivation = new List<EffectNode>();
        }
    }

    // Nodo para representar un efecto
    public class EffectNode : ASTNode
    {
        public string EffectName { get; set; }
        public Dictionary<string, object> Params { get; set; }

        public EffectNode(Token token) : base(token)
        {
            Params = new Dictionary<string, object>();
        }
    }

    // Nodo para un identificador genérico
    public class IdentifierNode : ASTNode
    {
        public string Value { get; set; }

        public IdentifierNode(Token token, string value) : base(token)
        {
            Value = value;
        }
    }
}
