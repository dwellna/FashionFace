using System;
using Tweetinvi.Models;

namespace FashionFace.Listeners
{
  /// <summary>
  /// Defines an extended stream listener behavior for the twitter stream.
  /// </summary>
  public interface ITwitterStreamListener : IStreamListener
  {
    /// <summary>
    /// Gets an observable (a push collection) notifying any subscriber about a newly received tweet.
    /// </summary>
    IObservable<ITweet> TweetObservable { get; }

    /// <summary>
    /// Gets an observable (a push collection) notifying any subscriber about a newly received tweet 
    /// containing at least one picture.
    /// </summary>
    IObservable<ITweet> TweetWithPicturesObservable { get; }
  }
}