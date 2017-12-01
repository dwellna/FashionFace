using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FaceRecognition;
using FashionFace.Listeners;
using Microsoft.ProjectOxford.Face.Contract;
using Tweetinvi.Models;
using System.Threading.Tasks;

namespace FashionFace.WebApplication.Models
{
  /// <summary>
  /// This class is a stream processor implementation for using the twitter streaming api.
  /// </summary>
  public class TwitterStreamProcessor : IStreamProcessor
  {
    #region Constants
    /// <summary>
    /// The key to be used on to select tweets from the twitter stream only containing pictures.
    /// </summary>
    private const string MEDIA_TYPE_KEY = "photo";

    /// <summary>
    /// The key to be used to select only pictures containing female faces from the face recognition results.
    /// </summary>
    private const string GENDER_KEY_FEMALE = "female";

    /// <summary>
    /// The key to be used to select only pictures containing male faces from the face recognition results.
    /// </summary>
    private const string GENDER_KEY_MALE = "male";

    /// <summary>
    /// The max amount of pictures to keep for displaying from last tweets.
    /// </summary>
    private const int MAX_COUNT_DISPLAY_FACES = 10;
    #endregion

    #region Private Variables
    /// <summary>
    /// Holds the overall amount of pictures with male faces which have been detected.
    /// </summary>
    private int _malePictureTweetCount;

    /// <summary>
    /// Holds the overall amount of pictures with female faces which have been detected.
    /// </summary>
    private int _femalePictureTweetCount;

    /// <summary>
    /// An object instance used for thread synchronization.
    /// </summary>
    private readonly object _syncLock = new object();

    /// <summary>
    /// The time source for calculating the running average detection rates.
    /// </summary>
    private Stopwatch _runTimeStopWatch;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the TwitterStreamProcessor class.
    /// </summary>
    /// <param name="results"></param>
    public TwitterStreamProcessor(ITweetEvaluationResults results)
    {
      Results = results ?? throw new ArgumentNullException(nameof(results));
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the latest results tweet evaluation of this stream processor.
    /// </summary>
    public ITweetEvaluationResults Results { get; private set; }

    /// <summary>
    /// Gets or sets the listener service providing the stream to be processed.
    /// </summary>
    public IStreamListener Listener { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Starts the stream processor.
    /// </summary>
    public void Start()
    {
      Listener = StreamListenerFactory.GetTwitterListener(Results.CurrentFilter);
      ((ITwitterStreamListener) Listener).TweetObservable.Subscribe(UpdateTweetStatistics);
      ((ITwitterStreamListener) Listener).TweetWithPicturesObservable.Subscribe(UpdatePictureTweetStatistics);

      _runTimeStopWatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Stops the stream processor.
    /// </summary>
    public void Stop()
    {
      Listener?.Dispose();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Updates the current tweet statistics.
    /// </summary>
    /// <param name="tweet">The latest received tweet.</param>
    private void UpdateTweetStatistics(ITweet tweet)
    {
      Results.TweetCount++;
      UpdateAverageTweetCountPerHour();
    }

    /// <summary>
    /// Updates the average tweet count per hour.
    /// </summary>
    private void UpdateAverageTweetCountPerHour()
    {
      Results.TweetRatePerHour = _runTimeStopWatch.Elapsed.TotalHours > 1 ? 
        Results.TweetCount / _runTimeStopWatch.Elapsed.TotalHours :
        Results.TweetCount;
    }

    /// <summary>
    /// Updates the tweet statistics related to tweets including pictures.
    /// </summary>
    /// <param name="tweet">The latest received tweet.</param>
    private void UpdatePictureTweetStatistics(ITweet tweet)
    {
      var faceRecognitionService = FaceRecognitionServiceFactory.GetService();
      tweet.Media
        .Where(
          (media) => media.MediaType.Equals(MEDIA_TYPE_KEY, StringComparison.OrdinalIgnoreCase))
        .ToList().ForEach(
          async (photoMedia) => await faceRecognitionService.DetectFaces(photoMedia.MediaURLHttps)
            .ContinueWith(task => AnalyseResults(photoMedia.MediaURLHttps, task.Result), TaskContinuationOptions.OnlyOnRanToCompletion)
            .ConfigureAwait(false));
    }

    /// <summary>
    /// Analyzes face recognition results for the image to be found under the given url.
    /// </summary>
    /// <param name="imageUrl">The url of the image which has been analyzed.</param>
    /// <param name="detectedFaces">The face recognition results as collection of face classes containing several attributes.</param>
    private void AnalyseResults(string imageUrl, IEnumerable<Face> detectedFaces)
    {
      var resultFaces = detectedFaces.ToList();
      var countResult = CountFaces(resultFaces);

      var newResultData = new FaceRecognitionResult(
        imageUrl,
        countResult.totalFaceCount,
        countResult.femaleFaceCount,
        countResult.totalFaceCount - countResult.femaleFaceCount);

      StoreResult(newResultData);
    }

    /// <summary>
    /// Stores the latest face recognition results.
    /// </summary>
    /// <param name="newResultData">The results set to be stored.</param>
    private void StoreResult(FaceRecognitionResult newResultData)
    {
      Results.FaceRecognitionResults.Enqueue(newResultData);

      // Only keep the last n results for live display. Ensure by removing oldest results (FIFO).
      // Check defined constant 'MAX_COUNT_DISPLAY_FACES' to specify n.
      if (Results.FaceRecognitionResults.Count > MAX_COUNT_DISPLAY_FACES)
      {
        Results.FaceRecognitionResults.TryDequeue(out FaceRecognitionResult dequeuedResult);
      }
    }

    /// <summary>
    /// Counts the different kind of faces within the given face collection.
    /// </summary>
    /// <param name="resultFaces">The collection of faces to be considered.</param>
    /// <returns>A tuple containing the total face count and the count of female faces</returns>
    /// <remarks>The count of male faces can simply be calculated as difference between total and female face counts.</remarks>
    private (int totalFaceCount, int femaleFaceCount) CountFaces(List<Face> resultFaces)
    {
      var maleFaceCount = resultFaces.Count(face => 
        face?.FaceAttributes?.Gender.Equals(GENDER_KEY_MALE, StringComparison.OrdinalIgnoreCase) ?? false);

      var femaleFaceCount = resultFaces.Count(face => 
        face?.FaceAttributes?.Gender.Equals(GENDER_KEY_FEMALE, StringComparison.OrdinalIgnoreCase) ?? false);

      // Check if count changed to avoid unnecessary locks (less overhead)
      if (maleFaceCount > 0)
      {
        Interlocked.Add(ref _malePictureTweetCount, maleFaceCount);
        lock (_syncLock)
        {
          Results.MaleFacesPerHourCount = _runTimeStopWatch.Elapsed.TotalHours > 1 ?
            _malePictureTweetCount / _runTimeStopWatch.Elapsed.TotalHours :
            _malePictureTweetCount;
        }
      }

      // Check if count changed to avoid unnecessary locks (less overhead)
      if (femaleFaceCount > 0)
      {
        Interlocked.Add(ref _femalePictureTweetCount, femaleFaceCount);
        lock (_syncLock)
        {
          Results.FemaleFacesPerHourCount = _runTimeStopWatch.Elapsed.TotalHours > 1 ?
            _femalePictureTweetCount / _runTimeStopWatch.Elapsed.TotalHours :
            _femalePictureTweetCount;
        }
      }

      return (resultFaces.Count, femaleFaceCount);
    }
  }
  #endregion
}