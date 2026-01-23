using BitcoderCZ.Buffers.Generators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace BitcoderCZ.Buffers.Generators;

[Generator]
public class FixedArrayGenerator : IIncrementalGenerator
{
	private static readonly ImmutableArray<int> Lengths = [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 32, 64, 128, 256, 512];

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Use expression body for method", Justification = "No")]
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(ctx =>
		{
			foreach (int length in Lengths)
			{
				string result = GenerateFixedArray(length);
				ctx.AddSource($"FixedArray{length}.g.cs", SourceText.From(result, Encoding.UTF8));
			}
		});
	}

	private static string GenerateFixedArray(int length)
	{
		// TODO: add xml documentation
		IndentedStringBuilder builder = new IndentedStringBuilder(new ValueStringBuilder(1024 * 4));
		
		builder.AppendLine(/* language=c# */ $$"""
			using System;
			using System.Collections;
			using System.Collections.Generic;
			using System.Diagnostics.CodeAnalysis;
			using System.Runtime.CompilerServices;
			using BitcoderCZ.Utils;

			#nullable enable
			namespace BitcoderCZ.Buffers;
			
			[CollectionBuilder(
				typeof(FixedArray{{length}}),
				nameof(FixedArray{{length}}.Create))]
			#if NET8_0_OR_GREATER
			[InlineArray({{length}})]
			#endif
			public struct FixedArray{{length}}<T> : IFixedArray<T>
			{
				private const int LengthConst = {{length}};

			#if NET6_0_OR_GREATER
				static int IFixedArray<T>.Length => LengthConst;
			#else
				readonly int IFixedArray<T>.Length => LengthConst;
			#endif

			#if NET8_0_OR_GREATER
				private T _value0;
			#else
			#pragma warning disable IDE0044 // Add readonly modifier
			#pragma warning disable CS0169
			""");

		builder.Indent++;
		for (int i = 0; i < length; i++)
		{
			builder.AppendLine($"private T _value{i};");
		}

		builder.Indent--;

		builder.AppendLine($$"""
			#pragma warning restore CS0169
			#pragma warning restore IDE0044 // Add readonly modifier
			#endif

				[UnscopedRef]
				public ref T Element0
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => ref _value0;
				}

				[UnscopedRef]
			#pragma warning disable CS9181 // Inline array indexer will not be used for element access expression. - required for IFixedArray<T>
				public ref T this[int index]
			#pragma warning restore CS9181 // Inline array indexer will not be used for element access expression.
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get
					{
						ThrowHelper.ThrowIfGreaterThanOrEqualToOrNegative(index, LengthConst);

						return ref Unsafe.Add(ref _value0, index);
					}
				}

				public readonly Span<T> AsSpan()
					=> System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in _value0), LengthConst);

				public readonly ReadOnlySpan<T> AsROSpan()
					=> System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _value0), LengthConst);

				public void Swap(int index1, int index2)
				{
					ref T ref1 = ref this[index1];
					ref T ref2 = ref this[index2];

					T temp = ref1;
					ref1 = ref2;
					ref2 = temp;
				}

			#if NET7_0_OR_GREATER
				[UnscopedRef]
				public readonly RefEnumerator GetEnumerator()
					=> new RefEnumerator(in this);
			#else
				public readonly Enumerator GetEnumerator()
					=> new Enumerator(this);
			#endif

				readonly IEnumerator<T> IEnumerable<T>.GetEnumerator()
					=> new Enumerator(this);

				readonly IEnumerator IEnumerable.GetEnumerator()
					=> new Enumerator(this);

				public struct Enumerator : IEnumerator<T>
				{
					private FixedArray{{length}}<T> _array;
					private int _index;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					internal Enumerator(FixedArray{{length}}<T> array)
					{
						_array = array;
						_index = -1;
					}

					[UnscopedRef]
					public ref readonly T Current
					{
						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						get => ref _array[_index];
					}

					T IEnumerator<T>.Current => Current;

					object IEnumerator.Current => Current!;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public bool MoveNext()
					{
						int next = _index + 1;
						if (next < LengthConst)
						{
							_index = next;
							return true;
						}

						return false;
					}

					public void Reset()
						=> _index = -1;

					public readonly void Dispose()
					{
					}
				}

			#if NET7_0_OR_GREATER
				public ref struct RefEnumerator : IEnumerator<T>
				{
					private ref readonly FixedArray{{length}}<T> _array;
					private int _index;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					internal RefEnumerator(ref readonly FixedArray{{length}}<T> array)
					{
						_array = ref array;
						_index = -1;
					}

					[UnscopedRef]
					public ref readonly T Current
					{
						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						get => ref _array[_index];
					}

					T IEnumerator<T>.Current => Current;

					object IEnumerator.Current => Current!;

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					public bool MoveNext()
					{
						int next = _index + 1;
						if (next < LengthConst)
						{
							_index = next;
							return true;
						}

						return false;
					}

					public void Reset()
						=> _index = -1;

					public readonly void Dispose()
					{
					}
				}
			#endif
			}

			public static class FixedArray{{length}}
			{
				[System.Runtime.CompilerServices.SkipLocalsInit]
				public static FixedArray{{length}}<T> Create<T>(ReadOnlySpan<T> items)
				{
					if (items.Length is not {{length}})
					{
						ThrowHelper.ThrowArgumentException($"{nameof(items)} must contain exactly {{length}} elements.", nameof(items));
					}

					System.Runtime.CompilerServices.Unsafe.SkipInit<FixedArray{{length}}<T>>(out var result);
					items.CopyTo(result.AsSpan());
					return result;
				}

				[System.Runtime.CompilerServices.SkipLocalsInit]
				public static void Create<T>(ReadOnlySpan<T> items, out FixedArray{{length}}<T> result)
				{
					if (items.Length is not {{length}})
					{
						ThrowHelper.ThrowArgumentException($"{nameof(items)} must contain exactly {{length}} elements.", nameof(items));
					}

					System.Runtime.CompilerServices.Unsafe.SkipInit(out result);
					items.CopyTo(result.AsSpan());
				}
			}
			""");

		return builder.ToString(); // also disposes
	}
}
