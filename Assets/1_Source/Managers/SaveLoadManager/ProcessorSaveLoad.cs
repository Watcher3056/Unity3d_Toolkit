using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Sirenix.Serialization;
using UnityEngine;


namespace TeamAlpha.Source
{
    public partial class ProcessorSaveLoad : MonoBehaviour
    {
        public static ProcessorSaveLoad Default => _default;
        private static ProcessorSaveLoad _default;

        public static event Action OnLocalDataUpdated = () => { };

        private const string filenameLocal = "saves.txt";
        private const string tagVersion = "SaveFileVersion";
        private const string tagUserID = "UserID";
        //Starts from 0
        private const int lastVersion = 0;

        private bool alreadyInProcess;
        private bool dataWasLoadedFromCloud;

        private void Awake()
        {
            _default = this;
            SetupMigration();

            TryMigrate();
        }
        public static void Save<T>(string key, T value,
            ES2Settings.SaveLocation saveLocation = ES2Settings.SaveLocation.File,
            bool serializeWithJson = false)
        {
            if (serializeWithJson)
            {
                string data = JsonUtility.ToJson(value);
                Save(key, data, saveLocation, false);
            }
            else if (saveLocation == ES2Settings.SaveLocation.File)
                ES2.Save<T>(value, filenameLocal + "?tag=" + key, new ES2Settings { saveLocation = saveLocation });
            else if (saveLocation == ES2Settings.SaveLocation.PlayerPrefs)
                ES2.Save<T>(value, key, new ES2Settings { saveLocation = saveLocation });
            else
                Default.LogError("Unsupported Save Location!");
        }
        public static T Load<T>(string key,
            ES2Settings.SaveLocation saveLocation = ES2Settings.SaveLocation.File,
            bool serializedWithJson = false)
        {
            T result = default;

            if (serializedWithJson)
            {
                string data = Load<string>(key);
                result = JsonUtility.FromJson<T>(data);
            }
            else if (saveLocation == ES2Settings.SaveLocation.File)
                result = ES2.Load<T>(filenameLocal + "?tag=" + key);
            else if (saveLocation == ES2Settings.SaveLocation.PlayerPrefs)
                result = ES2.Load<T>(filenameLocal + key);
            else
                Default.LogError("Unsupported Save Location!");

            return result;
        }
        public static T Load<T>(string key, T defaultValue,
            ES2Settings.SaveLocation saveLocation = ES2Settings.SaveLocation.File,
            bool serializedWithJson = false)
        {
            return Exists(key) ? Load<T>(key, saveLocation, serializedWithJson) : defaultValue;
        }
        public static bool Exists(string key,
            ES2Settings.SaveLocation saveLocation = ES2Settings.SaveLocation.File)
        {
            if (saveLocation == ES2Settings.SaveLocation.File)
                return ES2.Exists(filenameLocal + "?tag=" + key, new ES2Settings { saveLocation = saveLocation });
            else if (saveLocation == ES2Settings.SaveLocation.PlayerPrefs)
                return ES2.Exists(key, new ES2Settings { saveLocation = saveLocation });
            else
            {
                Default.LogError("Unsupported Save Location!");
                return false;
            }
        }
        public static void CleanSaves()
        {
            ES2.Delete(filenameLocal);
            PlayerPrefs.DeleteAll();
            Default.Log("::: Local saves was deleted!");
        }

        #region Saved Games
        private static byte[] GameDataToBytes(ES2Data gameData)
        {
            return SerializationUtility.SerializeValue(gameData, DataFormat.Binary);
        }
        private static ES2Data BytesToGameData(byte[] gameData)
        {
            return SerializationUtility.DeserializeValue<ES2Data>(gameData, DataFormat.Binary);
        }
        private void SaveDataLocal(ES2Data data)
        {
            this.Log(this + "::: Overwriting local data start...");
            Dictionary<string, object> dic = data.loadedData;
            List<string> keys = dic.Keys.ToList();

            using (ES2Writer writer = ES2Writer.Create(filenameLocal))
            {
                // Write our data to the file.
                for (int i = 0; i < keys.Count; i++)
                {
                    writer.Write(dic[keys[i]], keys[i]);
                    this.Log(this + "::: Overwrite key " + keys[i] + " with value: " + dic[keys[i]]);
                }
                // Remember to save when we're done.
                writer.Save();
            }
            this.Log(this + "::: Overwriting local data end!");
            TryMigrate();
        }
        #endregion /Saved Games

        #region Tests
        public static void TestMigration()
        {
            ProcessorSaveLoad.Save(ProcessorSaveLoad.tagVersion, 0);

            //...
        }
        public static void TestSerialization()
        {
            string filenameTest = "test.txt";
            Guid guid = Guid.NewGuid();

            ES2.Save(guid.ToString(), filenameTest + "?tag=guid");
            if (BytesToGameData(GameDataToBytes(ES2.LoadAll(filenameTest))).Load<string>("guid") == guid.ToString())
                Debug.Log("Test Success!");
            else
                Debug.Log("Test Failed!");
        }
        public static void TestSyncronization()
        {
            ES2Data originalData = ES2.LoadAll(filenameLocal);

            Debug.Log("::: Overwriting local originalData start...");
            Dictionary<string, object> dicOriginal = originalData.loadedData;
            List<string> keys = dicOriginal.Keys.ToList();

            using (ES2Writer writer = ES2Writer.Create(filenameLocal))
            {
                // Write our data to the file.
                for (int i = 0; i < keys.Count; i++)
                {
                    writer.Write(dicOriginal[keys[i]], keys[i]);
                    Debug.Log("::: Overwrite key " + keys[i] + " with value: " + dicOriginal[keys[i]]);
                }
                // Remember to save when we're done.
                writer.Save();
            }
            Debug.Log("::: Overwriting local originalData end!");

            Dictionary<string, object> dicNew = ES2.LoadAll(filenameLocal).loadedData;

            for (int i = 0; i < keys.Count; i++)
            {
                if (dicNew[keys[i]].ToString() != dicOriginal[keys[i]].ToString())
                    Debug.LogError("Key: " + keys[i] + " Original value: " + dicOriginal[keys[i]] + " Writed value: " + dicNew[keys[i]]);
            }
            Debug.Log("Sync test ended!");
        }
        #endregion /Tests
    }
}
