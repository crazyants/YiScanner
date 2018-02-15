﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Archive;

namespace Wikiled.YiScanner.Monitoring
{
    public class MonitoringInstance
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IMonitoringConfig configuration;

        private readonly IFtpDownloader[] downloaders;

        private readonly IDeleteArchiving archiving;

        private readonly List<IDisposable> connections = new List<IDisposable>();

        private readonly IScheduler scheduler;

        public MonitoringInstance(IScheduler scheduler, IMonitoringConfig configuration, IDestinationFactory downloaderFactory, IDeleteArchiving archiving)
        {
            Guard.NotNull(() => configuration, configuration);
            Guard.NotNull(() => scheduler, scheduler);
            Guard.NotNull(() => downloaderFactory, downloaderFactory);
            Guard.NotNull(() => archiving, archiving);
            this.configuration = configuration;
            this.archiving = archiving;
            this.scheduler = scheduler;
            downloaders = downloaderFactory.GetDestinations();
        }

        public bool Start()
        {
            if (downloaders.Length == 0)
            {
                return false;
            }

            if (configuration.Archive.HasValue)
            {
                var archivingObservable = Observable.Empty<bool>()
                                          .Delay(TimeSpan.FromDays(1), scheduler)
                                          .Concat(Observable.FromAsync(Archiving, scheduler))
                                          .Repeat()
                                          .SubscribeOn(scheduler)
                                          .Subscribe();
                connections.Add(archivingObservable);
            }

            var observableMonitor = Observable.Empty<bool>()
                                    .Delay(TimeSpan.FromSeconds(configuration.Scan), scheduler)
                                    .Concat(Observable.FromAsync(Download, scheduler))
                                    .Repeat()
                                    .SubscribeOn(scheduler)
                                    .Subscribe();

            connections.Add(observableMonitor);
            return true;
        }

        public void Stop()
        {
            foreach (var connection in connections)
            {
                connection.Dispose();
            }

            connections.Clear();
        }

        private async Task<bool> Archiving()
        {
            log.Info("Archiving...");
            await archiving.Archive(configuration.Out, TimeSpan.FromDays(configuration.Archive.Value)).ConfigureAwait(false);
            log.Info("Archiving. Done!");
            return true;
        }

        private async Task<bool> Download()
        {
            log.Info("Checking Ftp....");
            try
            {
                var tasks = downloaders.Select(ftpDownloader => ftpDownloader.Download());
                await Task.WhenAll(tasks).ConfigureAwait(false);
                log.Info("Done!");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return false;
        }
    }
}
