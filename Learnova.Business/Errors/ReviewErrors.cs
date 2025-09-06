using Learnova.Business.Abstraction;

namespace Learnova.Business.Errors
{
    public static class ReviewErrors
    {
        public static readonly Error ReviewNotFound = new("Review.NotFound", "Review was not found", 404);

        public static readonly Error CourseNotFound = new("Review.CourseNotFound", "Course was not found", 404);

        public static readonly Error UserNotEnrolled = new("Review.UserNotEnrolled", "User is not enrolled in this course", 403);

        public static readonly Error ReviewAlreadyExists = new("Review.AlreadyExists", "User has already reviewed this course", 409);

        public static readonly Error UnauthorizedAccess = new("Review.UnauthorizedAccess", "User is not authorized to perform this action", 403);

        public static readonly Error CannotReviewOwnCourse = new("Review.CannotReviewOwnCourse", "Course instructors cannot review their own courses", 403);
    }
}