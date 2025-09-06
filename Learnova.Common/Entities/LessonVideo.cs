namespace Learnova.Domain.Entities
{
    public class LessonVideo
    {
        public int Id { get; set; }

        public int LessonId { get; set; }

        public string FileName { get; set; }
        public string FileUrl { get; set; }

        public DateTime UploadedAt { get; set; }

        public string S3FileUrl { get; set; }

        public long FileSize { get; set; }


        public string UploadedById { get; set; }



        //navation prop
        public Lesson Lesson { get; set; }

    }
}
