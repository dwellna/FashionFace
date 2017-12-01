using System;

namespace FashionFace.WebApplication.Models
{
  /// <summary>
  /// The processed results of a face recognition request, which relates to a single image url.
  /// </summary>
  public class FaceRecognitionResult
  {
    /// <summary>
    /// Initializes a new instance of the FaceRecognitionResult class.
    /// </summary>
    /// <param name="sourceImageUrl">The image url.</param>
    /// <param name="totalFaceCount">The total amount of faces detected.</param>
    /// <param name="femaleFaceCount">The amount of female faces detected.</param>
    /// <param name="maleFaceCount">The amount of male faces detected.</param>
    public FaceRecognitionResult(string sourceImageUrl, int totalFaceCount, int femaleFaceCount, int maleFaceCount)
    {
      SourceImageUrl = new Uri(sourceImageUrl, UriKind.Absolute);
      TotalFaceCount = totalFaceCount;
      FemaleFaceCount = femaleFaceCount;
      MaleFaceCount = maleFaceCount;
    }

    /// <summary>
    /// Gets the total amount of detected faces.
    /// </summary>
    public int TotalFaceCount { get; }

    /// <summary>
    /// Gets the amount of female faces detected.
    /// </summary>
    public int FemaleFaceCount { get; }

    /// <summary>
    /// Gets the amount of male faces detected.
    /// </summary>
    public int MaleFaceCount { get; }

    /// <summary>
    /// Gets the url of the image used to get this results.
    /// </summary>
    public Uri SourceImageUrl { get; }

  }
}
