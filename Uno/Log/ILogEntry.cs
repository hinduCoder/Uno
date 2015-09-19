using Uno.Model;

namespace Uno.Log
{
    public interface ILogEntry
    {
        Player Player { get; }
        string Description { get; }
    }
}