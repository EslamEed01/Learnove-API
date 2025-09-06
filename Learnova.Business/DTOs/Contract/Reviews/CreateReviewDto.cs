namespace Learnova.Business.DTOs.Contract.Reviews
{
    public class CreateReviewDto
    {
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}