namespace MABMoney.Caching
{
    public interface ICachingHelpers
    {
        string GetCacheKey(string key);
        string GetDependencyKey(CachingDependency dependency);
        string GetDependencyKey(string key);
        string GetDependencyKey(string key, string userId);
    }
}
