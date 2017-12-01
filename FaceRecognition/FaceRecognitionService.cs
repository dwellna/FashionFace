using Microsoft.ProjectOxford.Face;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaceRecognition
{
  /// <summary>
  /// This class implements a face recognition service using the microsoft azure face recognition api.
  /// (Project Oxford)
  /// </summary>
  public class FaceRecognitionService
  {
    #region Constants
    /// <summary>
    /// The key used for authenticating to the face recognition API.
    /// </summary>
    private const string API_KEY = "0990c8bbc57c4fe99280932ffbd3e5f3";

    /// <summary>
    /// The API url.
    /// </summary>
    private const string API_ROOT = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0";
    #endregion

    #region Private Variables
    /// <summary>
    /// The face recognition api service client.
    /// </summary>
    FaceServiceClient _client;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the FaceRecognitionService class.
    /// </summary>
    /// <remarks>
    /// Access to this constructor is restricted to classes within this module.
    /// The service instance ahsll be retrieved using the appropriate factory.
    /// </remarks>
    internal FaceRecognitionService()
    {
      _client = new FaceServiceClient(API_KEY, API_ROOT);
    }
    #endregion

    /// <summary>
    /// Calls the face recognition api to detect faces in the image located on the given url.
    /// </summary>
    /// <param name="imageUrl">The url of the image to be used for face recognition.</param>
    /// <returns>
    /// A collection of face objects containing various attributes like gender.
    /// THe collection will be empty if no faces could be detected.
    /// </returns>
    public async Task<IEnumerable<Microsoft.ProjectOxford.Face.Contract.Face>> DetectFaces(string imageUrl)
    {
      return await _client.DetectAsync(imageUrl, false, false, new[] { FaceAttributeType.Gender });
    }
  }
}
