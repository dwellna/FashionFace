namespace FashionFace.Listeners
{
  /// <summary>
  /// This factory provides access to the twitter stream listener instance.
  /// </summary>
  public class StreamListenerFactory
  {
    /// <summary>
    /// The only instance of the twitter stream listener.
    /// </summary>
    private static IStreamListener _listener;

    /// <summary>
    /// Retrieves and if needed, creates the twitter stream listener instance.
    /// </summary>
    /// <returns>The instance of the twitter stream listener.</returns>
    public static IStreamListener GetTwitterListener(string filterValue)
    {
      if (IsServiceValid) return _listener;

      _listener = new TwitterStreamListener();
      _listener.InitializeAsync(filterValue);
      return _listener;
    }

    /// <summary>
    /// Gets a value determining whether the current twitter stream listener instance is valid.
    /// </summary>
    private static bool IsServiceValid => !_listener?.IsDisposed ?? false;
  }
}
