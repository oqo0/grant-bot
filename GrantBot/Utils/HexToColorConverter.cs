using System.Globalization;

namespace GrantBot.Utils;

public static class HexToColorConverter
{
    public static Color Convert(string hexString)
    {
        if (!hexString.StartsWith('#') || hexString.Length != 7)
            return Color.White;
        
        byte red = GetByteFromStr(hexString, 1, 2);
        byte green = GetByteFromStr(hexString, 3, 2);
        byte blue = GetByteFromStr(hexString, 5, 2);

        return Color.FromRgb(red, green, blue);
    }

    private static byte GetByteFromStr(string str, int startIndex, int length)
        => byte.Parse(str.Substring(startIndex, length), NumberStyles.HexNumber);
}