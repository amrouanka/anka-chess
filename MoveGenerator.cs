using static Board;
using static CastlingRights;
using static MoveEncoding;
using static PieceAttacks;
using static Square;

public static class MoveGenerator
{
    private static readonly int[] castlingRights =
    [
        7, 15, 15, 15, 3, 15, 15, 11,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        13, 15, 15, 15, 12, 15, 15, 14
    ];

    public const int White = 0;
    public const int Black = 1;
    public const int Both = 2;

    public static void GenerateMoves(ref MoveList moveList)
    {
        moveList.count = 0;
        int sourceSquare, targetSquare;
        ulong bitboard, attacks;

        bool isWhite = side == White;
        ulong friendlyOcc = isWhite ? occupancies[White] : occupancies[Black];
        ulong enemyOcc = isWhite ? occupancies[Black] : occupancies[White];
        ulong bothOcc = occupancies[Both];
        ulong notFriendly = ~friendlyOcc;

        for (int piece = P; piece <= k; piece++)
        {
            bitboard = bitboards[piece];

            if (bitboard == 0) continue;

            if (isWhite)
            {
                if (piece == P)
                {
                    while (bitboard != 0)
                    {
                        sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                        targetSquare = sourceSquare - 8;

                        if (!(targetSquare < (int)a8) && !BitboardOperations.GetBit(bothOcc, targetSquare))
                        {
                            // pawn promotion
                            if (sourceSquare >= (int)a7 && sourceSquare <= (int)h7)
                            {
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, Q, 0, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, R, 0, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, B, 0, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, N, 0, 0, 0, 0));
                            }
                            else
                            {
                                // normal pawn move
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 0, 0, 0, 0));

                                // double push pawn move
                                if (sourceSquare >= (int)a2 && sourceSquare <= (int)h2 && !BitboardOperations.GetBit(bothOcc, targetSquare - 8))
                                    AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare - 8, piece, 0, 0, 1, 0, 0));
                            }
                        }

                        attacks = pawnAttacks[side, sourceSquare] & occupancies[Black];

                        while (attacks != 0)
                        {
                            targetSquare = BitboardOperations.GetLs1bIndex(attacks);

                            // pawn promotion capture
                            if (sourceSquare >= (int)a7 && sourceSquare <= (int)h7)
                            {
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, Q, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, R, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, B, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, N, 1, 0, 0, 0));
                            }
                            else    // normal capture
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));

                            BitboardOperations.PopBit(ref attacks, targetSquare);
                        }

                        if (enPassant != (int)noSquare)
                        {
                            ulong epAttacks = pawnAttacks[side, sourceSquare] & (1UL << enPassant);

                            if (epAttacks != 0)
                                AddMove(ref moveList, EncodeMove(sourceSquare, BitboardOperations.GetLs1bIndex(epAttacks), piece, 0, 1, 0, 1, 0));
                        }

                        BitboardOperations.PopBit(ref bitboard, sourceSquare);
                    }
                }

                // White castling
                if (piece == K)
                {
                    if ((castle & (int)wk) != 0 && !BitboardOperations.GetBit(bothOcc, (int)f1) && !BitboardOperations.GetBit(bothOcc, (int)g1) && !IsSquareAttacked((int)e1, Black) && !IsSquareAttacked((int)f1, Black))
                        AddMove(ref moveList, EncodeMove((int)e1, (int)g1, piece, 0, 0, 0, 0, 1));

                    if ((castle & (int)wq) != 0 && !BitboardOperations.GetBit(bothOcc, (int)d1) && !BitboardOperations.GetBit(bothOcc, (int)c1) && !BitboardOperations.GetBit(bothOcc, (int)b1) && !IsSquareAttacked((int)e1, Black) && !IsSquareAttacked((int)d1, Black))
                        AddMove(ref moveList, EncodeMove((int)e1, (int)c1, piece, 0, 0, 0, 0, 1));
                }
            }
            else
            {
                if (piece == p)
                {
                    while (bitboard != 0)
                    {
                        sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                        targetSquare = sourceSquare + 8;

                        if (!(targetSquare > (int)h1) && !BitboardOperations.GetBit(bothOcc, targetSquare))
                        {
                            // pawn promotion
                            if (sourceSquare >= (int)a2 && sourceSquare <= (int)h2)
                            {
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, q, 0, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, r, 0, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, b, 0, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, n, 0, 0, 0, 0));
                            }
                            else
                            {
                                // normal pawn move
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 0, 0, 0, 0));

                                // double pawn move
                                if (sourceSquare >= (int)a7 && sourceSquare <= (int)h7 && !BitboardOperations.GetBit(bothOcc, targetSquare + 8))
                                    AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare + 8, piece, 0, 0, 1, 0, 0));
                            }
                        }

                        attacks = pawnAttacks[side, sourceSquare] & occupancies[White];
                        while (attacks != 0)
                        {
                            targetSquare = BitboardOperations.GetLs1bIndex(attacks);

                            if (sourceSquare >= (int)a2 && sourceSquare <= (int)h2)
                            {
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, q, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, r, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, b, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, n, 1, 0, 0, 0));
                            }
                            else
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));

                            BitboardOperations.PopBit(ref attacks, targetSquare);
                        }

                        if (enPassant != (int)noSquare)
                        {
                            ulong epAttacks = pawnAttacks[side, sourceSquare] & (1UL << enPassant);

                            if (epAttacks != 0)
                                AddMove(ref moveList, EncodeMove(sourceSquare, BitboardOperations.GetLs1bIndex(epAttacks), piece, 0, 1, 0, 1, 0));
                        }

                        BitboardOperations.PopBit(ref bitboard, sourceSquare);
                    }
                }

                if (piece == k)
                {
                    if ((castle & (int)bk) != 0 && !BitboardOperations.GetBit(bothOcc, (int)f8) && !BitboardOperations.GetBit(bothOcc, (int)g8) && !IsSquareAttacked((int)e8, White) && !IsSquareAttacked((int)f8, White))
                        AddMove(ref moveList, EncodeMove((int)e8, (int)g8, piece, 0, 0, 0, 0, 1));

                    if ((castle & (int)bq) != 0 && !BitboardOperations.GetBit(bothOcc, (int)d8) && !BitboardOperations.GetBit(bothOcc, (int)c8) && !BitboardOperations.GetBit(bothOcc, (int)b8) && !IsSquareAttacked((int)e8, White) && !IsSquareAttacked((int)d8, White))
                        AddMove(ref moveList, EncodeMove((int)e8, (int)c8, piece, 0, 0, 0, 0, 1));
                }
            }

            if (isWhite ? piece == N : piece == n)
            {
                while (bitboard != 0)
                {
                    sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                    attacks = knightAttacks[sourceSquare] & notFriendly;

                    while (attacks != 0)
                    {
                        targetSquare = BitboardOperations.GetLs1bIndex(attacks);

                        // quite move
                        if (!BitboardOperations.GetBit(enemyOcc, targetSquare))
                            AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 0, 0, 0, 0));

                        // capture move
                        else
                            AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));

                        BitboardOperations.PopBit(ref attacks, targetSquare);
                    }

                    BitboardOperations.PopBit(ref bitboard, sourceSquare);
                }
            }

            if (isWhite ? piece == B : piece == b)
            {
                while (bitboard != 0)
                {
                    sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                    attacks = GetBishopAttacks(sourceSquare, bothOcc) & notFriendly;

                    while (attacks != 0)
                    {
                        targetSquare = BitboardOperations.GetLs1bIndex(attacks);

                        if (!BitboardOperations.GetBit(enemyOcc, targetSquare))
                            AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 0, 0, 0, 0));
                        else
                            AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));

                        BitboardOperations.PopBit(ref attacks, targetSquare);
                    }

                    BitboardOperations.PopBit(ref bitboard, sourceSquare);
                }
            }

            if (isWhite ? piece == R : piece == r)
            {
                while (bitboard != 0)
                {
                    sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                    attacks = GetRookAttacks(sourceSquare, bothOcc) & notFriendly;

                    while (attacks != 0)
                    {
                        targetSquare = BitboardOperations.GetLs1bIndex(attacks);

                        if (!BitboardOperations.GetBit(enemyOcc, targetSquare))
                            AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 0, 0, 0, 0));
                        else
                            AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));

                        BitboardOperations.PopBit(ref attacks, targetSquare);
                    }

                    BitboardOperations.PopBit(ref bitboard, sourceSquare);
                }
            }

            if (isWhite ? piece == Q : piece == q)
            {
                while (bitboard != 0)
                {
                    sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                    attacks = GetQueenAttacks(sourceSquare, bothOcc) & notFriendly;

                    while (attacks != 0)
                    {
                        targetSquare = BitboardOperations.GetLs1bIndex(attacks);

                        if (!BitboardOperations.GetBit(enemyOcc, targetSquare))
                            AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 0, 0, 0, 0));
                        else
                            AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));

                        BitboardOperations.PopBit(ref attacks, targetSquare);
                    }

                    BitboardOperations.PopBit(ref bitboard, sourceSquare);
                }
            }

            if (isWhite ? piece == K : piece == k)
            {
                while (bitboard != 0)
                {
                    sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                    attacks = kingAttacks[sourceSquare] & notFriendly;

                    while (attacks != 0)
                    {
                        targetSquare = BitboardOperations.GetLs1bIndex(attacks);

                        if (!BitboardOperations.GetBit(enemyOcc, targetSquare))
                            AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 0, 0, 0, 0));
                        else
                            AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));

                        BitboardOperations.PopBit(ref attacks, targetSquare);
                    }

                    BitboardOperations.PopBit(ref bitboard, sourceSquare);
                }
            }
        }
    }

    public static void GenerateCaptureMoves(ref MoveList moveList)
    {
        moveList.count = 0;
        int sourceSquare, targetSquare;
        ulong bitboard, attacks;

        bool isWhite = side == White;
        ulong friendlyOcc = isWhite ? occupancies[White] : occupancies[Black];
        ulong enemyOcc = isWhite ? occupancies[Black] : occupancies[White];
        ulong bothOcc = occupancies[Both];
        ulong notFriendly = ~friendlyOcc;

        for (int piece = P; piece <= k; piece++)
        {
            bitboard = bitboards[piece];

            if (bitboard == 0) continue;

            if (isWhite)
            {
                if (piece == P)
                {
                    while (bitboard != 0)
                    {
                        sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);

                        attacks = pawnAttacks[side, sourceSquare] & occupancies[Black];

                        while (attacks != 0)
                        {
                            targetSquare = BitboardOperations.GetLs1bIndex(attacks);

                            // pawn promotion capture
                            if (sourceSquare >= (int)a7 && sourceSquare <= (int)h7)
                            {
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, Q, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, R, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, B, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, N, 1, 0, 0, 0));
                            }
                            else    // normal capture
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));

                            BitboardOperations.PopBit(ref attacks, targetSquare);
                        }

                        if (enPassant != (int)noSquare)
                        {
                            ulong epAttacks = pawnAttacks[side, sourceSquare] & (1UL << enPassant);

                            if (epAttacks != 0)
                                AddMove(ref moveList, EncodeMove(sourceSquare, BitboardOperations.GetLs1bIndex(epAttacks), piece, 0, 1, 0, 1, 0));
                        }

                        BitboardOperations.PopBit(ref bitboard, sourceSquare);
                    }
                }
            }
            else
            {
                if (piece == p)
                {
                    while (bitboard != 0)
                    {
                        sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);

                        attacks = pawnAttacks[side, sourceSquare] & occupancies[White];
                        while (attacks != 0)
                        {
                            targetSquare = BitboardOperations.GetLs1bIndex(attacks);

                            if (sourceSquare >= (int)a2 && sourceSquare <= (int)h2)
                            {
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, q, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, r, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, b, 1, 0, 0, 0));
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, n, 1, 0, 0, 0));
                            }
                            else
                                AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));

                            BitboardOperations.PopBit(ref attacks, targetSquare);
                        }

                        if (enPassant != (int)noSquare)
                        {
                            ulong epAttacks = pawnAttacks[side, sourceSquare] & (1UL << enPassant);

                            if (epAttacks != 0)
                                AddMove(ref moveList, EncodeMove(sourceSquare, BitboardOperations.GetLs1bIndex(epAttacks), piece, 0, 1, 0, 1, 0));
                        }

                        BitboardOperations.PopBit(ref bitboard, sourceSquare);
                    }
                }
            }

            if (isWhite ? piece == N : piece == n)
            {
                while (bitboard != 0)
                {
                    sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                    attacks = knightAttacks[sourceSquare] & enemyOcc;

                    while (attacks != 0)
                    {
                        targetSquare = BitboardOperations.GetLs1bIndex(attacks);
                        AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));
                        BitboardOperations.PopBit(ref attacks, targetSquare);
                    }

                    BitboardOperations.PopBit(ref bitboard, sourceSquare);
                }
            }

            if (isWhite ? piece == B : piece == b)
            {
                while (bitboard != 0)
                {
                    sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                    attacks = GetBishopAttacks(sourceSquare, bothOcc) & enemyOcc;

                    while (attacks != 0)
                    {
                        targetSquare = BitboardOperations.GetLs1bIndex(attacks);
                        AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));
                        BitboardOperations.PopBit(ref attacks, targetSquare);
                    }

                    BitboardOperations.PopBit(ref bitboard, sourceSquare);
                }
            }

            if (isWhite ? piece == R : piece == r)
            {
                while (bitboard != 0)
                {
                    sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                    attacks = GetRookAttacks(sourceSquare, bothOcc) & enemyOcc;

                    while (attacks != 0)
                    {
                        targetSquare = BitboardOperations.GetLs1bIndex(attacks);
                        AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));
                        BitboardOperations.PopBit(ref attacks, targetSquare);
                    }

                    BitboardOperations.PopBit(ref bitboard, sourceSquare);
                }
            }

            if (isWhite ? piece == Q : piece == q)
            {
                while (bitboard != 0)
                {
                    sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                    attacks = GetQueenAttacks(sourceSquare, bothOcc) & enemyOcc;

                    while (attacks != 0)
                    {
                        targetSquare = BitboardOperations.GetLs1bIndex(attacks);
                        AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));
                        BitboardOperations.PopBit(ref attacks, targetSquare);
                    }

                    BitboardOperations.PopBit(ref bitboard, sourceSquare);
                }
            }

            if (isWhite ? piece == K : piece == k)
            {
                while (bitboard != 0)
                {
                    sourceSquare = BitboardOperations.GetLs1bIndex(bitboard);
                    attacks = kingAttacks[sourceSquare] & enemyOcc;

                    while (attacks != 0)
                    {
                        targetSquare = BitboardOperations.GetLs1bIndex(attacks);
                        AddMove(ref moveList, EncodeMove(sourceSquare, targetSquare, piece, 0, 1, 0, 0, 0));
                        BitboardOperations.PopBit(ref attacks, targetSquare);
                    }

                    BitboardOperations.PopBit(ref bitboard, sourceSquare);
                }
            }
        }
    }

    public static void AddMove(ref MoveList moveList, int move)
    {
        moveList.moves[moveList.count] = move;
        moveList.count++;
    }

    public static int MakeMove(int move, int moveFlag)
    {
        bool isWhite = side == White;

        // Remove the old en passant from hash (if it existed)
        if (enPassant != (int)noSquare)
            Zobrist.hashKey ^= Zobrist.enpassantKeys[enPassant];

        // Then clear it
        enPassant = (int)noSquare;

        if (moveFlag == (int)MoveFlag.allMoves)
        {
            int piece = GetMovePiece(move);
            int capture = GetMoveCapture(move);

            // 50-move rule clock:
            // reset on pawn move or capture, otherwise increment
            if (piece == P || piece == p || capture != 0)
                halfmoveClock = 0;
            else
                halfmoveClock++;

            int sourceSquare = GetMoveSource(move);
            int targetSquare = GetMoveTarget(move);

            BitboardOperations.PopBit(ref bitboards[piece], sourceSquare);
            BitboardOperations.SetBit(ref bitboards[piece], targetSquare);

            Zobrist.hashKey ^= Zobrist.pieceKeys[piece, sourceSquare];    // remove piece from source square in hash key
            Zobrist.hashKey ^= Zobrist.pieceKeys[piece, targetSquare];    // set piece to the target square in hash key

            if (capture != 0)
            {
                int startPiece = isWhite ? p : P;
                int endPiece = isWhite ? k : K;

                for (int bbPiece = startPiece; bbPiece <= endPiece; bbPiece++)
                {
                    if (BitboardOperations.GetBit(bitboards[bbPiece], targetSquare))
                    {
                        BitboardOperations.PopBit(ref bitboards[bbPiece], targetSquare);
                        Zobrist.hashKey ^= Zobrist.pieceKeys[bbPiece, targetSquare];
                        break;
                    }
                }
            }

            int promotedPiece = GetMovePromoted(move);
            if (promotedPiece != 0)
            {
                if (isWhite)
                {
                    BitboardOperations.PopBit(ref bitboards[P], targetSquare);
                    Zobrist.hashKey ^= Zobrist.pieceKeys[P, targetSquare];  // remove pawn from target square in hash key
                }
                else
                {
                    BitboardOperations.PopBit(ref bitboards[p], targetSquare);
                    Zobrist.hashKey ^= Zobrist.pieceKeys[p, targetSquare];  // remove pawn from target square in hash key
                }

                BitboardOperations.SetBit(ref bitboards[promotedPiece], targetSquare);
                Zobrist.hashKey ^= Zobrist.pieceKeys[promotedPiece, targetSquare];  // add promoted piece to target square in hash key
            }

            if (GetMoveEnpassant(move) != 0)
            {
                // white to move
                if (isWhite)
                {
                    // remove captured pawn
                    BitboardOperations.PopBit(ref bitboards[p], targetSquare + 8);

                    // remove pawn from hash key
                    Zobrist.hashKey ^= Zobrist.pieceKeys[p, targetSquare + 8];
                }

                // black to move
                else
                {
                    // remove captured pawn
                    BitboardOperations.PopBit(ref bitboards[P], targetSquare - 8);

                    // remove pawn from hash key
                    Zobrist.hashKey ^= Zobrist.pieceKeys[P, targetSquare - 8];
                }
            }

            int doublePush = GetMoveDouble(move);
            if (doublePush != 0)
            {
                if (isWhite)
                {
                    enPassant = targetSquare + 8;

                    // hash enpassant
                    Zobrist.hashKey ^= Zobrist.enpassantKeys[targetSquare + 8];
                }
                else
                {
                    enPassant = targetSquare - 8;

                    // hash enpassant
                    Zobrist.hashKey ^= Zobrist.enpassantKeys[targetSquare - 8];
                }
            }

            if (GetMoveCastling(move) != 0)
            {
                switch ((Square)targetSquare)
                {
                    case g1:
                        BitboardOperations.PopBit(ref bitboards[R], (int)h1);
                        BitboardOperations.SetBit(ref bitboards[R], (int)f1);
                        // hash rook
                        Zobrist.hashKey ^= Zobrist.pieceKeys[R, (int)h1];  // remove rook from h1 from hash key
                        Zobrist.hashKey ^= Zobrist.pieceKeys[R, (int)f1];  // put rook on f1 into a hash key
                        break;
                    case c1:
                        BitboardOperations.PopBit(ref bitboards[R], (int)a1);
                        BitboardOperations.SetBit(ref bitboards[R], (int)d1);
                        Zobrist.hashKey ^= Zobrist.pieceKeys[R, (int)a1];  // remove rook from a1 from hash key
                        Zobrist.hashKey ^= Zobrist.pieceKeys[R, (int)d1];  // put rook on d1 into a hash key
                        break;
                    case g8:
                        BitboardOperations.PopBit(ref bitboards[r], (int)h8);
                        BitboardOperations.SetBit(ref bitboards[r], (int)f8);
                        Zobrist.hashKey ^= Zobrist.pieceKeys[r, (int)h8];  // remove rook from h8 from hash key
                        Zobrist.hashKey ^= Zobrist.pieceKeys[r, (int)f8];  // put rook on f8 into a hash key
                        break;
                    case c8:
                        BitboardOperations.PopBit(ref bitboards[r], (int)a8);
                        BitboardOperations.SetBit(ref bitboards[r], (int)d8);
                        Zobrist.hashKey ^= Zobrist.pieceKeys[r, (int)a8];  // remove rook from a8 from hash key
                        Zobrist.hashKey ^= Zobrist.pieceKeys[r, (int)d8];  // put rook on d8 into a hash key
                        break;
                }
            }
            Zobrist.hashKey ^= Zobrist.castleKeys[castle];  // hash castling

            castle &= castlingRights[sourceSquare];
            castle &= castlingRights[targetSquare];

            Zobrist.hashKey ^= Zobrist.castleKeys[castle];  // hash castling

            UpdateOccupancies();

            side ^= 1;
            Zobrist.hashKey ^= Zobrist.sideKey;

            int kingSquare = isWhite
                ? BitboardOperations.GetLs1bIndex(bitboards[K])
                : BitboardOperations.GetLs1bIndex(bitboards[k]);

            if (IsSquareAttacked(kingSquare, side))
                return 0;

            return 1;
        }
        else
        {
            if (GetMoveCapture(move) == 0)
                return 0;

            return MakeMove(move, (int)MoveFlag.allMoves);
        }
    }
}
