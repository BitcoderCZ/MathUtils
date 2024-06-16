using MathUtils.Equations.Parts.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Equations
{
    public class Token
    {
        public readonly TokenType Type;
        public object? Value;

        public Token(TokenType _type, object? _value = null)
        {
            Type = _type;
            Value = _value;
        }
    }

    public enum TokenType
    {
        Number,
        Plus,
        Minus,
        Star,
        Slash,
        Percent,
        Caret,
        OpenParentheses,
        CloseParentheses,
        Comma,
        Block,
        /// <summary>
        /// Variable or function
        /// </summary>
        Indentifier,
        Function,
    }

    public static class TokenTypeE
    {
        public static int OperationPriority(this TokenType type, bool binary)
        {
            switch (type)
            {
                case TokenType.Plus: return binary ? PlusOp.Priority : IdentityOp.Priority << 8;
                case TokenType.Minus: return binary ? MinusOp.Priority : NegateOp.Priority << 8;
                case TokenType.Star: return MultiplyOp.Priority;
                case TokenType.Slash: return DivideOp.Priority;
                case TokenType.Percent: return ModuloOp.Priority;
                case TokenType.Caret: return PowerOp.Priority;
                default: return -1;
            }
        }
    }
}
