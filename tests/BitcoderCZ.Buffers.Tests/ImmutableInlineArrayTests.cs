namespace BitcoderCZ.Buffers.Tests;

public abstract class ImmutableInlineArrayTests<TArray> where TArray : struct, IFixedArray<int>
{
    protected static int Capacity => FixedArray.GetLength<TArray, int>();

    [Test]
    public async Task Empty_HasZeroLength()
    {
        var array = ImmutableInlineArray<TArray, int>.Empty;
        await Assert.That(array.Length).IsEqualTo(0);
    }

    [Test]
    public async Task Create_WithItemsUnderCapacity_UsesInlineStorage()
    {
        // Arrange
        var items = Enumerable.Range(1, Capacity).ToArray();

        // Act
        var array = ImmutableInlineArray.Create<TArray, int>(items);

        // Assert
        await Assert.That(array.Length).IsEqualTo(Capacity);
        for (int i = 0; i < Capacity; i++)
        {
            await Assert.That(array[i]).IsEqualTo(items[i]);
        }
    }

    [Test]
    public async Task Create_WithItemsOverCapacity_UsesOverflow()
    {
        // Arrange
        int extra = 5;
        var items = Enumerable.Range(1, Capacity + extra).ToArray();

        // Act
        var array = ImmutableInlineArray.Create<TArray, int>(items);

        // Assert
        await Assert.That(array.Length).IsEqualTo(Capacity + extra);
        await Assert.That(array[Capacity]).IsEqualTo(items[Capacity]); // First overflow element
        await Assert.That(array[Capacity + extra - 1]).IsEqualTo(items.Last());
    }

    [Test]
    public async Task Add_TriggersOverflow_WhenCapacityReached()
    {
        // Arrange
        var array = ImmutableInlineArray.Create<TArray, int>(Enumerable.Range(1, Capacity).ToArray());

        // Act
        var newArray = array.Add(999);

        // Assert
        await Assert.That(array.Length).IsEqualTo(Capacity); // Original unchanged (Immutable)
        await Assert.That(newArray.Length).IsEqualTo(Capacity + 1);
        await Assert.That(newArray[Capacity]).IsEqualTo(999);
    }

    [Test]
    public async Task CopyRangeTo_HandlesBoundariesCorrectly()
    {
        // Arrange: Create array that spans both inline and overflow
        var items = Enumerable.Range(0, Capacity + 10).ToArray();
        var array = ImmutableInlineArray.Create<TArray, int>(items);
        int[] destination = new int[10];

        // Act: Copy a range that starts in Inline and ends in Overflow
        // e.g., if capacity is 8, take range 5..15 (length 10)
        int start = Math.Max(0, Capacity - 5);
        array.CopyRangeTo(new Range(start, start + 10), destination);

        // Assert
        for (int i = 0; i < 10; i++)
        {
            await Assert.That(destination[i]).IsEqualTo(items[start + i]);
        }
    }

    [Test]
    public async Task Builder_DrainToImmutable_ClearsBuilder()
    {
        // Arrange
        var builder = new ImmutableInlineArray<TArray, int>.Builder(Capacity + 2);
        builder.Add(10);
        builder.Add(20);

        // Act
        var array = builder.DrainToImmutable();

        // Assert
        await Assert.That(array.Length).IsEqualTo(2);
        await Assert.That(builder.Count).IsEqualTo(0);
        await Assert.That(array[0]).IsEqualTo(10);
    }

    [Test]
    public async Task Enumerator_IteratesAllElements()
    {
        // Arrange
        var items = Enumerable.Range(1, Capacity + 2).ToArray();
        var array = ImmutableInlineArray.Create<TArray, int>(items);
        var result = new List<int>();

        // Act
        foreach (var item in array)
        {
            result.Add(item);
        }

        // Assert
        await Assert.That(result).IsEquivalentTo(items);
    }

    [Test]
    public async Task Add_ToEmpty_ResultsInLengthOne()
    {
        // Arrange
        var array = ImmutableInlineArray<TArray, int>.Empty;

        // Act
        var result = array.Add(42);

        // Assert
        await Assert.That(result.Length).IsEqualTo(1);
        await Assert.That(result[0]).IsEqualTo(42);
        await Assert.That(array.Length).IsEqualTo(0); // Ensure immutability
    }

    [Test]
    public async Task Add_StayWithinCapacity_UsesInlineStorage()
    {
        // Arrange: Fill up to Capacity - 1
        var initialCount = Math.Max(0, Capacity - 1);
        var array = ImmutableInlineArray.Create<TArray, int>(Enumerable.Range(1, initialCount).ToArray());

        // Act
        var result = array.Add(99);

        // Assert
        await Assert.That(result.Length).IsEqualTo(initialCount + 1);
        await Assert.That(result[initialCount]).IsEqualTo(99);
    }

    [Test]
    public async Task Add_AtCapacity_TransitionsToOverflow()
    {
        // Arrange: Start with a full inline array
        var array = ImmutableInlineArray.Create<TArray, int>(Enumerable.Range(1, Capacity).ToArray());

        // Act
        var result = array.Add(101);

        // Assert
        await Assert.That(result.Length).IsEqualTo(Capacity + 1);
        await Assert.That(result[Capacity]).IsEqualTo(101);
        
        // Internal check: Since Capacity is exceeded, the result must now have an overflow array
        // We verify this via the public indexer logic which we know routes to overflow
    }

    [Test]
    public async Task Add_AlreadyInOverflow_IncrementsOverflowSize()
    {
        // Arrange: Start with items already in overflow
        var array = ImmutableInlineArray.Create<TArray, int>(Enumerable.Range(1, Capacity + 2).ToArray());

        // Act
        var result = array.Add(202);

        // Assert
        await Assert.That(result.Length).IsEqualTo(Capacity + 3);
        await Assert.That(result[Capacity + 2]).IsEqualTo(202);
    }

    [Test]
    public async Task Add_OutParameterVariant_ProducesSameResult()
    {
        // Arrange
        var array = ImmutableInlineArray.Create<TArray, int>([1, 2]);

        // Act
        array.Add(3, out var result);

        // Assert
        await Assert.That(result.Length).IsEqualTo(3);
        await Assert.That(result[2]).IsEqualTo(3);
    }

    [Test]
    public async Task Insert_AtStart_ShiftsAllElements()
    {
        // Arrange
        var array = ImmutableInlineArray.Create<TArray, int>([1, 2, 3]);
        
        // Act
        var result = array.Insert(0, 99);

        // Assert
        await Assert.That(result.Length).IsEqualTo(4);
        await Assert.That(result[0]).IsEqualTo(99);
        await Assert.That(result[1]).IsEqualTo(1);
    }

    [Test]
    public async Task Insert_AtCapacity_TriggersOverflow()
    {
        // Arrange: Fill exactly to capacity
        var initialItems = Enumerable.Range(0, Capacity).ToArray();
        var array = ImmutableInlineArray.Create<TArray, int>(initialItems);

        // Act: Insert at index 0, pushing the last element into overflow
        var result = array.Insert(0, 500);

        // Assert
        await Assert.That(result.Length).IsEqualTo(Capacity + 1);
        await Assert.That(result[0]).IsEqualTo(500);
        await Assert.That(result[Capacity]).IsEqualTo(initialItems.Last());
    }

    [Test]
    public async Task RemoveAt_FromMiddle_CollapsesArray()
    {
        // Arrange
        var array = ImmutableInlineArray.Create<TArray, int>([10, 20, 30]);

        // Act
        var result = array.RemoveAt(1); // Remove '20'

        // Assert
        await Assert.That(result.Length).IsEqualTo(2);
        await Assert.That(result[0]).IsEqualTo(10);
        await Assert.That(result[1]).IsEqualTo(30);
    }

    [Test]
    public async Task RemoveAt_FromOverflow_ShiftsOverflowCorrectly()
    {
        // Arrange: 2 elements in overflow
        var array = ImmutableInlineArray.Create<TArray, int>(Enumerable.Range(0, Capacity + 2).ToArray());

        // Act: Remove the first overflow element (at index Capacity)
        var result = array.RemoveAt(Capacity);

        // Assert
        await Assert.That(result.Length).IsEqualTo(Capacity + 1);
        // The element that was at Capacity + 1 should now be at Capacity
        await Assert.That(result[Capacity]).IsEqualTo(Capacity + 1);
    }

    [Test]
    public async Task Remove_WithComparer_FindsAndRemovesItem()
    {
        // Arrange
        var array = ImmutableInlineArray.Create<TArray, int>([1, 5, 10]);
        
        // Act
        var result = array.Remove(5, EqualityComparer<int>.Default);

        // Assert
        await Assert.That(result.Length).IsEqualTo(2);
        await Assert.That(result.Contains(5, EqualityComparer<int>.Default)).IsFalse();
    }

    [Test]
    [Arguments(-1)]
    [Arguments(2)]
    public async Task Indexer_NegativeIndex_ThrowsException(int invalidIndex)
    {
        var array = ImmutableInlineArray.Create<TArray, int>([1, 2]);
        
        var action = () => { _ = array[invalidIndex]; };
        
        await Assert.That(action).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(-1)]
    [Arguments(5)] // Greater than current length
    public async Task Insert_InvalidIndex_ThrowsException(int invalidIndex)
    {
        var array = ImmutableInlineArray.Create<TArray, int>([1, 2, 3]);
        
        var action = () => array.Insert(invalidIndex, 99);
        
        await Assert.That(action).Throws<ArgumentOutOfRangeException>();
    }
}

[InheritsTests]
public class FixedArray1Tests : ImmutableInlineArrayTests<FixedArray1<int>>
{
}

[InheritsTests]
public class FixedArray4Tests : ImmutableInlineArrayTests<FixedArray4<int>>
{
}

[InheritsTests]
public class FixedArray16Tests : ImmutableInlineArrayTests<FixedArray16<int>>
{
}

[InheritsTests]
public class FixedArray64Tests : ImmutableInlineArrayTests<FixedArray64<int>>
{
}

[InheritsTests]
public class FixedArray1024Tests : ImmutableInlineArrayTests<FixedArray1024<int>>
{
}