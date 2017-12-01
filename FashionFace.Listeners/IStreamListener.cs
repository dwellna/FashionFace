using System;
using System.Threading.Tasks;

namespace FashionFace.Listeners
{
  /// <summary>
  /// Defines a behavior of a stream listener.
  /// </summary>
  public interface IStreamListener : IDisposable
  {
    /// <summary>
    /// Gets a value determining whether or not this instance has been disposed.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Initializes and starts this stream listener.
    /// </summary>
    /// <param name="filterValue">A value used to filter out specific messages from the stream.</param>
    /// <returns>An awaitable object for this asynchronous operation.</returns>
    Task InitializeAsync(string filterValue);
  }
}