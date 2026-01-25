namespace BitcoderCZ.Buffers.Tests;

public abstract class InlineListTests<TArray>
    where TArray : struct, IFixedArray<int>
{
    protected static int BufferCapacity => FixedArray.GetLength<TArray, int>();

    #region Lifecycle & Capacity

    [Test]
    public async Task Constructor_WithCollection_PopulatesCorrectly()
    {
        // Arrange
        var items = Enumerable.Range(1, BufferCapacity + 5).ToArray();

        // Act
        var list = new InlineList<TArray, int>(items);

        // Assert
        await Assert.That(list.Count).IsEqualTo(items.Length);
        await Assert.That(list.SequenceEqual(ref list)).IsTrue();
    }

    [Test]
    public async Task Capacity_Setter_HandlesShrinkingAndGrowing()
    {
        // Arrange
        var list = new InlineList<TArray, int>();

        // Act: Grow
        list.Capacity = BufferCapacity + 20;
        await Assert.That(list.Capacity).IsGreaterThanOrEqualTo(BufferCapacity + 20);

        // Act: Shrink below buffer
        list.Capacity = BufferCapacity - 1;
        await Assert.That(list.Capacity).IsEqualTo(BufferCapacity);
    }

    #endregion

    #region Mutation
    [Test]
    public async Task Add_TransitionsToInternalList_WhenBufferFull()
    {
        // Arrange
        var list = new InlineList<TArray, int>();

        // Act: Fill buffer
        for (int i = 0; i < BufferCapacity; i++) list.Add(i);

        // Act: Trigger overflow
        list.Add(999);

        // Assert
        await Assert.That(list.Count).IsEqualTo(BufferCapacity + 1);
        await Assert.That(list[BufferCapacity]).IsEqualTo(999);
    }

    [Test]
    public async Task Insert_AtBoundary_PushesElementToList()
    {
        // Arrange: Fill buffer exactly
        var list = new InlineList<TArray, int>(Enumerable.Range(0, BufferCapacity));

        // Act: Insert at index 0
        list.Insert(0, -1);

        // Assert
        await Assert.That(list.Count).IsEqualTo(BufferCapacity + 1);
        await Assert.That(list[0]).IsEqualTo(-1);
        await Assert.That(list[1]).IsEqualTo(0);
        // Ensure the last element of the buffer was pushed to the internal list
        await Assert.That(list[BufferCapacity]).IsEqualTo(BufferCapacity - 1);
    }

    [Test]
    public async Task Remove_ItemInList_ReturnsTrueAndShiftsBuffer()
    {
        // Arrange: 
        // If BufferCapacity is 4, items are [1, 2, 3, 4, 5]
        // Buffer holds: [1, 2, 3, 4], List holds: [5]
        var items = Enumerable.Range(1, BufferCapacity + 1).ToArray();
        var list = new InlineList<TArray, int>(items);

        int itemToRemove = items[0];        // The value 1
        int valueThatShouldShiftToFront = items[1]; // The value 2
        int valueThatShouldShiftFromList = items[BufferCapacity]; // The value 5 (if cap is 4)

        // Act
        bool removed = list.Remove(itemToRemove);

        // Assert
        await Assert.That(removed).IsTrue();
        await Assert.That(list.Count).IsEqualTo(BufferCapacity);

        // 1. Verify the "hole" at index 0 was filled by the next item (2)
        await Assert.That(list[0]).IsEqualTo(valueThatShouldShiftToFront);

        // 2. Verify the end of the buffer was filled by the first item from the overflow list
        // In a Size 4 buffer, index 3 should now be 5.
        await Assert.That(list[BufferCapacity - 1]).IsEqualTo(valueThatShouldShiftFromList);
    }

    [Test]
    public async Task RemoveAt_FromBuffer_PullsElementFromList()
    {
        // Arrange: Buffer full + 1 in list
        var list = new InlineList<TArray, int>(Enumerable.Range(0, BufferCapacity + 1));

        // Act: Remove from the very start of the buffer
        list.RemoveAt(0);

        // Assert
        await Assert.That(list.Count).IsEqualTo(BufferCapacity);
        await Assert.That(list[0]).IsEqualTo(1);
        // The element from _list[0] should have moved into _buffer[BufferCapacity - 1]
        await Assert.That(list[BufferCapacity - 1]).IsEqualTo(BufferCapacity);
    }

    [Test]
    public async Task Remove_LastItemInBuffer_PullsFromOverflow()
    {
        // Arrange
        // Buffer: [0,1,2,3]  Overflow: [4,5]
        var items = Enumerable.Range(0, BufferCapacity + 2).ToArray();
        var list = new InlineList<TArray, int>(items);

        int itemToRemove = items[BufferCapacity - 1]; // last buffer element
        int expectedReplacement = items[BufferCapacity]; // first overflow element

        // Act
        bool removed = list.Remove(itemToRemove);

        // Assert
        await Assert.That(removed).IsTrue();
        await Assert.That(list.Count).IsEqualTo(BufferCapacity + 1);

        // Last buffer slot should now contain the first overflow element
        await Assert.That(list[BufferCapacity - 1]).IsEqualTo(expectedReplacement);
    }

    [Test]
    public async Task Remove_FirstItemInOverflow_DoesNotAffectBuffer()
    {
        // Arrange
        // Buffer: [0,1,2,3]  Overflow: [4,5]
        var items = Enumerable.Range(0, BufferCapacity + 2).ToArray();
        var list = new InlineList<TArray, int>(items);

        int overflowFirstItem = items[BufferCapacity];

        // Act
        bool removed = list.Remove(overflowFirstItem);

        // Assert
        await Assert.That(removed).IsTrue();
        await Assert.That(list.Count).IsEqualTo(BufferCapacity + 1);

        // Buffer should remain intact
        for (int i = 0; i < BufferCapacity; i++)
        {
            await Assert.That(list[i]).IsEqualTo(items[i]);
        }

        // New first overflow item should now be the next value
        await Assert.That(list[BufferCapacity]).IsEqualTo(items[BufferCapacity + 1]);
    }

    [Test]
    public async Task Remove_LastItemInOverflow_ShrinksOverflowOnly()
    {
        // Arrange
        // Buffer: [0,1,2,3]  Overflow: [4,5]
        var items = Enumerable.Range(0, BufferCapacity + 2).ToArray();
        var list = new InlineList<TArray, int>(items);

        int lastOverflowItem = items[^1];

        // Act
        bool removed = list.Remove(lastOverflowItem);

        // Assert
        await Assert.That(removed).IsTrue();
        await Assert.That(list.Count).IsEqualTo(BufferCapacity + 1);

        // Buffer unchanged
        for (int i = 0; i < BufferCapacity; i++)
        {
            await Assert.That(list[i]).IsEqualTo(items[i]);
        }

        // Overflow should now contain only the former first element
        await Assert.That(list[BufferCapacity]).IsEqualTo(items[BufferCapacity]);
    }

    #endregion

    #region Specialized Operations

    [Test]
    public async Task Swap_CrossesSegmentBoundary()
    {
        // Arrange: 0 is in buffer, last is in list
        var list = new InlineList<TArray, int>(Enumerable.Range(0, BufferCapacity + 1));
        int lastIndex = BufferCapacity;

        // Act
        list.Swap(0, lastIndex);

        // Assert
        await Assert.That(list[0]).IsEqualTo(BufferCapacity);
        await Assert.That(list[lastIndex]).IsEqualTo(0);
    }

    [Test]
    public async Task GetRef_AllowsDirectMutation()
    {
        // Arrange
        var list = new InlineList<TArray, int>(Enumerable.Range(0, BufferCapacity + 1));

        // Act: Mutate via ref
        ref int inlineRef = ref list.GetRef(0);
        inlineRef = 555;

        ref int listRef = ref list.GetRef(BufferCapacity);
        listRef = 777;

        // Assert
        await Assert.That(list[0]).IsEqualTo(555);
        await Assert.That(list[BufferCapacity]).IsEqualTo(777);
    }

    [Test]
    public async Task CopyTo_WhenSpanningSegments_CopiesAllData()
    {
        // Arrange
        var items = Enumerable.Range(0, BufferCapacity + 5).ToArray();
        var list = new InlineList<TArray, int>(items);
        var destination = new int[items.Length];

        // Act
        list.CopyTo(destination, 0);

        // Assert
        await Assert.That(destination).IsEquivalentTo(items);
    }

    [Test]
    public async Task CopyTo_DestinationTooSmall_ThrowsArgumentException()
    {
        var list = new InlineList<TArray, int>(Enumerable.Range(0, 5));
        var destination = new int[2];

        var action = () => list.CopyTo(destination.AsSpan());

        await Assert.That(action).Throws<ArgumentOutOfRangeException>();
    }

    #endregion

    #region Equality & Hashing

    [Test]
    public async Task SequenceEqual_IdenticalLists_ReturnsTrue()
    {
        // Arrange
        var list1 = new InlineList<TArray, int>(Enumerable.Range(0, BufferCapacity + 2));
        var list2 = new InlineList<TArray, int>(Enumerable.Range(0, BufferCapacity + 2));

        // Act & Assert
        await Assert.That(list1.SequenceEqual(ref list2)).IsTrue();
    }

    [Test]
    public async Task SequenceEqual_DifferentData_ReturnsFalse()
    {
        var list1 = new InlineList<TArray, int>([1, 2, 3]);
        var list2 = new InlineList<TArray, int>([1, 0, 3]);

        await Assert.That(list1.SequenceEqual(ref list2)).IsFalse();
    }

    [Test]
    public async Task CalculateHashCode_SameContent_ProducesSameHash()
    {
        var list1 = new InlineList<TArray, int>(Enumerable.Range(0, BufferCapacity + 2));
        var list2 = new InlineList<TArray, int>(Enumerable.Range(0, BufferCapacity + 2));

        await Assert.That(list1.CalculateHashCode()).IsEqualTo(list2.CalculateHashCode());
    }

    #endregion

    #region Sorting

    [Test]
    public async Task Sort_UnorderedData_ResultsInSortedSequence()
    {
        // Arrange: Mixed data across buffer and list
        var data = new int[] { 10, 1, 9, 2, 8, 3, 7, 4 };
        var list = new InlineList<TArray, int>();
        foreach (var x in data) list.Add(x);

        // Act
        list.Sort();

        // Assert
        var expected = data.OrderBy(x => x).ToArray();
        for (int i = 0; i < list.Count; i++)
        {
            await Assert.That(list[i]).IsEqualTo(expected[i]);
        }
    }

    #endregion
}

[InheritsTests]
public class InlineList_Size1 : InlineListTests<FixedArray1<int>>
{
}

[InheritsTests]
public class InlineList_Size4 : InlineListTests<FixedArray4<int>>
{
}

[InheritsTests]
public class InlineList_Size16 : InlineListTests<FixedArray16<int>>
{
}

[InheritsTests]
public class InlineList_Size64 : InlineListTests<FixedArray64<int>>
{
}

[InheritsTests]
public class InlineList_Size1024 : InlineListTests<FixedArray1024<int>>
{
}