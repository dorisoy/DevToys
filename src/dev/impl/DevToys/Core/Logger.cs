﻿#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DevToys.Core.Threading;
using DevToys.Shared.Core.Threading;
using Windows.Storage;
using Windows.System;

namespace DevToys.Core
{
    internal static class Logger
    {
        private const string LogFileName = "logs.txt";

        private static readonly DisposableSempahore _semaphore = new();

        internal static async Task OpenLogsAsync()
        {
            using (await _semaphore.WaitAsync(CancellationToken.None))
            {
                StorageFolder localCacheFolder = ApplicationData.Current.LocalCacheFolder;

                IStorageItem file = await localCacheFolder.TryGetItemAsync(LogFileName);

                if (file is null)
                {
                    file = await localCacheFolder.CreateFileAsync(LogFileName, CreationCollisionOption.OpenIfExists);
                }

                if (file is not null and IStorageFile storageFile)
                {
                    await ThreadHelper.RunOnUIThreadAsync(async () =>
                    {
                        await Launcher.LaunchFileAsync(storageFile);
                    });
                }
            }
        }

        internal static void Log(string featureName, string message)
        {
            lock (_semaphore)
            {
                LogFaultAsync(featureName, null, message).ForgetSafely();
            }
        }

        internal static void LogFault(string featureName, Exception ex, string? message = null)
        {
            lock (_semaphore)
            {
                LogFaultAsync(featureName, ex, message).ForgetSafely();
            }
        }

        private static async Task LogFaultAsync(string featureName, Exception? ex, string? message)
        {
            await TaskScheduler.Default;

            try
            {
                using (await _semaphore.WaitAsync(CancellationToken.None))
                {
                    StorageFolder localCacheFolder = ApplicationData.Current.LocalCacheFolder;

                    StorageFile logFile = await localCacheFolder.CreateFileAsync(LogFileName, CreationCollisionOption.OpenIfExists);

                    if (ex is null)
                    {
                        var logsLine = new List<string>
                        {
                            $"# - [{DateTime.Now.ToString("G", DateTimeFormatInfo.InvariantInfo)}]",
                            $"Feature name: {featureName}",
                            $"Custom message: {message ?? string.Empty}",
                            string.Empty // empty line
                        };

                        await FileIO.AppendLinesAsync(logFile, logsLine);
                    }
                    else
                    {
                        var logsLine = new List<string>
                        {
                            $"# - [{DateTime.Now.ToString("G", DateTimeFormatInfo.InvariantInfo)}]",
                            $"Feature name: {featureName}",
                            $"Custom message: {message ?? string.Empty}",
                            $"Exception message: {ex.Message}",
                            $"Exception stack trace:",
                            ex.StackTrace,
                            string.Empty // empty line
                        };

                        await FileIO.AppendLinesAsync(logFile, logsLine);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
