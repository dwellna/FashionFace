
namespace FaceRecognition
{
  /// <summary>
  /// This factory provides access to the face recognition service instance.
  /// </summary>
  public class FaceRecognitionServiceFactory
  {
    /// <summary>
    /// The only instance of the face recognition service.
    /// </summary>
    private static FaceRecognitionService _service;

    /// <summary>
    /// Retrieves and if needed, creates the face recognition service instance.
    /// </summary>
    /// <returns>The instance of the face recognition service.</returns>
    public static FaceRecognitionService GetService() => _service ?? (_service = new FaceRecognitionService());
  }
}
