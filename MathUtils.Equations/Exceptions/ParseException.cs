﻿using System;

namespace MathUtils.Exceptions;

public class ParseException : Exception
{
	private readonly string? message;
	private readonly int index;

	//public override string Message => $"Error at char {index}: {message}";

	public ParseException(int _index, string? _message)
		: base(_message)
	{
		index = _index;
		message = _message;
	}
}
