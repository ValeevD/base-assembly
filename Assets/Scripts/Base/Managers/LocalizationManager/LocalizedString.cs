using System;
using UnityEngine;
using UnityEngine.UI;

namespace Base
{
    [Serializable]
    public struct LocalizedString
    {
        public string LocalizationID;

        public LocalizedString(string id)
        {
            LocalizationID = id;
        }
    }
}
