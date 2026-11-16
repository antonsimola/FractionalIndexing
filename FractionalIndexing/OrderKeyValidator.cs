namespace FractionalIndexing;

public static class OrderKeyValidator
{
    /// <summary>
    ///     Checks whether the specified value is a valid order key.
    /// </summary>
    /// <param name="orderLevel">Order key</param>
    /// <param name="digits">Digits</param>
    /// <returns>Is the value valid</returns>
    public static bool IsValid(string? orderLevel, string digits = Data.Base62Digits)
        => TryValidate(orderLevel, out _, digits);

    /// <summary>
    ///     Validates an order key and returns an error message if invalid.
    /// </summary>
    /// <param name="orderLevel">Order key</param>
    /// <param name="error">The error that caused validation to fail</param>
    /// <param name="digits">Digits</param>
    /// <returns>Is the value valid or errors</returns>
    public static bool TryValidate(string? orderLevel, out string? error, string digits = Data.Base62Digits)
    {
        error = null;

        if (string.IsNullOrEmpty(orderLevel))
        {
            error = "OrderLevel cannot be null or empty.";

            return false;
        }

        if (!IsValidAlphabet(digits))
        {
            error = "The alphabet must contain at least two unique characters sorted in ordinal order.";

            return false;
        }

        var head = orderLevel[0];
        int integerPartLength;

        if (head >= 'a' && head <= 'z')
        {
            integerPartLength = head - 'a' + 2;
        }
        else if (head >= 'A' && head <= 'Z')
        {
            integerPartLength = 'Z' - head + 2;
        }
        else
        {
            error = "Invalid first character of OrderLevel.";

            return false;
        }

        if (orderLevel.Length < integerPartLength)
        {
            error = "OrderLevel has an incomplete integer part.";

            return false;
        }

        if (IsReservedKey(orderLevel, digits[0]))
        {
            error = "OrderLevel contains a reserved key.";

            return false;
        }

        var remainingIntegerCharacters = integerPartLength - 1;
        var hasFractionalPart = false;

        for (var i = 1; i < orderLevel.Length; i++)
        {
            var current = orderLevel[i];

            var isIntegerPart = remainingIntegerCharacters > 0;

            if (digits.IndexOf(current) < 0)
            {
                var partName = isIntegerPart
                    ? "integer"
                    : "fractional";

                error = $"Invalid character '{current}' in {partName} part.";

                return false;
            }

            if (isIntegerPart)
                remainingIntegerCharacters--;
            else
                hasFractionalPart = true;
        }

        if (hasFractionalPart && orderLevel[orderLevel.Length - 1] == digits[0])
        {
            error = "OrderLevel cannot end with the zero digit.";

            return false;
        }

        return true;
    }

    private static bool IsValidAlphabet(string digits)
    {
        if (string.IsNullOrEmpty(digits) || digits.Length < 2)
            return false;

        for (var i = 1; i < digits.Length; i++)
        {
            if (digits[i] <= digits[i - 1])
                return false;
        }

        return true;
    }

    private static bool IsReservedKey(string value, char zero)
    {
        if (value.Length != 27 || value[0] != 'A')
            return false;

        for (var i = 1; i < value.Length; i++)
        {
            if (value[i] != zero)
                return false;
        }

        return true;
    }
}