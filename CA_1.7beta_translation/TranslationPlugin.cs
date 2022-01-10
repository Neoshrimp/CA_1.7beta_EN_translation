using BepInEx;
using GameDataEditor;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CA_1._7beta_translation
{
    [BepInPlugin(GUID, "Beta translation for new random events", version)]
    [BepInProcess("ChronoArk.exe")]
    public class TranslationPlugin : BaseUnityPlugin
    {

        public const string GUID = "org.neo.beta.translation";
        public const string version = "1.0.0";


        private static readonly Harmony harmony = new Harmony(GUID);

        private static BepInEx.Logging.ManualLogSource logger;

        void Awake()
        {
            logger = Logger;
            harmony.PatchAll();
        }
        void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchAll(GUID);
        }

        [HarmonyPatch(typeof(GDEDataManager), nameof(GDEDataManager.InitFromText))]
        // modify gdata json
        class JsonInject
        {

            
            static void Prefix(ref string dataString)
            {
                Dictionary<string, object> masterJson = (Json.Deserialize(dataString) as Dictionary<string, object>);
                Dictionary<string, object> translatedJson;
                using (StreamReader r = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "RandomEvent_data.json")))
                {
                    translatedJson = Json.Deserialize(r.ReadToEnd()) as Dictionary<string, object>;
                }

                string[] fields = { "Name", "Desc", "UseButtonTooltip", "EventDetails", "OrderStrings", "UseButton" };

                foreach (var e in translatedJson)
                {
                    foreach (string k in fields)
                    {
                        (masterJson[e.Key] as Dictionary<string, object>)[k] = (translatedJson[e.Key] as Dictionary<string, object>)[k];
                    }
                }

                dataString = Json.Serialize(masterJson);

            }
        }

    }
}
