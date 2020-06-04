using System.Collections.Generic;

namespace Logfella.AspNetCore
{
    public static class HashSet
    {
        public static HashSet<T> New<T>(params T[] values) =>
            new HashSet<T>(values);
    }
}