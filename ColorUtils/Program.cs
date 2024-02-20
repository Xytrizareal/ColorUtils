using System;
using System.Text.RegularExpressions;

class ColorConverter
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Enter a color (hex or RGB): ");
        string color = Console.ReadLine();
        PrintColorValues(color);
    }

    public static (int, int, int) HexToRgb(string hexColor)
    {
        hexColor = hexColor.TrimStart('#');
        if (hexColor.Length == 3)
        {
            hexColor = string.Join("", hexColor.Select(c => $"{c}{c}"));
        }
        return (
            Convert.ToInt32(hexColor.Substring(0, 2), 16),
            Convert.ToInt32(hexColor.Substring(2, 2), 16),
            Convert.ToInt32(hexColor.Substring(4, 2), 16)
        );
    }

    public static string RgbToHex((int, int, int) rgbColor)
    {
        return $"{rgbColor.Item1:X2}{rgbColor.Item2:X2}{rgbColor.Item3:X2}";
    }

    public static (double, double, double) RgbToHsl((int, int, int) rgbColor)
    {
        double r = rgbColor.Item1 / 255.0;
        double g = rgbColor.Item2 / 255.0;
        double b = rgbColor.Item3 / 255.0;
        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double h, s, l;
        h = s = l = (max + min) / 2.0;

        if (max == min)
        {
            h = s = 0;
        }
        else
        {
            double d = max - min;
            s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
            if (max == r)
            {
                h = (g - b) / d + (g < b ? 6 : 0);
            }
            else if (max == g)
            {
                h = (b - r) / d + 2;
            }
            else if (max == b)
            {
                h = (r - g) / d + 4;
            }
            h /= 6;
        }
        return (h * 360, s, l);
    }

    public static (double, double, double) RgbToHsv((int, int, int) rgbColor)
    {
        double delta, min;
        double h = 0, s, v;

        min = Math.Min(Math.Min(rgbColor.Item1, rgbColor.Item2), rgbColor.Item3);
        v = Math.Max(Math.Max(rgbColor.Item1, rgbColor.Item2), rgbColor.Item3);
        delta = v - min;

        if (v == 0.0)
        {
            s = 0;
        }
        else
        {
            s = delta / v;
        }

        if (s == 0)
        {
            h = 0.0;
        }
        else
        {
            if (rgbColor.Item1 == v)
            {
                h = (rgbColor.Item2 - rgbColor.Item3) / delta;
            }
            else if (rgbColor.Item2 == v)
            {
                h = 2 + (rgbColor.Item3 - rgbColor.Item1) / delta;
            }
            else if (rgbColor.Item3 == v)
            {
                h = 4 + (rgbColor.Item1 - rgbColor.Item2) / delta;
            }

            h *= 60;
            if (h < 0.0)
            {
                h = h + 360;
            }
        }

        return (h, s, v / 255);
    }

    public static (double c, double m, double y, double k) RgbToCmyk(int r, int g, int b)
    {
        double rd = r / 255.0;
        double gd = g / 255.0;
        double bd = b / 255.0;

        double k = 1 - Math.Max(rd, Math.Max(gd, bd));
        double c = (1 - rd - k) / (1 - k);
        double m = (1 - gd - k) / (1 - k);
        double y = (1 - bd - k) / (1 - k);

        if (k == 1)
        {
            c = 0;
            m = 0;
            y = 0;
        }

        return (c, m, y, k);
    }

    public static void PrintColorValues(string color)
    {
        color = color.Replace(" ", "");
        (int, int, int) rgbColor;

        if (Regex.IsMatch(color, "^#?[0-9a-fA-F]{3}(?:[0-9a-fA-F]{3})?$"))
        {
            rgbColor = HexToRgb(color);
        }
        else if (Regex.IsMatch(color, @"^\(\d{1,3},\d{1,3},\d{1,3}\)$") || Regex.IsMatch(color, @"^\d{1,3},\d{1,3},\d{1,3}$"))
        {
            string[] parts = color.Trim('(', ')').Split(',');
            rgbColor = (int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }
        else
        {
            Console.WriteLine("Invalid color format. Please enter a hex color (example: #ffffff) or an RGB color (example: 255, 255, 255).");

            Console.ReadLine();
            return;
        }

        string hex = RgbToHex(rgbColor);
        var hsl = RgbToHsl(rgbColor);
        var hsv = RgbToHsv(rgbColor);
        var cmyk = RgbToCmyk(rgbColor.Item1, rgbColor.Item2, rgbColor.Item3);

        Console.WriteLine("CODE\tVALUE\t\tHTML/CSS");
        Console.WriteLine($"Hex\t{hex}\t\t#{hex}");
        Console.WriteLine($"RGB\t{rgbColor.Item1}, {rgbColor.Item2}, {rgbColor.Item3}\trgb({rgbColor.Item1}, {rgbColor.Item2}, {rgbColor.Item3})");
        Console.WriteLine($"HSL\t{Math.Round(hsl.Item1)}°, {Math.Round(hsl.Item2 * 100)}%, {Math.Round(hsl.Item3 * 100)}%\thsl({Math.Round(hsl.Item1)}, {Math.Round(hsl.Item2 * 100)}%, {Math.Round(hsl.Item3 * 100)}%)");
        Console.WriteLine($"HSV\t{Math.Round(hsv.Item1)}°, {Math.Round(hsv.Item2 * 100)}%, {Math.Round(hsv.Item3 * 100)}%\thsv({Math.Round(hsv.Item1)}, {Math.Round(hsv.Item2 * 100)}%, {Math.Round(hsv.Item3 * 100)}%)");
        Console.WriteLine($"CMYK\t{Math.Round(cmyk.c * 100)}%, {Math.Round(cmyk.m * 100)}%, {Math.Round(cmyk.y * 100)}%, {Math.Round(cmyk.k * 100)}%\t");

        Console.ReadLine();
    }
}
