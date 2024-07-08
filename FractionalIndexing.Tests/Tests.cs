using NUnit.Framework;

namespace FractionalIndexing.Tests;

public class FractionalIndexingTests
{
    private record Person(int Age, string Order);

    [Test]
    public void LinqTestAndExample()
    {
        var p1 = new Person(1, OrderKeyGenerator.GenerateKeyBetween(null, null));
        var p2 = new Person(3, OrderKeyGenerator.GenerateKeyBetween(p1.Order, null));
        //between p1 and p2
        var p3 = new Person(2, OrderKeyGenerator.GenerateKeyBetween(p1.Order, p2.Order));
        var p4 = new Person(4, OrderKeyGenerator.GenerateKeyBetween(p2.Order, null));


        var ordered = new Person[] { p1, p2, p3, p4 }.OrderBy(p => p.Order).ToList();

        Assert.That(ordered, Is.EqualTo(new List<Person>() { p1, p3, p2, p4 }));
    }

    [Test]
    public void TestsBase95()
    {
        TestBase95("a00", "a01", "a00P");
        TestBase95("a0/", "a00", "a0/P");
        TestBase95(null, null, "a ");
        TestBase95("a ", null, "a!");
        TestBase95(null, "a ", "Z~");
        TestBase95("a0 ", "a0!", "invalid order key: a0 ");
        TestBase95(
            null,
            "A                          0",
            "A                          ("
        );
        TestBase95("a~", null, "b  ");
        TestBase95("Z~", null, "a ");
        TestBase95("b   ", null, "invalid order key: b   ");
        TestBase95("a0", "a0V", "a0;");
        TestBase95("a  1", "a  2", "a  1P");
        TestBase95(
            null,
            "A                          ",
            "invalid order key: A                          "
        );
    }

    [Test]
    public void TestsN()
    {
        TestN(null, null, 5, "a0 a1 a2 a3 a4");
        TestN("a4", null, 10, "a5 a6 a7 a8 a9 b00 b01 b02 b03 b04");
        TestN(null, "a0", 5, "Z5 Z6 Z7 Z8 Z9");
        TestN(
            "a0",
            "a2",
            20,
            "a01 a02 a03 a035 a04 a05 a06 a07 a08 a09 a1 a11 a12 a13 a14 a15 a16 a17 a18 a19"
        );
    }

    [Test]
    public void Tests()
    {
        Test(null, null, "a0");
        Test(null, "a0", "Zz");
        Test(null, "Zz", "Zy");
        Test("a0", null, "a1");
        Test("a1", null, "a2");
        Test("a0", "a1", "a0V");
        Test("a1", "a2", "a1V");
        Test("a0V", "a1", "a0l");
        Test("Zz", "a0", "ZzV");
        Test("Zz", "a1", "a0");
        Test(null, "Y00", "Xzzz");
        Test("bzz", null, "c000");
        Test("a0", "a0V", "a0G");
        Test("a0", "a0G", "a08");
        Test("b125", "b129", "b127");
        Test("a0", "a1V", "a1");
        Test("Zz", "a01", "a0");
        Test(null, "a0V", "a0");
        Test(null, "b999", "b99");
        Test(
            null,
            "A00000000000000000000000000",
            "invalid order key: A00000000000000000000000000"
        );
        Test(null, "A000000000000000000000000001", "A000000000000000000000000000V");
        Test("zzzzzzzzzzzzzzzzzzzzzzzzzzy", null, "zzzzzzzzzzzzzzzzzzzzzzzzzzz");
        Test("zzzzzzzzzzzzzzzzzzzzzzzzzzz", null, "zzzzzzzzzzzzzzzzzzzzzzzzzzzV");
        Test("a00", null, "invalid order key: a00");
        Test("a00", "a1", "invalid order key: a00");
        Test("0", "1", "invalid order key head: 0");
        Test("a1", "a0", "a1 >= a0");
        Test("a0", "a00V", "a00G");
    }

    private void Test(string? a, string? b, string? exp)
    {
        var act = "";
        try
        {
            act = OrderKeyGenerator.GenerateKeyBetween(a, b);
        }
        catch (Exception e)
        {
            act = e.Message;
        }

        Assert.That(act, Is.EqualTo(exp));
    }

    private void TestN(string? a, string? b, int n, string exp)
    {
        var base10Digits = "0123456789";
        if (base10Digits == null) throw new ArgumentNullException(nameof(base10Digits));
        
        var act = "";
        try
        {
            act = string.Join(" ", OrderKeyGenerator.GenerateNKeysBetween(a, b, n, base10Digits));
        }
        catch (Exception e)
        {
            act = e.Message;
        }

        Assert.That(act, Is.EqualTo(exp));
    }

    private void TestBase95(string? a, string? b, string exp)
    {
        var base95Digits =
            " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

        var act = "";
        try
        {
            act = OrderKeyGenerator.GenerateKeyBetween(a, b, base95Digits);
        }
        catch (Exception e)
        {
            act = e.Message;
        }

        Assert.That(act, Is.EqualTo(exp));
    }
}