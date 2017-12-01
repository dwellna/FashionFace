using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Tweetinvi.Events;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace FashionFace.Listeners
{
  /// <summary>
  /// This class implements a twitter stream listener and provides access to observables for tweets in general
  /// and for tweets including at least one picture.
  /// </summary>
  public class TwitterStreamListener : ITwitterStreamListener
  {
    #region Constants
    /// <summary>
    /// The consumer key used for api access.
    /// </summary>
    private const string CONSUMER_KEY = "fiPFHHR1LfbjAJVfUHywn3BET";

    /// <summary>
    /// The consumer secret used for api access.
    /// </summary>
    private const string CONSUMER_SECRET = "4JRRR7t8766BaS5PLSQMlFSQe3iYAJi7wDh4Jk93dopugIL1YR";

    /// <summary>
    /// The access token used for api access.
    /// </summary>
    private const string ACCESS_TOKEN = "933611148305412096-OLeLyyl8WrhVvPOPJ6u1GJx91RFnpUX";

    /// <summary>
    /// The access token secret used for api access.
    /// </summary>
    private const string ACCESS_TOKEN_SECRET = "x5WKZxd97M0PTLHZ5XkoSRIHqEOMCFGr2NL7RQrC3n6gA";
    #endregion

    #region Private Variables
    /// <summary>
    /// The instance of the filtered twitter stream.
    /// </summary>
    private IFilteredStream _stream;

    /// <summary>
    /// The observable (push collection) of tweets.
    /// </summary>
    private IObservable<ITweet> _tweetObservable;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the TwitterStreamListener class.
    /// </summary>
    /// <remarks>
    /// Access to this constructor is restricted to classes within this module.
    /// The listener instance ahsll be retrieved using the appropriate factory.
    /// </remarks>
    internal TwitterStreamListener()
    { }
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets a value determining whether or not this instance has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets an observable (a push collection) notifying any subscriber about a newly received tweet.
    /// </summary>
    public IObservable<ITweet> TweetObservable => _tweetObservable;

    /// <summary>
    /// Gets an observable (a push collection) notifying any subscriber about a newly received tweet 
    /// containing at least one picture.
    /// </summary>
    public IObservable<ITweet> TweetWithPicturesObservable => _tweetObservable.Where(tweet => tweet.Media.Any());
    #endregion

    #region Public Methods
    /// <summary>
    /// Initializes and starts this stream listener.
    /// </summary>
    /// <param name="filterValue">A value used to filter out specific messages from the stream.</param>
    /// <returns>An awaitable object for this asynchronous operation.</returns>
    public async Task InitializeAsync(string filterValue)
    {
      if (_stream != null)
        return;

      _stream = Tweetinvi.Stream.CreateFilteredStream(
        new TwitterCredentials(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET));
      _stream.AddTrack(filterValue);

      _tweetObservable = Observable.FromEventPattern<MatchedTweetReceivedEventArgs>(_stream, nameof(_stream.MatchingTweetReceived)).
        Select(pattern => pattern.EventArgs.Tweet);

      await _stream.StartStreamMatchingAnyConditionAsync();
    }

    /// <summary>
    /// Disposes this instance and stops any listeners.
    /// </summary>
    public void Dispose()
    {
      if (IsDisposed) return;

      StopListener();
      IsDisposed = true;
      GC.SuppressFinalize(this);
    }
    #endregion

    #region Private Methods
    private void StopListener()
    {
      _stream?.StopStream();
      _stream = null;
    }
    #endregion


  }
}
