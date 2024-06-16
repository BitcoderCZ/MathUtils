using MathUtils.Equations.Parts;
using MathUtils.Equations.Parts.Functions;
using MathUtils.Equations.Parts.Operations;
using MathUtils.Exceptions;
using System.Globalization;
using System.Text;

namespace MathUtils.Equations
{
    public class Equation
    {
        private IPart root;

        private Equation(IPart _root)
        {
            root = _root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ExecutionException"></exception>
        public double Calculate()
            => Calculate(CalculationContext.Default);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ExecutionException"></exception>
        public double Calculate(CalculationContext context)
        {
            return root.GetValue(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="numberFormat"></param>
        /// <returns></returns>
        /// <exception cref="ParseException"></exception>
        public static IList<Token> Tokenize(string equation, NumberFormatInfo? numberFormat = null)
        {
            List<Token> tokens = new();
            StringBuilder builder = new StringBuilder();

            numberFormat ??= NumberFormatInfo.InvariantInfo;
            char decimalSeparator = numberFormat.NumberDecimalSeparator[0];
            //char groupSeparator = numberFormat.NumberGroupSeparator[0];

            for (int i = 0; i < equation.Length; i++)
            {
                char c = equation[i];

                switch (c)
                {
                    case '+':
                        tokens.Add(new Token(TokenType.Plus));
                        break;
                    case '-':
                        tokens.Add(new Token(TokenType.Minus));
                        break;
                    case '*':
                        tokens.Add(new Token(TokenType.Star));
                        break;
                    case '/':
                        tokens.Add(new Token(TokenType.Slash));
                        break;
                    case '%':
                        tokens.Add(new Token(TokenType.Percent));
                        break;
                    case '^':
                        tokens.Add(new Token(TokenType.Caret));
                        break;
                    case '(':
                        tokens.Add(new Token(TokenType.OpenParentheses));
                        break;
                    case ')':
                        tokens.Add(new Token(TokenType.CloseParentheses));
                        break;
                    case ',':
                        tokens.Add(new Token(TokenType.Comma));
                        break;
                    default:
                        {
                            if (char.IsControl(c) || char.IsWhiteSpace(c)) continue;
                            else if (char.IsDigit(c) || c == decimalSeparator) tokens.Add(new Token(TokenType.Number, readNumber()));
                            else if (char.IsLetter(c)) tokens.Add(new Token(TokenType.Indentifier, readIdentifier()));
                            else throw new ParseException(i, $"Unknown character '{c}'");
                        }
                        break;
                }

                double readNumber()
                {
                    bool foundDecimal = false;

                    char currentChar;
                    while (i < equation.Length && (char.IsDigit(currentChar = equation[i]) || currentChar == decimalSeparator/* || currentChar == groupSeparator*/))
                    {
                        if (foundDecimal && currentChar == decimalSeparator)
                            throw new ParseException(currentChar, "Multiple decimal separators in one number");
                        if (currentChar == decimalSeparator)
                            foundDecimal = true;

                        //if (currentChar != groupSeparator)
                        builder.Append(currentChar);
                        i++;
                    }

                    i--; // will be increased by the for loop

                    if (!double.TryParse(builder.ToString(), numberFormat, out double numb))
                        throw new ParseException(i, "Invalid number");
                    builder.Clear();
                    return numb;
                }

                string readIdentifier()
                {
                    char currentChar;
                    while (i < equation.Length && char.IsLetterOrDigit(currentChar = equation[i]))
                    {
                        builder.Append(currentChar);
                        i++;
                    }

                    i--; // will be increased by the for loop

                    string str = builder.ToString();
                    builder.Clear();
                    return str;
                }
            }

            return tokens;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="numberFormat"></param>
        /// <returns></returns>
        /// <exception cref="ParseException"></exception>
        public static Equation Parse(string equation, NumberFormatInfo? numberFormat = null, ParseContext? context = null)
            => Parse(Tokenize(equation, numberFormat), context);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        /// <exception cref="ParseException"></exception>
        public static Equation Parse(IList<Token> tokens, ParseContext? context)
        {
            Dictionary<int, ParseOperation> tokenToOp = new();
            Dictionary<int, ParseOperation> operations = new();

            context ??= ParseContext.Default;

            if (tokens.Count == 0)
                throw new ParseException(-1, "Tokens can't be empty");

            processFunctions(tokens, context);
            processBlocks(tokens, context);
            addMultiplications(tokens);

            for (int i = 0; i < tokens.Count; i++)
            {
                Token current = tokens[i];

                switch (current.Type)
                {
                    case TokenType.Number:
                    case TokenType.Indentifier:
                    case TokenType.Function:
                    case TokenType.Block:
                        break;
                    default:
                        processOperation();
                        break;
                }

                void processOperation()
                {
                    bool binary;
                    switch (current.Type)
                    {
                        case TokenType.Plus:
                        case TokenType.Minus:
                            if (i == 0)
                                binary = false;
                            else
                            {
                                switch (tokens[i - 1].Type)
                                {
                                    case TokenType.Number:
                                    case TokenType.Indentifier:
                                    case TokenType.Function:
                                    case TokenType.Block:
                                        binary = true;
                                        break;
                                    default:
                                        binary = false;
                                        break;
                                }
                            }
                            break;
                        default:
                            binary = true;
                            break;
                    }

                    List<int> usedTokens = new(2);
                    if (binary)
                        usedTokens.Add(i - 1);

                    usedTokens.Add(i + 1);

                    ParseOperation op = new ParseOperation(current.Type, i, binary, usedTokens);
                    operations.Add(i, op);

                    for (int j = 0; j < usedTokens.Count; j++)
                        claimToken(op, j);
                }
            }

            void claimToken(ParseOperation op, int usedIndex)
            {
                int index = op.UsedTokens[usedIndex];

                if (tokenToOp.TryGetValue(index, out ParseOperation? otherOp))
                {
                    if (op.Priority > otherOp.Priority)
                    {
                        int a = otherOp.UsedTokens.IndexOf(index);
                        otherOp.UsedTokens[a] = op.Index; // "redirect" other op to use op
                        tokenToOp[index] = op;
                        tokenToOp.Add(op.Index, otherOp);
                    }
                    else
                    {
                        op.UsedTokens[usedIndex] = otherOp.Index; // "redirect" op to use other op
                        int a = otherOp.UsedTokens.IndexOf(index);
                        tokenToOp[a] = otherOp;
                        tokenToOp[otherOp.Index] = op;
                    }
                }
                else
                    tokenToOp.Add(index, op);
            }

            if (operations.Count == 0)
                return new Equation(createPart(0));
            else
                foreach (var item in operations)
                    if (!tokenToOp.ContainsKey(item.Value.Index))
                        return new Equation(createPart(item.Value.Index));

            throw new ParseException(-1, "Failed to find root operation");

            IPart createPart(int index)
            {
                Token token = tokens[index];

                switch (token.Type)
                {
                    case TokenType.Number:
                        return new NumberPart((double)token.Value!);
                    case TokenType.Block:
                        return new BlockPart((IPart)token.Value!);
                    case TokenType.Indentifier:
                        return new VariablePart((string)token.Value!);
                    case TokenType.Function:
                        return (FunctionPart)token.Value!;
                    default:
                        {
                            ParseOperation op = operations[index];

                            switch (token.Type)
                            {
                                case TokenType.Plus:
                                    {
                                        if (op.Binary)
                                            return new PlusOp(createPart(op.UsedTokens[0]), createPart(op.UsedTokens[1]));
                                        else
                                            return new IdentityOp(createPart(op.UsedTokens[0]));
                                    }
                                case TokenType.Minus:
                                    {
                                        if (op.Binary)
                                            return new MinusOp(createPart(op.UsedTokens[0]), createPart(op.UsedTokens[1]));
                                        else
                                            return new NegateOp(createPart(op.UsedTokens[0]));
                                    }
                                case TokenType.Star:
                                    return new MultiplyOp(createPart(op.UsedTokens[0]), createPart(op.UsedTokens[1]));
                                case TokenType.Slash:
                                    return new DivideOp(createPart(op.UsedTokens[0]), createPart(op.UsedTokens[1]));
                                case TokenType.Percent:
                                    return new ModuloOp(createPart(op.UsedTokens[0]), createPart(op.UsedTokens[1]));
                                case TokenType.Caret:
                                    return new PowerOp(createPart(op.UsedTokens[0]), createPart(op.UsedTokens[1]));
                                default:
                                    throw new ParseException(index, "Unknown operation type");
                            }
                        }
                }
            }
        }
        private static void addMultiplications(IList<Token> tokens)
        {
            if (tokens.Count < 2) return;

            for (int i = 0; i < tokens.Count - 1; i++)
            {
                TokenType a = tokens[i].Type;
                TokenType b = tokens[i + 1].Type;

                if ((a is TokenType.Number or TokenType.Indentifier or TokenType.Function or TokenType.Block)
                    && (b is TokenType.Number or TokenType.Indentifier or TokenType.Function or TokenType.Block))
                {
                    tokens.Insert(i + 1, new Token(TokenType.Star));
                    i++;
                }
            }
        }
        private static void processFunctions(IList<Token> tokens, ParseContext context)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];

                switch (token.Type)
                {
                    case TokenType.Indentifier:
                        if (i < tokens.Count - 1 && tokens[i + 1].Type == TokenType.OpenParentheses)
                            readFunction(i);
                        break;
                }
            }

            void readFunction(int start)
            {
                string name = (string)tokens[start].Value!;

                List<IPart> args = new();
                List<Token> argTokens = new();
                int argStart = start + 2;
                int depth = 1;
                for (int i = start + 2; i < tokens.Count; i++)
                {
                    Token token = tokens[i];

                    switch (token.Type)
                    {
                        case TokenType.OpenParentheses:
                            depth++;
                            break;
                        case TokenType.CloseParentheses:
                            depth--;
                            break;
                    }

                    if (depth == 0 || (depth == 1 && token.Type == TokenType.Comma))
                    {
                        if (argTokens.Count == 0)
                            throw new ParseException(i, "Argument can't be empty");

                        args.Add(Parse(argTokens, context).root);
                        argTokens.Clear();

                        int count = i - argStart + 1;
                        for (int j = 0; j < count; j++)
                            tokens.RemoveAt(argStart);

                        i = argStart - 1; // i will be increased by the for loop

                        continue;
                    }

                    argTokens.Add(token);
                }

                if (depth > 0)
                    throw new ParseException(start, "Unclosed function parameter list");

                for (int i = 0; i < 2; i++)
                    tokens.RemoveAt(start);

                tokens.Insert(start, new Token(TokenType.Function, context.GetFunction(name, args.Count).CreatePart(args)));
            }
        }
        private static void processBlocks(IList<Token> tokens, ParseContext context)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];

                switch (token.Type)
                {
                    case TokenType.OpenParentheses:
                        readBlock(i);
                        break;
                    case TokenType.CloseParentheses:
                        throw new ParseException(i, "Close parentheses without Open parentheses");
                }
            }

            void readBlock(int start)
            {
                List<Token> blockTokens = new();

                int end;
                int depth = 1;
                for (end = start + 1; end < tokens.Count; end++)
                {
                    Token token = tokens[end];
                    switch (token.Type)
                    {
                        case TokenType.OpenParentheses:
                            depth++;
                            break;
                        case TokenType.CloseParentheses:
                            depth--;
                            break;
                    }

                    if (depth == 0)
                        break;
                    else
                        blockTokens.Add(token);
                }

                if (depth > 0)
                    throw new ParseException(start, "Unclosed code block");
                else if (blockTokens.Count == 0)
                    throw new ParseException(start, "Block can't be empty");

                int count = end - start + 1;
                for (int i = 0; i < count; i++)
                    tokens.RemoveAt(start);

                tokens.Insert(start, new Token(TokenType.Block, Parse(blockTokens, context).root));
            }
        }

        public override string ToString()
            => ToString(StringOptions.Default);
        public string ToString(StringOptions options)
        {
            return root.ToString(options);
        }

        class ParseOperation
        {
            public readonly TokenType OpToken;
            public readonly int Index;
            public readonly bool Binary;
            public readonly int Priority;
            public readonly List<int> UsedTokens;

            public ParseOperation(TokenType _opToken, int _index, bool _binary, List<int> _usedTokens)
            {
                OpToken = _opToken;
                Index = _index;
                Binary = _binary;
                Priority = OpToken.OperationPriority(Binary);
                UsedTokens = _usedTokens;
            }
        }

        public class StringOptions
        {
            public static StringOptions Default => new StringOptions(1);

            public readonly int NumbSpaces;
            public readonly string Separator;
            public readonly NumberFormatInfo NumberFormat;

            public StringOptions(int _numbSpaces, NumberFormatInfo? _numberFormat = null)
            {
                NumbSpaces = _numbSpaces;
                Separator = new string(' ', NumbSpaces);
                NumberFormat = _numberFormat ?? NumberFormatInfo.InvariantInfo;
            }
        }
    }
}
