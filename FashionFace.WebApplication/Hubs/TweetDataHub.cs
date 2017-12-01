using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FashionFace.WebApplication.Hubs
{
  /// <summary>
  /// This SignalR hub is used to notify any client about updated tweet statistics.
  /// </summary>
  /// <remarks>
  /// This class does not have an implementation as I could not figure out a way to retrieve the current hub instance from the controller class.
  /// Instead I am retrieving a hub context for this type where I can notify the clients.
  /// </remarks>
  public class TweetDataHub : Hub
  {
    //public async Task Update(int maleFaceCount, int femaleFaceCount, int tweetCount)
    //{
    //  await Clients.All.InvokeAsync("Update", maleFaceCount, femaleFaceCount, tweetCount);
    //}
  }
}
