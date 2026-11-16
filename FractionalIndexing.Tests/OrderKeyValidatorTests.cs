using NUnit.Framework;

namespace FractionalIndexing.Tests;

public class OrderKeyValidatorTests
{
    private const string Base10Digits = "0123456789";

    private const string Base95Digits = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

    [TestCase("a0")]
    [TestCase("a1")]
    [TestCase("a0V")]
    [TestCase("a0l")]
    [TestCase("Zz")]
    [TestCase("Zy")]
    [TestCase("b00")]
    [TestCase("b125")]
    [TestCase("b129")]
    [TestCase("Y00")]
    public void IsValid_ValidBase62Keys_ShouldReturnTrue(string orderLevel)
    {
        var result = OrderKeyValidator.IsValid(orderLevel);

        Assert.That(result, Is.True, $"Expected '{orderLevel}' to be valid.");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("Привет Мир")]
    [TestCase("hello")]
    [TestCase("12345")]
    [TestCase("0")]
    [TestCase("Z")]
    [TestCase("b0")]
    [TestCase("a00")]
    [TestCase("a0!")]
    [TestCase("a!V")]
    [TestCase("a0V0")]
    public void IsValid_InvalidKeys_ShouldReturnFalse(string? orderLevel)
    {
        var result = OrderKeyValidator.IsValid(orderLevel);

        Assert.That(result, Is.False, $"Expected '{orderLevel}' to be invalid.");
    }

    [Test]
    public void IsValid_ReservedKey_ShouldReturnFalse()
    {
        var reservedKey = "A" + new string('0', 26);
        var result = OrderKeyValidator.IsValid(reservedKey);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_LongValidKey_ShouldReturnTrue()
    {
        var orderLevel = new string('z', 27);
        var result = OrderKeyValidator.IsValid(orderLevel);

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsValid_CustomBase10Alphabet_ShouldValidateCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(OrderKeyValidator.IsValid("a5", Base10Digits), Is.True);
            Assert.That(OrderKeyValidator.IsValid("a59", Base10Digits), Is.True);
            Assert.That(OrderKeyValidator.IsValid("aA", Base10Digits), Is.False);
            Assert.That(OrderKeyValidator.IsValid("a00", Base10Digits), Is.False);
        });
    }

    [Test]
    public void IsValid_CustomBase95Alphabet_ShouldValidateCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(OrderKeyValidator.IsValid("a ", Base95Digits), Is.True);
            Assert.That(OrderKeyValidator.IsValid("a!", Base95Digits), Is.True);
            Assert.That(OrderKeyValidator.IsValid("a0/", Base95Digits), Is.True);
            Assert.That(OrderKeyValidator.IsValid("a0~", Base95Digits), Is.True);
        });
    }

    [Test]
    public void IsValid_InvalidAlphabet_ShouldReturnFalse()
    {
        Assert.Multiple(() =>
        {
            Assert.That(OrderKeyValidator.IsValid("a0", "10"), Is.False);
            Assert.That(OrderKeyValidator.IsValid("a0", "001"), Is.False);
            Assert.That(OrderKeyValidator.IsValid("a0", "0"), Is.False);
            Assert.That(OrderKeyValidator.IsValid("a0", string.Empty), Is.False);
        });
    }

    [Test]
    public void TryValidate_InvalidKey_ShouldReturnError()
    {
        const string orderLevel = "Привет Мир";
        var result = OrderKeyValidator.TryValidate(orderLevel, out var error);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(error, Is.Not.Null.And.Not.Empty);
        });
    }

    [Test]
    public void TryValidate_ValidKey_ShouldReturnNullError()
    {
        const string orderLevel = "a0V";
        var result = OrderKeyValidator.TryValidate(orderLevel, out var error);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(error, Is.Null);
        });
    }

    [Test]
    public void IsValid_GeneratedKeys_ShouldAlwaysReturnTrue()
    {
        var bounds = new (string? A, string? B)[]
        {
            (null, null),
            (null, "a0"),
            ("a0", null),
            ("a0", "a1"),
            ("Zz", "a0")
        };

        foreach (var (a, b) in bounds)
        {
            var key = OrderKeyGenerator.GenerateKeyBetween(a, b);

            Assert.That(
                OrderKeyValidator.IsValid(key),
                Is.True,
                $"Generated key '{key}' is invalid. Bounds: '{a}' and '{b}'."
            );
        }
    }

    [Test]
    public void IsValid_GeneratedMultipleKeys_ShouldAlwaysReturnTrue()
    {
        var keys = OrderKeyGenerator.GenerateNKeysBetween("a0", "a1", 100);

        Assert.That(keys, Is.Not.Empty);

        foreach (var key in keys)
        {
            Assert.That(
                OrderKeyValidator.IsValid(key),
                Is.True,
                $"Generated key '{key}' is invalid."
            );
        }
    }
}