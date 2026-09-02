using System.Security.Cryptography;
using System.Text;

namespace Gadema.Api.Services.Authentication;

/// <summary>Standard TOTP (RFC 6238): SHA-1, 30 s period, 6 digits, ±1 step window.</summary>
public static class TotpService
{
    public static string GenerateSecret()
    {
        var bytes = RandomNumberGenerator.GetBytes(20);
        return Base32Encode(bytes);
    }

    public static bool Validate(string base32Secret, string code)
    {
        var key = Base32Decode(base32Secret);
        if (key == null) return false;

        var step = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds / 30;
        for (var offset = -1L; offset <= 1; offset++)
            if (ConstantTimeEquals(Compute(key, step + offset), code))
                return true;
        return false;
    }

    private static string Compute(byte[] key, long step)
    {
        var buffer = new byte[8];
        for (var i = 7; i >= 0; i--) { buffer[i] = (byte)(step & 0xFF); step >>= 8; }

        using var hmac = new HMACSHA1(key);
        var hash = hmac.ComputeHash(buffer);

        var offset = hash[^1] & 0x0F;
        var binary = ((hash[offset] & 0x7F) << 24) | (hash[offset + 1] << 16)
                   | (hash[offset + 2] << 8) | hash[offset + 3];
        return (binary % 1_000_000).ToString("D6");
    }

    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a.Length != b.Length) return false;
        var diff = 0;
        for (var i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
        return diff == 0;
    }

    private static readonly string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public static string Base32Encode(byte[] data)
    {
        var chars = new char[(data.Length * 8 + 4) / 5];
        int bits = 0, value = 0;
        var length = chars.Length;
        for (var i = 0; i < data.Length; i++)
        {
            value = (value << 8) | data[i];
            bits += 8;
            while (bits >= 5)
            {
                chars[--length] = Base32Alphabet[(value >> (bits -= 5)) & 31];
            }
        }
        while (bits > 0)
        {
            chars[--length] = Base32Alphabet[(value << (5 - bits)) & 31];
            bits += 3;
        }
        return new string(chars);
    }

    public static byte[]? Base32Decode(string base32)
    {
        base32 = base32.ToUpperInvariant().TrimEnd('=');
        int bits = 0, value = 0;
        var output = new byte[base32.Length * 5 / 8];
        var index = 0;
        var Base32AlphabetArray = Base32Alphabet.ToArray<char>();
        foreach (var c in base32)
        {
            var idx = Array.IndexOf(Base32AlphabetArray, c);
            if (idx < 0) return null;
            value = (value << 5) | idx;
            bits += 5;
            if (bits >= 8)
            {
                output[index++] = (byte)((value >> (bits -= 8)) & 0xFF);
            }
        }
        return output;
    }
}