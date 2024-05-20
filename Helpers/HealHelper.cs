namespace WebApi_KR.Helpers
{
    public static class HealHelper
    {
        private static Dictionary<string, List<string>> Syndromes { get; set; }

        static HealHelper()
        {
            Syndromes = [];

            var c_size = (int)(Math.Pow(2, CycleEncoder.BinarySize) - 1);
            int[] errors = new int[c_size]; // 2^15 - 1 (все возможные комбинации i из 7)
            // Заполнение массива errors
            for (int i = 0; i < c_size; i++)
                errors[i] = i + 1;

            // Словарь синдром-ошибка
            foreach (var error in errors)
            {
                // В остатке - синдром
                var errorBinary = PolyHelper.DecimalToBinary(error, CycleEncoder.BinarySize);
                if (errorBinary.Count(x => x == '1') > 1)
                    continue;
                
                PolyHelper.PolynomialDivision(errorBinary, PolyHelper.DecimalToBinary(CycleEncoder.GenerativePoly, CycleEncoder.BinarySize), out int[] quotient, out int[] remainder);
                var syndromBinary = string.Concat(remainder);
                if (Syndromes.TryGetValue(syndromBinary, out List<string>? value))
                    value.Add(errorBinary);
                else
                    Syndromes.Add(syndromBinary, [errorBinary]);
            }
        }

        public static List<string> ErrorBySyndrome(string syndrome) => Syndromes[syndrome];

    }
}