using IssueDashboard.Models;

namespace IssueDashboard.Providers.Reviews
{
    public interface IReviewProvider
    {
        List<Review> GetReviews();
    }
}