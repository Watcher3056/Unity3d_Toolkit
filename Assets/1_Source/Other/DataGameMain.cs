using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamAlpha.Source
{
    public partial class DataGameMain : ScriptableObject, IRequireAssetInit
    {
        #region SAVE LOAD KEYS
        public const string SAVE_KEY_VIBRATIONS_IS_ENABLED = "VIBRATIONS_IS_ENABLED";
        #endregion
        #region SETTINGS
        [FoldoutGroup("SFX")]
        public AudioPlayData audioOnPanelShow;
        [FoldoutGroup("SFX")]
        public AudioPlayData audioOnPanelHide;
        [FoldoutGroup("Content"), Required, AssetsOnly, AssetSelector(Paths = "Assets/2_Content/Stats")]
        public List<DataStat> statsList;

        //[BoxGroup("Visual")]
        #endregion
        #region RUNTIME DATA
        public static DataGameMain Default
        {
            get
            {
                if (_default == null)
                {
                    _default = Resources.Load<DataGameMain>("DataGameMain");
#if UNITY_EDITOR
                    DataGameMain.Default._EditorUpdateData();
#endif
                }
                return _default;
            }
        }
        private static DataGameMain _default;

        public bool VibrationsIsEnabled
        {
            get => vibrationsIsEnabled;
            set
            {
                vibrationsIsEnabled = value;
                ProcessorSaveLoad.Save(SAVE_KEY_VIBRATIONS_IS_ENABLED, vibrationsIsEnabled);
            }
        }
        private bool vibrationsIsEnabled;

        public Dictionary<string, IGUID> DBAssetsIDs
        {
            get
            {
                InitDataIfNeeded();
                return dbAssetsIDs;
            }
        }
        [NonSerialized, ReadOnly, ShowInInspector]
        private Dictionary<string, IGUID> dbAssetsIDs;
        [SerializeField, ReadOnly]
        public List<UnityEngine.Object> dbAssetsAll;

        public List<IRequireAssetInit> DBAssetsRequireInit
        {
            get
            {
                InitDataIfNeeded();
                return dbAssetsRequireInit;
            }
        }
        [NonSerialized, ReadOnly, ShowInInspector]
        private List<IRequireAssetInit> dbAssetsRequireInit;
        #endregion


        private void HandleLocalDataUpdated()
        {
            vibrationsIsEnabled = ProcessorSaveLoad.Load(SAVE_KEY_VIBRATIONS_IS_ENABLED, true);
        }

        public void InitAsset()
        {
            ProcessorSaveLoad.OnLocalDataUpdated += HandleLocalDataUpdated;
            HandleLocalDataUpdated();
        }
        public void InitDataIfNeeded(bool forceRefresh = false)
        {
            if (dbAssetsIDs == null || forceRefresh)
            {
                List<IGUID> guids = new List<IGUID>();
                guids.AddRange(dbAssetsAll.OfType<IGUID>());

                dbAssetsIDs = new Dictionary<string, IGUID>(guids.Count);

                foreach (IGUID guid in guids)
                    dbAssetsIDs.Add(guid.GUID.id, guid);
            }
            if (dbAssetsRequireInit == null || forceRefresh)
            {
                dbAssetsRequireInit = new List<IRequireAssetInit>(dbAssetsAll.OfType<IRequireAssetInit>());
            }
        }
    }
}
