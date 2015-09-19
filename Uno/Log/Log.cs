using System;
using System.Collections.Generic;
using System.Linq;

namespace Uno.Log
{
    public class Log
    {
        private List<ILogEntry> _entries = new List<ILogEntry>();

        internal void AddEntry(ILogEntry entry)
        {
            _entries.Add(entry);
            Loged?.Invoke(this, new LogEventArgs { Entry = entry });
        }

        public IReadOnlyList<ILogEntry> Entries => _entries;
        public string TotalDescription => String.Join("\n", _entries.Select(e => e.Description));

        public static EventHandler<LogEventArgs> Loged;
    }

    public class LogEventArgs : EventArgs
    {
        public ILogEntry Entry { get; internal set; }
    }
}