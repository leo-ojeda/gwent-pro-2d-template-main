using System;
using System.Collections.Generic;
using DSL.Lexer;

using System.Text;


namespace DSL.Parser
{
    public class Parser
    {
        private List<Token> tokens;
        private int position;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            this.position = 0;
        }

        public Card ParseCard()
        {
            Consume(TokenType.Card);  // Debe empezar con "card"
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
                    Consume(TokenType.Type);
                    Consume(TokenType.Colon);
                    type = Consume(TokenType.String).Value;
                }
                else if (Match(TokenType.Name))
                {
                    Consume(TokenType.Name);
                    Consume(TokenType.Colon);
                    name = Consume(TokenType.String).Value;
                }
                else if (Match(TokenType.Faction))
                {
                    Consume(TokenType.Faction);
                    Consume(TokenType.Colon);
                    faction = Consume(TokenType.String).Value;
                }
                else if (Match(TokenType.Power))
                {
                    Consume(TokenType.Power);
                    Consume(TokenType.Colon);
                    power = int.Parse(Consume(TokenType.Number).Value);
                }
                else if (Match(TokenType.Range))
                {
                    Consume(TokenType.Range);
                    Consume(TokenType.Colon);
                    Consume(TokenType.OpenBracket);
                    while (!Match(TokenType.CloseBracket))
                    {
                        range.Add(Consume(TokenType.String).Value);
                        if (Match(TokenType.Comma))
                        {
                            Consume(TokenType.Comma);
                        }
                    }
                    Consume(TokenType.CloseBracket);
                }
                else if (Match(TokenType.OnActivation))
                {
                    Consume(TokenType.OnActivation);
                    Consume(TokenType.Colon);
                    Consume(TokenType.OpenBracket);
                    while (!Match(TokenType.CloseBracket))
                    {
                        var effectActivation = ParseEffectActivation();
                        onActivation.Add(effectActivation);
                        if (Match(TokenType.Comma))
                        {
                            Consume(TokenType.Comma);
                        }
                    }
                    Consume(TokenType.CloseBracket);
                }
                else
                {
                    throw new Exception($"Unexpected token: {LookAhead()}");
                }
            }

            Consume(TokenType.CloseBrace);
            return new Card(name, power, type, range.ToArray(), faction, onActivation, null);
        }

        private EffectActivation ParseEffectActivation()
        {
            Consume(TokenType.OpenBrace);

            Effect effect = null;
            Selector selector = null;
            PostAction postAction = null;

            while (!Match(TokenType.CloseBrace))
            {
                if (Match(TokenType.Effect))
                {
                    effect = ParseEffect();
                }
                else if (Match(TokenType.Selector))
                {
                    selector = ParseSelector();
                }
                else if (Match(TokenType.PostAction))
                {
                    postAction = ParsePostAction();
                }
                else
                {
                    throw new Exception($"Unexpected token: {LookAhead()}");
                }
            }

            Consume(TokenType.CloseBrace);
            return new EffectActivation(effect, selector);
        }

        private Effect ParseEffect()
        {
            Consume(TokenType.Effect);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBrace);

            string name = null;
            List<Parameter> parameters = new List<Parameter>();

            while (!Match(TokenType.CloseBrace))
            {
                if (Match(TokenType.Name))
                {
                    Consume(TokenType.Name);
                    Consume(TokenType.Colon);
                    name = Consume(TokenType.String).Value;
                }
                else if (Match(TokenType.Amount))
                {
                    Consume(TokenType.Amount);
                    Consume(TokenType.Colon);
                    int amount = int.Parse(Consume(TokenType.Number).Value);
                    parameters.Add(new Parameter("Amount", ParamType.Number, amount));
                }
                else
                {
                    throw new Exception($"Unexpected token: {LookAhead()}");
                }
            }

            Consume(TokenType.CloseBrace);

            return new Effect(name, parameters, null);
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
                if (Match(TokenType.Source))
                {
                    Consume(TokenType.Source);
                    Consume(TokenType.Colon);
                    source = Consume(TokenType.String).Value;
                }
                else if (Match(TokenType.Single))
                {
                    Consume(TokenType.Single);
                    Consume(TokenType.Colon);
                    single = bool.Parse(Consume(TokenType.String).Value);
                }
                else if (Match(TokenType.Predicate))
                {
                    Consume(TokenType.Predicate);
                    Consume(TokenType.Colon);
                    predicate = ParsePredicate();
                }
                else
                {
                    throw new Exception($"Unexpected token: {LookAhead()}");
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

            while (!Match(TokenType.CloseBrace))
            {
                if (Match(TokenType.Type))
                {
                    Consume(TokenType.Type);
                    Consume(TokenType.Colon);
                    type = Consume(TokenType.String).Value;
                }
                else if (Match(TokenType.Selector))
                {
                    selector = ParseSelector();
                }
                else
                {
                    throw new Exception($"Unexpected token: {LookAhead()}");
                }
            }

            Consume(TokenType.CloseBrace);
            return new PostAction(type, selector);
        }

        private string ParsePredicate()
        {
            // Implementación básica de análisis de predicado.
            StringBuilder predicate = new StringBuilder();
            Consume(TokenType.OpenParen);
            while (!Match(TokenType.CloseParen))
            {
                predicate.Append(Consume(TokenType.Identifier).Value);
            }
            Consume(TokenType.CloseParen);
            return predicate.ToString();
        }

        // Métodos auxiliares de consumo y comprobación de tokens.

        private Token Consume(TokenType type)
        {
            if (LookAhead().Type != type)
            {
                throw new Exception($"Expected {type}, but found {LookAhead().Type}");
            }
            return tokens[position++];
        }

        private bool Match(TokenType type)
        {
            return LookAhead().Type == type;
        }

        private Token LookAhead()
        {
            return position < tokens.Count ? tokens[position] : new Token(TokenType.EndOfInput, string.Empty,new Position(0, 0));
        }
    }
}