using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace BitcoderCZ.Utils;

public static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowArgumentException(string message, string paramName)
        => throw new ArgumentException(message, paramName);

    [DoesNotReturn]
    public static void ThrowArgumentException(string message)
        => throw new ArgumentException(message);

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException()
        => throw new ArgumentOutOfRangeException();

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException(string paramName)
        => throw new ArgumentOutOfRangeException(paramName);

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException(string paramName, string message)
        => throw new ArgumentOutOfRangeException(paramName, message);

    [DoesNotReturn]
    public static void ThrowArgumentNullException(string paramName)
        => throw new ArgumentNullException(paramName);

    [DoesNotReturn]
    public static void ThrowArgumentNullException(string paramName, string message)
        => throw new ArgumentNullException(paramName, message);

    [DoesNotReturn]
    public static void ThrowNotImplementedException()
        => throw new NotImplementedException();

    [DoesNotReturn]
    public static void ThrowNotImplementedException(string message)
        => throw new NotImplementedException(message);

    [DoesNotReturn]
    public static void ThrowInvalidOperationException()
        => throw new InvalidOperationException();

    [DoesNotReturn]
    public static void ThrowInvalidOperationException(string message)
        => throw new InvalidOperationException(message);

    [DoesNotReturn]
    public static void ThrowNotSupportedException()
        => throw new NotSupportedException();

    [DoesNotReturn]
    public static void ThrowInvalidEnumArgumentException(string? argumentName, int invalidValue, Type enumClass)
        => throw new InvalidEnumArgumentException(argumentName, invalidValue, enumClass);

    [DoesNotReturn]
    public static void ThrowInvalidDataException(string message)
        => throw new InvalidDataException(message);

    [DoesNotReturn]
    public static void ThrowKeyNotFound<TKey>(TKey key)
        => throw new KeyNotFoundException($"Key '{key}' wasn't found.");

    [DoesNotReturn]
    public static void ThrowDuplicateKey<TKey>(TKey key)
        => throw new ArgumentException($"Duplicate key '{key}'.", nameof(key));

    [DoesNotReturn]
    public static void ThrowIndexArgumentOutOfRange()
        => throw new IndexOutOfRangeException();

    #region Conditional
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNegative(int value, [CallerArgumentExpression("value")] string paramName = "")
    {
        if (value < 0)
        {
            ThrowArgumentOutOfRangeException(paramName, $"{paramName} cannot be negative.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfLessThan(int value, int other, [CallerArgumentExpression("value")] string paramName = "")
    {
        if (value < other)
        {
            ThrowArgumentOutOfRangeException(paramName, $"{paramName} ({value}) must be greater than or equal {other}.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfGreaterThan(int value, int other, [CallerArgumentExpression("value")] string paramName = "")
    {
        if (value > other)
        {
            ThrowArgumentOutOfRangeException(paramName, $"{paramName} ({value}) must be less than or equal to {other}.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfGreaterThanOrEqualToOrNegative(int value, int other, [CallerArgumentExpression("value")] string paramName = "")
    {
        if ((uint)value >= (uint)other)
        {
            ThrowArgumentOutOfRangeException(paramName, $"{paramName} ({value}) must be less than {other} and non negative.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNotInRangeInclusive(float value, float minInclusive, float maxInclusive, [CallerArgumentExpression("value")] string paramName = "")
    {
        if (value < minInclusive || value > maxInclusive)
        {
            ThrowArgumentOutOfRangeException(paramName, $"{paramName} ({value}) must be between {minInclusive} and {maxInclusive} (inclusive).");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNull<T>([NotNull] T? value, [CallerArgumentExpression("value")] string paramName = "")
    {
        if (value is null)
        {
            ThrowArgumentNullException(paramName, $"{paramName} cannot be null.");
        }
    }
    #endregion
}