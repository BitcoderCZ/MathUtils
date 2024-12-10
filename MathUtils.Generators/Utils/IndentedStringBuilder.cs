using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace MathUtils.Generators.Utils;

internal ref struct IndentedStringBuilder : IDisposable
{
	private readonly ValueStringBuilder _builder;
	private readonly string _tabString;
	private int _indentLevel;
	private bool _tabsPending;

	public const string DefaultTabString = "    ";

	public IndentedStringBuilder(ValueStringBuilder builder)
		: this(builder, DefaultTabString)
	{
	}

	public IndentedStringBuilder(ValueStringBuilder builder, string tabString)
	{
		_builder = builder;
		_tabString = tabString ?? DefaultTabString;
		_tabsPending = true;
	}

	public readonly string NewLine => "\n";

	public int Indent
	{
		readonly get => _indentLevel;
		set => _indentLevel = Math.Max(value, 0);
	}

	private void OutputTabs()
	{
		if (_tabsPending)
		{
			for (int i = 0; i < _indentLevel; i++)
			{
				_builder.Append(_tabString);
			}

			_tabsPending = false;
		}
	}

	public void Append(char value)
	{
		OutputTabs();
		_builder.Append(value);
	}

	public void Append(char value, int count)
	{
		OutputTabs();
		_builder.Append(value, count);
	}

	public void Append(string value)
	{
		Debug.Assert(!value.Contains("\r\n"));

		OutputTabs();

		if (value.IndexOf('\n') == -1)
		{
			_builder.Append(value);
			return;
		}

		int index = -1;

		while (true)
		{
			int nextIndex = value.IndexOf('\n', index);

			if (nextIndex == -1)
			{
				nextIndex = value.Length;

				OutputTabs();
				_builder.Append(value.AsSpan(index, nextIndex - index));
				return;
			}

			OutputTabs();
			_builder.Append(value.AsSpan(index, nextIndex - index + 1));
			_tabsPending = true;

			index = nextIndex + 1;

			if (index >= value.Length)
			{
				return;
			}
		}
	}

	public void AppendLine(string value)
	{
		Append(value);
		Append('\n');
		_tabsPending = true;
	}

	public readonly override string ToString()
		=> _builder.ToString();

	public readonly void Dispose()
		=> _builder.Dispose();
}
