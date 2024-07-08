namespace FractionalIndexing;

public static class OrderKeyGenerator
{
    private const string Base62Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";


    /// <summary>
    ///     Generate order key for an item that is about to be inserted between a and b
    /// </summary>
    /// <param name="a">the key before, or null if inserting as first one</param>
    /// <param name="b">the key after, or null if inserting as last one</param>
    /// <param name="digits">optionally choose your own list of characters to use in the key</param>
    /// <returns>key</returns>
    public static string GenerateKeyBetween(string? a, string? b, string digits = Base62Digits)
    {
        if (a != null) ValidateOrderKey(a, digits);

        if (b != null) ValidateOrderKey(b, digits);

        if (a != null && b != null && string.Compare(a, b, StringComparison.Ordinal) >= 0)
            throw new ArgumentException($"{a} >= {b}");

        if (a == null)
        {
            if (b == null) return "a" + digits[0];

            var ib1 = GetIntegerPart(b);
            var fb1 = b.Substring(ib1.Length);
            if (ib1 == "A" + new string(digits[0], 26)) return ib1 + Midpoint("", fb1, digits);

            if (string.Compare(ib1, b, StringComparison.Ordinal) < 0) return ib1;

            var res = DecrementInteger(ib1, digits);
            if (res == null) throw new ArgumentException("cannot decrement any more");

            return res;
        }

        if (b == null)
        {
            var ia2 = GetIntegerPart(a);
            var fa2 = a.Substring(ia2.Length);
            var i1 = IncrementInteger(ia2, digits);
            return i1 == null ? ia2 + Midpoint(fa2, null, digits) : i1;
        }

        var ia = GetIntegerPart(a);
        var fa = a.Substring(ia.Length);
        var ib = GetIntegerPart(b);
        var fb = b.Substring(ib.Length);
        if (ia == ib) return ia + Midpoint(fa, fb, digits);

        var i = IncrementInteger(ia, digits);
        if (i == null) throw new ArgumentException("cannot increment any more");

        if (string.Compare(i, b, StringComparison.Ordinal) < 0) return i;

        return ia + Midpoint(fa, null, digits);
    }

    /// <summary>
    ///     Generate multiple keys at the same time, distributing keys more evenly.
    /// </summary>
    /// <param name="a">the key before, or null if inserting as first one</param>
    /// <param name="b">the key after, or null if inserting as last one</param>
    /// <param name="n">number of keys generated</param>
    /// <param name="digits">optionally choose your own list of characters to use in the key</param>
    /// <returns>array of keys</returns>
    public static IList<string> GenerateNKeysBetween(string? a, string? b, int n, string digits = Base62Digits)
    {
        if (n == 0) return Array.Empty<string>();

        if (n == 1) return new List<string> { GenerateKeyBetween(a, b, digits) };

        if (b == null)
        {
            var c1 = GenerateKeyBetween(a, b, digits);
            var result = new List<string> { c1 };
            for (var i = 0; i < n - 1; i++)
            {
                c1 = GenerateKeyBetween(c1, b, digits);
                result.Add(c1);
            }

            return result;
        }

        if (a == null)
        {
            var c2 = GenerateKeyBetween(a, b, digits);
            var result = new List<string> { c2 };
            for (var i = 0; i < n - 1; i++)
            {
                c2 = GenerateKeyBetween(a, c2, digits);
                result.Add(c2);
            }

            result.Reverse();
            return result;
        }

        var mid = (int)Math.Floor((double)n / 2);
        var c = GenerateKeyBetween(a, b, digits);
        var res = new List<string>(n);
        res.AddRange(GenerateNKeysBetween(a, c, mid, digits));
        res.Add(c);
        res.AddRange(GenerateNKeysBetween(c, b, n - mid - 1, digits));
        return res;
    }

    private static string Midpoint(string? a, string? b, string digits = Base62Digits)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));

        var zero = digits[0];
        if (b != null && string.Compare(a, b, StringComparison.Ordinal) >= 0) throw new Exception(a + " >= " + b);

        if (a.LastOrDefault() == zero || (b != null && b.LastOrDefault() == zero))
            throw new ArgumentException("trailing zero");

        if (b != null)
        {
            // remove longest common prefix.  pad `a` with 0s as we
            // go.  note that we don't need to pad `b`, because it can't
            // end before `a` while traversing the common prefix.
            var n = 0;
            while ((n >= a.Length ? zero : a[n]) == b[n]) n++;

            if (n > 0) return b.Substring(0, n) + Midpoint(n >= a.Length ? string.Empty : a.Substring(n), b.Substring(n), digits);
        }

        // first digits (or lack of digit) are different
        var digitA = string.IsNullOrEmpty(a) ? 0 : digits.IndexOf(a[0]);
        var digitB = b != null ? digits.IndexOf(b[0]) : digits.Length;
        if (digitB - digitA > 1)
        {
            var midDigit = (int)Math.Floor(0.5 * (digitA + digitB) + 0.5);
            return digits[midDigit] + "";
        }

        // first digits are consecutive
        if (!string.IsNullOrEmpty(b) && b!.Length > 1)
            return $"{b[0]}";
        // `b` is null or has length 1 (a single digit).
        // the first digit of `a` is the previous digit to `b`,
        // or 9 if `b` is null.
        // given, for example, midpoint('49', '5'), return
        // '4' + midpoint('9', null), which will become
        // '4' + '9' + midpoint('', null), which is '495'
        return digits[digitA] + Midpoint(string.IsNullOrEmpty(a) ? "" : a.Substring(1), null, digits);
    }

    private static void ValidateInteger(string num)
    {
        if (num.Length != GetIntegerLength(num[0] + ""))
            throw new ArgumentException("invalid integer part of order key: " + num);
    }

    private static int GetIntegerLength(string head)
    {
        if (string.Compare(head, "a", StringComparison.Ordinal) >= 0 &&
            string.Compare(head, "z", StringComparison.Ordinal) <= 0)
            return head[0] - 'a' + 2;
        if (string.Compare(head, "A", StringComparison.Ordinal) >= 0 &&
            string.Compare(head, "Z", StringComparison.Ordinal) <= 0)
            return 'Z' - head[0] + 2;
        throw new ArgumentException($"invalid order key head: {head}");
    }

    private static string GetIntegerPart(string key)
    {
        var integerPartLength = GetIntegerLength(key[0] + "");
        if (integerPartLength > key.Length) throw new ArgumentException($"invalid order key: {key}");

        return key.Substring(0, integerPartLength);
    }

    private static void ValidateOrderKey(string key, string digits)
    {
        if (key == "A" + new string(digits[0], 26)) throw new ArgumentException($"invalid order key: {key}");

        // getIntegerPart will throw if the first character is bad,
        // or the key is too short.  we'd call it to check these things
        // even if we didn't need the result
        var i = GetIntegerPart(key);
        var f = key.Substring(i.Length);
        if (f.LastOrDefault() == digits[0]) throw new ArgumentException($"invalid order key: {key}");
    }

    private static string? IncrementInteger(string x, string digits)
    {
        ValidateInteger(x);
        var chars = x.ToCharArray();
        var head = chars[0];
        var digs = chars.Skip(1).ToList();
        var carry = true;
        for (var i = digs.Count - 1; carry && i >= 0; i--)
        {
            var d = digits.IndexOf(digs[i]) + 1;
            if (d == digits.Length)
            {
                digs[i] = digits[0];
            }
            else
            {
                digs[i] = digits[d];
                carry = false;
            }
        }

        if (carry)
        {
            if (head == 'Z') return "a" + digits[0];

            if (head == 'z') return null;

            var h = (char)(head + 1);
            if (h > 'a')
                digs.Add(digits[0]);
            else
                digs.RemoveAt(digs.Count - 1);

            return h + string.Join("", digs);
        }

        return head + string.Join("", digs);
    }

    private static string DecrementInteger(string x, string digits)
    {
        ValidateInteger(x);
        var chars = x.ToCharArray();
        var head = chars[0];
        var digs = chars.Skip(1).ToList();
        var borrow = true;
        for (var i = digs.Count - 1; borrow && i >= 0; i--)
        {
            var d = digits.IndexOf(digs[i]) - 1;
            if (d == -1)
            {
                digs[i] = digits[digits.Length - 1];
            }
            else
            {
                digs[i] = digits[d];
                borrow = false;
            }
        }

        if (borrow)
        {
            if (head == 'a') return "Z" + digits[digits.Length - 1];

            if (head == 'A') return null;

            var h = (char)(head - 1);
            if (h < 'Z')
                digs.Add(digits[digits.Length - 1]);
            else
                digs.RemoveAt(digs.Count - 1);

            return h + string.Join("", digs);
        }

        return head + string.Join("", digs);
    }
}