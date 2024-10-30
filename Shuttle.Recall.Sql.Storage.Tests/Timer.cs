﻿using System;
using System.Diagnostics;

namespace Shuttle.Recall.Sql.Storage.Tests;

public class Timer : IDisposable
{
    private readonly string _what;
    private readonly Stopwatch _sw = new();

    private Timer(string what)
    {
        _what = what;

        _sw.Start();
    }

    public void Dispose()
    {
        _sw.Stop();

        Console.WriteLine("[{0}] : elapsed ms = {1}", _what, _sw.ElapsedMilliseconds);
    }

    public static Timer Time(string what)
    {
        return new(what);
    }
}