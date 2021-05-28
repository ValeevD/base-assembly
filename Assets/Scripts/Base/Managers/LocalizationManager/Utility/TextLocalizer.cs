using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Base
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteAlways]
    public sealed class TextLocalizer : AbstractBehaviour, IOnLanguageChanged
    {
        public LocalizedString StringID;

        [Inject] private ILocalizationManager locaManager = default;
        TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (Application.IsPlaying(this)) {
                eventBus.Subscribe<IOnLanguageChanged>(this);
                text.text = locaManager.GetString(StringID);
            }
        }

        void IOnLanguageChanged.Do()
        {
            text.text = locaManager.GetString(StringID);
        }

      #if UNITY_EDITOR
        void Update()
        {
            if (Application.IsPlaying(this))
                return;

            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
            if (text != null)
                text.text = LocalizationData.EditorGetLocalization(StringID);
        }
      #endif
    }
}
