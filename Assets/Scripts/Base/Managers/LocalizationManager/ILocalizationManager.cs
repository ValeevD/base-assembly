namespace Base
{
    public interface ILocalizationManager
    {
        Language CurrentLanguage { get; set; }
        string GetString(LocalizedString str);
    }
}
