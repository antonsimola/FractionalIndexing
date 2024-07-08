using NUnit.Framework;

namespace FractionalIndexing.Tests;

public class ListOrderTests
{
    [Test]
    public void MoveFirst_ManyTimes_ShouldSortAsExpected()
    {
        // Arrange
        const int numberOfIterations = 1000;

        var p1 = new Person(1, OrderKeyGenerator.GenerateKeyBetween(null, null));
        var p2 = new Person(3, OrderKeyGenerator.GenerateKeyBetween(p1.Order, null));
        var p3 = new Person(2, OrderKeyGenerator.GenerateKeyBetween(p2.Order, null));
        var list = new List<Person> { p1, p2, p3 };

        TestContext.WriteLine("Initial list");
        this.PrintList(list);

        // Act
        var comparer = new PersonSortComparer();

        for (var i = 0; i < numberOfIterations; i++)
        {
            var first = list.First();
            var lastIndex = list.Count - 1;
            var last = list[lastIndex];
            list[lastIndex] = last with { Order = OrderKeyGenerator.GenerateKeyBetween(null, first.Order) };
            list.Sort(comparer);

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine($"Move first {i + 1} times");
            this.PrintList(list);

            // Assert
            Assert.That(list[0].Id, Is.EqualTo(last.Id));
        }
    }

    [Test]
    public void MoveLast_ManyTimes_ShouldSortAsExpected()
    {
        // Arrange
        const int numberOfIterations = 1000;

        var p1 = new Person(1, OrderKeyGenerator.GenerateKeyBetween(null, null));
        var p2 = new Person(3, OrderKeyGenerator.GenerateKeyBetween(p1.Order, null));
        var p3 = new Person(2, OrderKeyGenerator.GenerateKeyBetween(p2.Order, null));
        var list = new List<Person> { p1, p2, p3 };

        TestContext.WriteLine("Initial list");
        this.PrintList(list);

        // Act
        var comparer = new PersonSortComparer();

        for (var i = 0; i < numberOfIterations; i++)
        {
            var first = list.First();
            var last = list.Last();
            list[0] = first with { Order = OrderKeyGenerator.GenerateKeyBetween(last.Order, null) };
            list.Sort(comparer);

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine($"Move last {i + 1} times");
            this.PrintList(list);

            // Assert
            Assert.That(list.Last().Id, Is.EqualTo(first.Id));
        }
    }

    [Test]
    public void MoveFirstToMiddle_ManyTimes_ShouldSortAsExpected()
    {
        // Arrange
        const int numberOfIterations = 1000;

        var p1 = new Person(1, OrderKeyGenerator.GenerateKeyBetween(null, null));
        var p2 = new Person(3, OrderKeyGenerator.GenerateKeyBetween(p1.Order, null));
        var p3 = new Person(2, OrderKeyGenerator.GenerateKeyBetween(p2.Order, null));
        var list = new List<Person> { p1, p2, p3 };

        TestContext.WriteLine("-- initial list --");
        this.PrintList(list);

        // Act
        var comparer = new PersonSortComparer();
        const int indexToMoveTo = 1;

        for (var i = 0; i < numberOfIterations; i++)
        {
            var first = list.First();
            var last = list.Last();
            var middle = list[indexToMoveTo];

            list[0] = first with { Order = OrderKeyGenerator.GenerateKeyBetween(middle.Order, last.Order) };

            list.Sort(comparer);

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine($"Move first to the middle {i + 1} times");
            this.PrintList(list);

            // Assert
            Assert.That(list[indexToMoveTo].Id, Is.EqualTo(first.Id));
        }
    }

    [Test]
    public void MoveLastToMiddle_ManyTimes_ShouldSortAsExpected()
    {
        // Arrange
        const int numberOfIterations = 1000;

        var p1 = new Person(1, OrderKeyGenerator.GenerateKeyBetween(null, null));
        var p2 = new Person(3, OrderKeyGenerator.GenerateKeyBetween(p1.Order, null));
        var p3 = new Person(2, OrderKeyGenerator.GenerateKeyBetween(p2.Order, null));
        var list = new List<Person> { p1, p2, p3 };

        TestContext.WriteLine("-- initial list --");
        this.PrintList(list);

        // Act
        var comparer = new PersonSortComparer();

        for (var i = 0; i < numberOfIterations; i++)
        {
            var first = list.First();
            var last = list.Last();
            var middle = list[1];

            list[^1] = last with { Order = OrderKeyGenerator.GenerateKeyBetween(first.Order, middle.Order) };

            list.Sort(comparer);

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine($"Move last to the middle {i + 1} times.");
            this.PrintList(list);
        }
    }

    private void PrintList(IEnumerable<Person> list)
    {
        foreach (var person in list)
        {
            TestContext.WriteLine(person.ToString());
        }
    }

    private sealed record Person(int Id, string Order);

    private sealed class PersonSortComparer : IComparer<Person>
    {
        public int Compare(Person? x, Person? y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x != null && y == null)
            {
                return 1;
            }

            if (x == null && y != null)
            {
                return -1;
            }

            return string.Compare(x!.Order, y!.Order, StringComparison.Ordinal);
        }
    }
}