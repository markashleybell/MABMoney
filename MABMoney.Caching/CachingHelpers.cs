namespace MABMoney.Caching
{
    public class CachingHelpers : ICachingHelpers
    {
        IModelCacheConfiguration _config;
        int _userId;

        public CachingHelpers(IModelCacheConfiguration config, int userId)
        {
            _config = config;
            _userId = userId;
        }

        public string GetCacheKey(string key)
        {
            return _config.Get<string>("CookieKey") + "-" + _userId + "-" + key;
        }

        public string GetDependencyKey(CachingDependency dependency)
        {
            return GetDependencyKey(dependency.ToString().ToLower(), _userId.ToString());
        }

        public string GetDependencyKey(string key)
        {
            return GetDependencyKey(key, _userId.ToString());
        }

        public string GetDependencyKey(string key, string userId)
        {
            return _config.Get<string>("CookieKey") + "-" + userId + "-dependency-" + key;
        }
    }
}
