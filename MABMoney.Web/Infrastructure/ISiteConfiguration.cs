namespace MABMoney.Web.Infrastructure
{
    public interface ISiteConfiguration
    {
        string this[string key] { get; }
        T Get<T>(string key);
    }
}