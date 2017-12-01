using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FashionFace.WebApplication.Models
{
  public interface ITweetEvaluationResults : INotifyPropertyChanged
  {
    string CurrentFilter { get; set; }

    double MaleFacesPerHourCount { get; set; }

    double FemaleFacesPerHourCount { get; set; }

    double TweetRatePerHour { get; set; }

    int TweetCount { get; set; }

    ConcurrentQueue<FaceRecognitionResult> FaceRecognitionResults { get; set; }
  }
}
