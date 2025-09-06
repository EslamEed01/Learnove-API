namespace Learnova.Business.DTOs.CourseDTO
{
    public class CourseDTO
    {

        //  public int CourseId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? LessonsCount { get; set; }


    }
}
