namespace WillowV2.Models
{
    public class GoalPrediction
    {
        public DateTime? PredictionDate {  get; set; } //when we think the goal will be reached
        public bool OnTrack { get; set; } //is predicted date before deadline
        public string Status { get; set; } //text: On track, Behind etc.
        public decimal RequiredDailyRate { get; set; }
        public string Confidence { get; set; } //consistency 
        public decimal WeightedDailyRate { get; set; } //calcualted daily saving rate

        public static GoalPrediction Calculate(Goal goal)
        {
            var prediction = new GoalPrediction();
            //check if the goal is reached
            if (goal.CurrentAmount >= goal.AmountToReach){
                prediction.Status = "Goal reached!";
                prediction.OnTrack = true;
                prediction.Confidence = "High";

                return prediction;
            }         
            //days that have passed
            var totalDays = (DateTime.Today - goal.CurrentDate.Date).Days;

            //check if there is enough data
            if (goal.CurrentAmount <=0 || totalDays <= 0)
            {
                prediction.Status = "Not enough data!";
                prediction.Confidence = "Unknown yet";
                return prediction;
            }
            //how much on avrg have u saved per day so far
            //rate: dividing total amnt saved up by the days that have passed
            //example: on avrg u have saved 5$ per day
            var overallDailyRate = goal.CurrentAmount / totalDays;

            //recent daily rate, using the last 30 days of data
            var recentDays = Math.Min(totalDays, 30);
            //estimate how much of the total amount belongs to the past 30 days
            var recentProgress = goal.CurrentAmount * (recentDays / totalDays);
            //recent daily rate: past 30 days
            var recentDailyRates = recentProgress / recentDays;
            //recent daily rate has 70% impact while overall 30%
            var weightedDailyRate = (recentDailyRates * 0.7) + (overallDailyRate * 0.3);

            prediction.WeightedDailyRate = (decimal)weightedDailyRate;
            //remainder of needed amount
            var remaining = goal.AmountToReach - goal.CurrentAmount;
            //if u havent saved anything yet
            if (weightedDailyRate <= 0)
            {
                prediction.Status = "Not saving currently!";
                prediction.Confidence = "Low";
                return prediction;
            }

            //predicted needed days
            var daysNeeded = remaining / weightedDailyRate;
            
            //overall vs recent day difference
            var difference = Math.Abs(recentDailyRates - overallDailyRate);
            var percentDifference = overallDailyRate > 0 ? (difference / overallDailyRate) * 100 : 0;
            if (daysNeeded > 36500) // more than 100 years out
            {
                prediction.PredictionDate = null;
                prediction.OnTrack = false;
                prediction.Status = "Behind schedule";
            }
            else
            {
                prediction.PredictionDate = DateTime.Today.AddDays((double)daysNeeded);
                prediction.OnTrack = prediction.PredictionDate <= goal.Deadline;
                prediction.Status = prediction.OnTrack ? "On track" : "Behind schedule";
            }
            //checking if recent and overall saving rates are very different
            if (percentDifference < 20)
            {
                prediction.Confidence = "High";
            }
            else if (percentDifference < 50)
            {
                prediction.Confidence = "Medium";
            }
            else
            {
                prediction.Confidence = "Low";
            }
            var daysUntilDeadline = (goal.Deadline - DateTime.Today).Days;

            if (daysUntilDeadline <= 0)
            {
                prediction.RequiredDailyRate = (decimal)remaining; // deadline passed
            }
            else
            {
                prediction.RequiredDailyRate = (decimal)remaining / daysUntilDeadline;
            }

            return prediction;
        }
    }
}
