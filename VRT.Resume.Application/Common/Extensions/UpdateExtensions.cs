using Microsoft.EntityFrameworkCore;

namespace VRT.Resume.Application
{
    internal static class UpdateExtensions
    {
        public static bool HasChanges<T>(this T obj, DbContext context )
            where T: class
        {
            if (obj == null || context==null) return false;
            var state = context.Entry(obj).State;
            return state == EntityState.Modified
                || state == EntityState.Added;
        }
    }
}
