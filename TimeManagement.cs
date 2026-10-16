using System;

public static class TimeManagement
{
    // UCI time controls
    public static int movetime = -1;
    public static int time = -1;
    public static int inc = 0;
    public static int movestogo = 30; // parsed but not really used

    // Search state
    public static bool timeset = false;
    public static bool stopped = false;
    public static bool quit = false;

    // Timing points
    public static long starttime = 0;
    public static long softStopTime = 0; // stop after finished iteration
    public static long stoptime = 0;     // hard stop inside search

    public static long GetTimeMs()
    {
        return Environment.TickCount64;
    }

    public static void ResetForGo()
    {
        movetime = -1;
        time = -1;
        inc = 0;
        movestogo = 30;

        timeset = false;
        stopped = false;

        starttime = 0;
        softStopTime = 0;
        stoptime = 0;
    }

    public static void StartInfiniteSearch()
    {
        starttime = GetTimeMs();
        timeset = false;
        stopped = false;
        softStopTime = 0;
        stoptime = 0;
    }

    public static void StartMoveTimeSearch(int moveTimeMs)
    {
        starttime = GetTimeMs();
        timeset = true;
        stopped = false;

        int spend = Math.Max(1, moveTimeMs - 20);

        softStopTime = starttime + spend;
        stoptime = starttime + spend;
    }

    // Very simple and conservative clock management
    public static void StartClockSearch(int remainingMs, int incrementMs)
    {
        starttime = GetTimeMs();
        timeset = true;
        stopped = false;

        remainingMs = Math.Max(1, remainingMs);
        incrementMs = Math.Max(0, incrementMs);

        // Conservative formula:
        // spend a small fraction of remaining time + half increment
        int spend = remainingMs / 40 + incrementMs / 2;

        // Never think too little
        spend = Math.Max(10, spend);

        // Never spend too much on one move
        spend = Math.Min(spend, remainingMs / 8);

        // Emergency low-time mode
        if (remainingMs < 2000)
            spend = Math.Min(spend, Math.Max(10, remainingMs / 5));

        // Small safety margin
        spend = Math.Min(spend, Math.Max(1, remainingMs - 30));

        softStopTime = starttime + spend;
        stoptime = softStopTime + 10; // tiny hard-stop cushion
    }

    public static void Communicate()
    {
        if (timeset && GetTimeMs() >= stoptime)
            stopped = true;
    }

    public static bool ShouldStopAfterIteration()
    {
        return timeset && GetTimeMs() >= softStopTime;
    }
}