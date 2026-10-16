using System.Numerics;

public static class BitboardOperations
{
    public static void SetBit(ref ulong bitboard, int square) => bitboard |= 1UL << square;
    public static bool GetBit(ulong bitboard, int square) => (bitboard & (1UL << square)) != 0;
    public static void PopBit(ref ulong bitboard, int square) => bitboard &= ~(1UL << square);

    public static int CountBits(ulong bitboard)
    {
        int count = 0;
        while (bitboard != 0)
        {
            count++;
            bitboard &= bitboard - 1;
        }
        return count;
    }

    public static int GetLs1bIndex(ulong bitboard)
    {
        return BitOperations.TrailingZeroCount(bitboard);
    }
}
