﻿using System;
using System.Collections.Concurrent;

namespace Wikiled.YiScanner.Client
{
    public class Analyzer : IAnalyzer
    {
        private readonly ConcurrentDictionary<string, bool> table = new ConcurrentDictionary<string, bool>();

        public bool CanDownload(DateTime? lastScan, string file, DateTime modified)
        {
            if (table.ContainsKey(file))
            {
                return false;
            }

            table[file] = true;
            return lastScan != null;
        }
    }
}