namespace FashionFace.WebApplication.Models
{
  /// <summary>
  /// The model used for the error view.
  /// </summary>
  public class ErrorViewModel
  {
    /// <summary>
    /// Gets or sets the id of the request that lead to an error.
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// Gets a value determining whether or not to show the request id.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
  }
}