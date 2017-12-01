using FashionFace.Listeners;

namespace FashionFace.WebApplication.Models
{
  /// <summary>
  /// Defines a behavior or a message stream processor.
  /// </summary>
  public interface IStreamProcessor
  {
    /// <summary>
    /// Starts the stream processor.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the stream processor.
    /// </summary>
    void Stop();

    /// <summary>
    /// Gets or sets the stream listener instance.
    /// </summary>
    IStreamListener Listener { get; set; }
  }
}