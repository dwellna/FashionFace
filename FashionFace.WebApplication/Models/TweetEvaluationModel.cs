using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FashionFace.WebApplication.Models
{
  /// <summary>
  /// This class is a MVC model providing tweet statistics to a view.
  /// </summary>
  public class TweetEvaluationModel : ITweetEvaluationResults
  {
    #region Constants
    /// <summary>
    /// Default value for the twitter filter value.
    /// </summary>
    private const string FILTER_VALUE = "#FashionFace";
    #endregion

    #region Private Variables
    /// <summary>
    /// The current filter value configured for the twitter stream processor.
    /// </summary>
    private string _currentFilter;

    /// <summary>
    /// The current amount of male faces detected.
    /// </summary>
    private double _maleFacesPerHourCount;

    /// <summary>
    /// The current amount of female faces detected.
    /// </summary>
    private double _femaleFacesPerHourCount;

    /// <summary>
    /// The current amount of tweets per hour.
    /// </summary>
    private double _tweetRatePerHour;

    /// <summary>
    /// The current total amount of received and filtered tweets.
    /// </summary>
    private int _tweetCount;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the TweetEvaluationModel class.
    /// </summary>
    public TweetEvaluationModel()
    {
      FaceRecognitionResults = new ConcurrentQueue<FaceRecognitionResult>();
      CurrentFilter = FILTER_VALUE;
    }
    #endregion

    /// <summary>
    /// Gets or sets the current twitter filter value.
    /// </summary>
    [Display(Name = "Twitter Filter")]
    public string CurrentFilter
    {
      get => _currentFilter;
      set
      {
        _currentFilter = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentFilter)));
      }
    }

    /// <summary>
    /// Gets or sets the amount of detected male faces per hour as a running average.
    /// </summary>
    public double MaleFacesPerHourCount
    {
      get => _maleFacesPerHourCount;
      set
      {
        _maleFacesPerHourCount = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaleFacesPerHourCount)));
      }
    }

    /// <summary>
    /// Gets or sets the amount of detected female faces per hour as a running average.
    /// </summary>
    public double FemaleFacesPerHourCount
    {
      get => _femaleFacesPerHourCount;
      set
      {
        _femaleFacesPerHourCount = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FemaleFacesPerHourCount)));
      }
    }

    /// <summary>
    /// Gets or sets the amount of tweets per hour as a running average.
    /// </summary>
    public double TweetRatePerHour
    {
      get => _tweetRatePerHour;
      set
      {
        _tweetRatePerHour = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TweetRatePerHour)));
      }
    }

    /// <summary>
    /// Gets the overall amount of tweets matching the given filter value since start of listening.
    /// </summary>
    public int TweetCount
    {
      get => _tweetCount;
      set
      {
        _tweetCount = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TweetCount)));
      }
    }

    /// <summary>
    /// Gets or sets the queue to store the results from the face recognition service.
    /// </summary>
    public ConcurrentQueue<FaceRecognitionResult> FaceRecognitionResults { get; set; }

    #region Events
    /// <summary>
    /// Notifies subscriber about property value changes within this instance.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion
  }
}
