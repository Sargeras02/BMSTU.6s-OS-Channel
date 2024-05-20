using System.Runtime.CompilerServices;

namespace WebApi_KR.Helpers
{
    public static class PolyHelper
    {
        /// <summary>
        /// Конвертирует заданное число в бинарную с.с., возвращая <paramref name="n"/>-разрядную запись.
        /// </summary>
        /// <param name="number">Заданное число.</param>
        /// <param name="n">Количество разрядов записи.</param>
        /// <returns>Бинарная запись <paramref name="number"/>.</returns>
        public static string DecimalToBinary(int number, int n) => Convert.ToString(number, 2).PadLeft(n, '0');

        public static int BinaryToDecimal(string binaryString) => Convert.ToInt32(binaryString, 2);

        /// <summary>
        /// 111, 5 => 111_00
        /// </summary>
        /// <param name="binaryString"></param>
        /// <param name="binarySize"></param>
        /// <returns></returns>
        public static string LeftSwiftTo(string binaryString, int binarySize)
        {
            var res = (string)binaryString.Clone();
            while (res.Length < binarySize)
                res += "0";
            return res;
        }

        /// <summary>
        /// 111, 5 => 00111
        /// </summary>
        /// <param name="binaryString"></param>
        /// <param name="binarySize"></param>
        /// <returns></returns>
        public static string NormalizeTo(string binaryString, int binarySize)
        {
            var res = (string)binaryString.Clone();
            while (res.Length < binarySize)
                res = $"0{res}";
            return res;
        }

        public static int[] SumArr(int[] left, int[] right)
        {
            var len = Math.Max(left.Length, right.Length);
            left = Inverse(left);
            right = Inverse(right);

            var res = new List<int>();
            bool overload = false;
            for (int i = 0; i < len; i++)
            {
                var temp = left[i] + right[i];
                if (overload)
                {
                    temp++;
                    overload = false;
                }
                
                if (temp > 1)
                {
                    temp -= 2;
                    overload = true;
                }
                
                res.Add(temp);
            }
            if (overload)
                res.Add(1);
            return Fit(Inverse([.. res]));
        }

        public static int[] Fit(int[] binaryData, int len = 0) => StrToArr(Fit(string.Concat(binaryData), len));
        public static string Fit(string binaryData, int len = 0)
        {
            while (binaryData[0] == 0 && binaryData.Length > len)
                binaryData = binaryData.Remove(0, 1);
            return binaryData;
        }

        public static int[] Inverse(int[] arr)
        {
            var res = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                res[arr.Length - i - 1] = arr[i];
            }
            return res;
        }

        /// <summary>
        /// Преобразует бинарную строку в целочисленный массив.
        /// </summary>
        /// <param name="binaryString">Бинарная строка.</param>
        /// <returns>Целочисленный массив.</returns>
        public static int[] StrToArr(string binaryString)
        {
            int[] array = binaryString.Select(c => c - '0').ToArray();
            return array;
        }

        public static void PolynomialDivision(string dividend, string divisor, out int[] quotient, out int[] remainder)
            => PolynomialDivision(StrToArr(dividend), StrToArr(divisor), out quotient, out remainder);
        
        public static void PolynomialDivision(int[] dividend, int[] divisor, out int[] quotient, out int[] remainder)
        {
            var n = dividend.Length;
            quotient = new int[n];
            remainder = new int[n];

            int j = 0;
            while (j < n && divisor[j] == 0)
            {
                j++;
            }

            int rw = n - j;
            int[] temp = new int[rw];
            int[] subDividend = new int[rw];
            for (int i = 0; i < rw; i++)
            {
                subDividend[i] = dividend[i];
            }
            for (int i = rw - 1; i < n; i++)
            {
                int X = subDividend[0] / divisor[j];

                quotient[i] = X;

                for (int k = 0; k < rw; k++)
                {
                    temp[k] = X * divisor[j + k];
                }

                for (int k = 0; k < rw; k++)
                {
                    subDividend[k] ^= temp[k];
                }

                if (i + 1 < n)
                {
                    for (int k = 0; k < rw - 1; k++)
                    {
                        subDividend[k] = subDividend[k + 1];
                    }
                    subDividend[rw - 1] = dividend[i + 1];
                }
            }

            for (int i = n - 1; i > 0; i--)
            {
                if (i >= n - rw)
                    remainder[i] = subDividend[i - n + rw];
            }
        }
    }
}
