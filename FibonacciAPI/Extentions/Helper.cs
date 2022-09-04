using System.Text;

namespace FibonacciAPI.Extentions
{
    public class Helper
    {
        public static string Plus(string a, string b)
        {
            StringBuilder res = new();
            int c = 0, d = 0;

            int max = Math.Max(a.Length, b.Length);
            if (a.Length < b.Length)
                a = a.PadLeft(max, '0');
            else
                if (a.Length > b.Length)
                b = b.PadLeft(max, '0');

            for (int i = max - 1; i >= 0; i--)
            {
                c = (d + (int)char.GetNumericValue(a[i]) + (int)char.GetNumericValue(b[i])) % 10;
                res.Append(c);
                d = (d + (int)char.GetNumericValue(a[i]) + (int)char.GetNumericValue(b[i])) / 10;
            }

            return new string(res.ToString().Reverse().ToArray());
        }
    }
}
