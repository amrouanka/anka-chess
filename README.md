# Anka

Anka is a custom chess engine written in C# for the UCI (Universal Chess Interface) protocol. It uses bitboards, legal move generation, alpha-beta search, evaluation heuristics, and time management to play chess positions and respond to GUI or engine-testing commands.

## Features

- Bitboard-based board representation
- Legal move generation for all standard piece types
- Chess move encoding and parsing
- FEN parsing for board setup
- Alpha-beta search with pruning and move ordering
- Transposition table and repetition tracking
- Evaluation with material, mobility, king safety, passed pawns, and piece-square tables
- UCI-compatible command loop for GUI integration
- Built-in tactical test suite for validation

## Project purpose

This project is intended to behave like a chess engine rather than a simple chess app. It can:

- accept UCI commands such as `position`, `go`, and `isready`
- search for a best move in the current position
- be connected to a chess GUI that supports UCI engines
- be used for self-testing against known tactical positions

## Requirements

- .NET SDK 10.0 or later
- Windows, Linux, or macOS supported by .NET

## Build

From the project root:

```bash
dotnet build
```

## Run

```bash
dotnet run
```

This starts the engine in UCI mode and waits for commands from stdin.

## Example UCI session

```text
uci
isready
position startpos

go depth 8
```

The engine will respond with UCI-formatted output such as `readyok` and `info` / `bestmove` lines.

## Typical commands

- `uci` – initialize engine
- `isready` – readiness check
- `ucinewgame` – reset internal state
- `position startpos` – set the standard initial board
- `position fen <fen>` – set a specific position
- `go depth 8` – search to a fixed depth
- `go movetime 1000` – search for one second
- `quit` – exit the program

## Project structure

- `Board.cs` – board state, FEN handling, occupancy, and move/state tracking
- `BitboardOperations.cs` – bitboard helpers and bit operations
- `MoveGenerator.cs` – legal move generation
- `Move.cs` – move structure and encoding helpers
- `Search.cs` – search engine, pruning, TT, and move ordering
- `Evaluation.cs` – position evaluation logic
- `TimeManagement.cs` – time-control management
- `TranspositionTable.cs` – transposition table implementation
- `Uci.cs` – UCI protocol parser and command loop
- `Program.cs` – engine entry point and tactical tests
- `Zobrist.cs` – hash key generation for repetition / transposition use

## Notes

This engine is optimized for speed and performance, with several micro-optimizations such as bitboards, flat arrays, and pruning heuristics in the search code. It is suitable for learning engine design, experimenting with evaluation tuning, and integrating with UCI-compatible tools.

## License

This project does not currently declare a license. If you plan to distribute or reuse it, add an explicit license file and terms before publication.
