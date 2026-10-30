
//using System.Security.Claims;
//using Hangfire.Dashboard;

//namespace Motorak.Utility
//{
//    public class HangFireAuth : IDashboardAuthorizationFilter
//    {
//        public bool Authorize(DashboardContext context)
//        {
//            var httpContext = context.GetHttpContext();

//            // Allow access only to authenticated admin users
//            return httpContext.User.Identity.IsAuthenticated &&
//                   httpContext.User.IsInRole(Seed.Role_Admin);
//        }
//    }
//}
