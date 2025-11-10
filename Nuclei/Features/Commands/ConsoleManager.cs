using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BepInEx.Logging;

namespace Nuclei.Features.Commands
{
    internal sealed class ConsoleManager : IDisposable
    {
        private readonly SynchronizationContext _unityCtx;
        private readonly Action<string, string[]> _onCommand;
        private CancellationTokenSource? _cts;
        private Thread? _thread;

        public ConsoleManager(SynchronizationContext unityCtx, Action<string, string[]> onCommand)
        {
            _unityCtx = unityCtx ?? throw new ArgumentNullException(nameof(unityCtx));
            _onCommand = onCommand ?? throw new ArgumentNullException(nameof(onCommand));
        }

        public void Start()
        {
            if (_thread != null) return;

            _cts = new CancellationTokenSource();
            _thread = new Thread(() => StdinLoop(_cts.Token))
            {
                IsBackground = true,
                Name = "Nuclei-StdInReader"
            };
            _thread.Start();
            Nuclei.Logger?.LogInfo("ConsoleManager: STDIN reader started.");
        }

        public void Stop()
        {
            try
            {
                _cts?.Cancel();
                if (_thread != null && _thread.IsAlive)
                    _thread.Join(250); 
            }
            catch { /* swallow */ }
            finally
            {
                _thread = null;
                _cts = null;
                Nuclei.Logger?.LogInfo("ConsoleManager: STDIN reader stopped.");
            }
        }

        private void StdinLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    
                    var line = Console.ReadLine();
                    if (line == null)
                    {
                        Nuclei.Logger?.LogWarning("ConsoleManager: STDIN closed (ReadLine returned null). Stopping.");
                        break;
                    }

                    line = line.Trim();
                    if (line.Length == 0)
                        continue;

                    var args = SplitArgs(line);
                    
                    _unityCtx.Post(_ => _onCommand(line, args), null);
                }
            }
            catch (Exception ex)
            {
                Nuclei.Logger?.LogError($"ConsoleManager: STDIN loop crashed: {ex}");
            }
        }

        public void Dispose() => Stop();

        private static string[] SplitArgs(string input)
        {
            // Matches "quoted strings" or bare tokens
            var matches = Regex.Matches(input, "\"([^\"]*)\"|([^\\s]+)");
            return matches.Cast<Match>()
                   .Select(m => m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value)
                   .ToArray();
        }
    }
}
