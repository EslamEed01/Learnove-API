using Hangfire.Dashboard;

namespace Learnova.API.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public HangfireAuthorizationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return true;
            }

            return httpContext.User.IsInRole("Admin");
        }
    }
}