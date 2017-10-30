﻿using System;
using System.IO;
using System.Threading.Tasks;
using NLog;

namespace Wikiled.YiScanner.Client.Archive
{
    public class DeleteArchiving 
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void Archive(string destination, TimeSpan time)
        {
            var files = Directory.EnumerateFiles(destination, "*", SearchOption.AllDirectories);
            DateTime cutOff = DateTime.Today.Subtract(time);
            Parallel.ForEach(files, file =>
            {
                try
                {
                    var info = new FileInfo(file);
                    if (info.CreationTime < cutOff)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            });
        }
    }
}
