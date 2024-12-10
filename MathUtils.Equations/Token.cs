using MathUtils.Equations.Parts.Operations;

namespace MathUtils.Equations;

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
		=> type switch
		{
			TokenType.Plus => binary ? PlusOp.Priority : IdentityOp.Priority << 8,
			TokenType.Minus => binary ? MinusOp.Priority : NegateOp.Priority << 8,
			TokenType.Star => MultiplyOp.Priority,
			TokenType.Slash => DivideOp.Priority,
			TokenType.Percent => ModuloOp.Priority,
			TokenType.Caret => PowerOp.Priority,
			_ => -1,
		};
}
