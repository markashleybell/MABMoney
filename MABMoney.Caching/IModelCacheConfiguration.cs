namespace MABMoney.Caching
{
    public interface IModelCacheConfiguration
    {
        string this[string key] { get; }
        T Get<T>(string key);
    }
}
