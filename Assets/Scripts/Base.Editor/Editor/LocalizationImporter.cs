using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base.Editor
{
    public sealed class LocalizationImporter : AbstractConfigImporter
    {
        LocalizationImporter()
            : base("1ey0gUulHPi4-BFLMCbc6f7Z7M-7FZU5VFC9_QirlmnQ", "Localization")
        {
        }

        protected override async Task ProcessData(IList<IList<object>> values)
        {
            // Îáðàáàòûâàåì çàãîëîâîê

            if (values.Count < 1)
                throw new Exception("Missing localization data.");
            if (values[0].Count < 1)
                throw new Exception("Missing localization data header.");
            if (values[0][0].ToString() != "ID")
                throw new Exception("Invalid localization data header.");

            var languages = new List<Language>();
            var langSet = new HashSet<Language>();

            for (int i = 1; i < values[0].Count; i++) {
                string langID = values[0][i].ToString();
                if (!Enum.TryParse<Language>(langID, out var lang))
                    throw new Exception("Unknown language ID \"{langID}\".");

                if (!langSet.Add(lang))
                    throw new Exception("Duplicate column for language \"{langID}\".");

                languages.Add(lang);
            }

            // Îáðàáàòûâàåì äàííûå

            var dict = new Dictionary<string, Dictionary<Language, string>>();

            for (int row = 1; row < values.Count; row++) {
                await Progress((float)row / values.Count);
                if (values[row].Count == 0)
                    continue;

                string stringID = values[row][0].ToString();
                if (dict.ContainsKey(stringID))
                    throw new Exception("Duplicate string ID \"{stringID}\".");

                var rowDict = new Dictionary<Language, string>();
                dict[stringID] = rowDict;

                for (int col = 1; col < values[row].Count; col++)
                    rowDict[languages[col - 1]] = values[row][col].ToString();
            }

            // Ñîõðàíÿåì äàííûå

            LocalizationData.EditorInvalidateCache();

            var asset = ScriptableObject.CreateInstance<LocalizationData>();
            asset.TranslatedStrings = dict;
            AssetDatabase.CreateAsset(asset, LocalizationData.AssetPath);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Tools/Import Localizations", false, 1100)]
        static void Import()
        {
            new LocalizationImporter().DoImport();
        }
    }
}
