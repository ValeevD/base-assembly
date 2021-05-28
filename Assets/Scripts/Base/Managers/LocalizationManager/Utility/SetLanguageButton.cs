using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Base
{
    [RequireComponent(typeof(Button))]
    public sealed class SetLanguageButton : AbstractBehaviour
    {
        [Inject] ILocalizationManager locaManager = default;

        public Language language;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => locaManager.CurrentLanguage = language);
        }
    }
}
