using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FashionFace.WebApplication.Models;
using FashionFace.WebApplication.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FashionFace.WebApplication.Controllers
{
  /// <summary>
  /// This is the mvc controller class for /home.
  /// </summary>
  public class HomeController : Controller
  {
    #region Private Variables
    private static TweetEvaluationModel TweetStatisticModel { get; set; }

    private IHubContext<TweetDataHub> _hubContext;

    private static List<IStreamProcessor> StreamProcessors { get; } = new List<IStreamProcessor>();
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the HomeController class.
    /// </summary>
    /// <param name="hubContext">
    /// The hub context used for bi-directional communication with the clients.
    /// This is part of SignalR.
    /// </param>
    public HomeController(IHubContext<TweetDataHub> hubContext)
    {
      _hubContext = hubContext;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Called upon HTTP GET request to /home, initially creates an empty model containing tweet statistics
    /// and starts observing and processing the twitter stream.
    /// </summary>
    /// <returns>The index view using the current tweet statistics model.</returns>
    public IActionResult Index()
    {
      if (TweetStatisticModel == null)
      {
        TweetStatisticModel = new TweetEvaluationModel();
        StartTweetStreamProcessing();
      }

      return View(TweetStatisticModel);
    }

    /// <summary>
    /// Called upon HTTP POST request to /home/updatefilter,
    /// changes the current twitter filter value and restarts observing and processing.
    /// </summary>
    /// <returns>The index view using the newly created tweet statistics model containing the update filter value.</returns>
    [HttpPost]
    public IActionResult UpdateFilter(TweetEvaluationModel model)
    {
      TweetStatisticModel = model;
      StartTweetStreamProcessing();
      return View(nameof(Index), TweetStatisticModel);
    }

    /// <summary>
    /// Called upon HTTP GET request to /home/tweetstatistics and returns a partial view only containing the actual
    /// tweet statistic values.
    /// </summary>
    /// <returns>Returns a partial view containing tweet statistics.</returns>
    public IActionResult TweetStatistics()
    {
      return View(TweetStatisticModel);
    }

    /// <summary>
    /// Returns an error view on any invalid request.
    /// </summary>
    /// <returns>An error view.</returns>
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Starts observing and processing the twitter stream.
    /// </summary>
    private void StartTweetStreamProcessing()
    {
      // Stop any previous running processors.
      StreamProcessors?.ForEach(p => p.Stop());

      // Sent an update to the clients upon property change on the tweet statistic model.
      TweetStatisticModel.PropertyChanged += async (s, e) =>
      {
        try
        {
          await _hubContext.Clients.All.InvokeAsync("update");
        }
        catch
        {
          // exceptions to be handled.
          // Multi-Task continuation is not valid.
        }
      };

      var streamProcessor = new TwitterStreamProcessor(TweetStatisticModel);
      StreamProcessors.Add(streamProcessor);
      streamProcessor.Start();
    }
    #endregion
  }
}
