using System.Text;
using WebApi_KR.Utils;

namespace WebApi_KR.Helpers
{
    public static class CycleEncoder
    {
        public static int BinaryBase { get; set; } = 11;

        public static int MetaPart { get; set; } = 3;
        public static int PayloadPart => BinaryBase - MetaPart;

        public static int BinarySize { get; set; } = 15;

        public const int CorruptionChance = 11;
        public const int LossChance = 2;

        // 929 = 1101
        public static int GenerativePoly { get; set; } = 13;
        public static string GenerativeBits => PolyHelper.DecimalToBinary(GenerativePoly, BinarySize);

        public static string Encode(string segment)
        {
            var cycled = string.Empty;
            var temp = string.Empty;
            var i = 0;
            while (i < segment.Length)
            {
                temp += segment[i];
                i++;
                if (temp.Length >= PayloadPart || i == segment.Length)
                {
                    temp = PolyHelper.LeftSwiftTo($"{Convert.ToString(PayloadPart - temp.Length, 2).PadLeft(3, '0')}{temp}", BinaryBase);
                    cycled += EncodePart(temp);
                    temp = string.Empty;
                }
            }

            return cycled;
        }

        public static string EncodePart(string sequence)
        {
            if (sequence.Length > BinaryBase)
                throw new ArgumentException();

            // Нормировать последоваельность
            var swiftedSeq = PolyHelper.LeftSwiftTo(sequence, BinarySize);
            PolyHelper.PolynomialDivision(swiftedSeq, GenerativeBits, out int[] quotient, out int[] remainder);
            var coded = string.Concat(PolyHelper.SumArr(PolyHelper.StrToArr(swiftedSeq), remainder));
            return coded;
        }

        public static string TryCorrupt(string segment)
        {
            var corrupted = segment;
            if (Random.Shared.Next(100) < CorruptionChance)
            {
                var temp = string.Empty;

                var nr = new NormalRandom();
                var randBit = nr.NextDouble() * 20 + 200; // Random.Shared.Next(segment.Length);
                if (randBit > segment.Length - 1)
                    randBit = segment.Length - 1;

                for (int i = 0; i < segment.Length; i++)
                    temp += i == randBit ? Invert(segment[i]) : segment[i];
                corrupted = temp;
            }
            return corrupted;
        }

        public static string Decode(string segment)
        {
            var decoded = string.Empty;
            var temp = string.Empty;
            var i = 0;
            while (i < segment.Length)
            {
                temp += segment[i];
                i++;
                if (temp.Length >= BinarySize || i == segment.Length)
                {
                    decoded += DecodePart(temp);
                    temp = string.Empty;
                }
            }
            return decoded;
        }

        public static string DecodePart(string sequence)
        {
            PolyHelper.PolynomialDivision(sequence, GenerativeBits, out int[] quotient, out int[] remainder);
            if (remainder.Sum() != 0)
            {
                sequence = HealPart(sequence);
            }

            var data = sequence[..BinaryBase];

            var dump = data[..MetaPart];
            var dumpLength = Convert.ToInt32(dump, 2);

            return data[MetaPart..(data.Length - dumpLength)];
        }

        public static string TryHeal(string corrupted)
        {
            var recovered = string.Empty;
            var temp = string.Empty;
            var i = 0;
            while (i < corrupted.Length)
            {
                temp += corrupted[i];
                i++;
                if (temp.Length >= BinarySize || i == corrupted.Length)
                {
                    recovered += HealPart(temp);
                    temp = string.Empty;
                }
            }
            return recovered;
        }

        public static string HealPart(string sequence)
        {
            PolyHelper.PolynomialDivision(sequence, GenerativeBits, out int[] quotient, out int[] remainder);

            var syndromeBinary = string.Concat(remainder);
            var errors = HealHelper.ErrorBySyndrome(syndromeBinary);

            var index = Random.Shared.Next(errors.Count);

            var corruptedIndex = errors[index].IndexOf('1');
            var healed = string.Empty;
            for (int i = 0; i < sequence.Length; i++)
                healed += i == corruptedIndex ? Invert(sequence[i]) : sequence[i];
            return healed;
        }

        public static char Invert(char input) => input == '0' ? '1' : '0';
    }
}