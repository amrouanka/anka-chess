
public static class MoveEncoding
{
    public static int EncodeMove(int source, int target, int piece, int promoted, int capture, int @double, int enpassant, int castling)
    {
        return source | (target << 6) | (piece << 12) | (promoted << 16) | (capture << 20) | (@double << 21) | (enpassant << 22) | (castling << 23);
    }

    public static int GetMoveSource(int move) => move & 0x3F;
    public static int GetMoveTarget(int move) => (move & 0xFC0) >> 6;
    public static int GetMovePiece(int move) => (move & 0xF000) >> 12;
    public static int GetMovePromoted(int move) => (move & 0xF0000) >> 16;
    public static int GetMoveCapture(int move) => move & 0x100000;
    public static int GetMoveDouble(int move) => move & 0x200000;
    public static int GetMoveEnpassant(int move) => move & 0x400000;
    public static int GetMoveCastling(int move) => move & 0x800000;

    public static string GetMove(int move)
    {
        return $"{Board.squareToCoordinates[GetMoveSource(move)]}{Board.squareToCoordinates[GetMoveTarget(move)]}{Board.promotedPieces[GetMovePromoted(move)]}";
    }

    public static void PrintMove(int move)
    {
        Console.WriteLine($"{Board.squareToCoordinates[GetMoveSource(move)]}{Board.squareToCoordinates[GetMoveTarget(move)]}{Board.promotedPieces[GetMovePromoted(move)]}");
    }
}

public struct MoveList
{
    public int[] moves;
    public int[] scores;
    public int count;

    public MoveList()
    {
        moves = new int[256];
        scores = new int[256];
        count = 0;
    }
}

public struct BoardState
{
    public ulong bb0;
    public ulong bb1;
    public ulong bb2;
    public ulong bb3;
    public ulong bb4;
    public ulong bb5;
    public ulong bb6;
    public ulong bb7;
    public ulong bb8;
    public ulong bb9;
    public ulong bb10;
    public ulong bb11;

    public ulong occ0;
    public ulong occ1;
    public ulong occ2;

    public int side;
    public int enPassant;
    public int castle;
    public int halfmoveClock;

    public ulong hashKey;
}
