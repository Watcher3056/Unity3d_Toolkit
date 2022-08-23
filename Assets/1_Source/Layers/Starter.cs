using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamAlpha.Source
{

    public partial class Starter : MonoBehaviour
    {
        public static Starter Default { get; private set; }

        public Starter() => Default = this;
        private void Awake()
        {
            ProcessorSaveLoad.OnLocalDataUpdated += HandleLocalDataUpdated;
            HandleLocalDataUpdated();
#if UNITY_EDITOR
            DataGameMain.Default._EditorUpdateData();
#endif
            InitAllAssets();
        }
        private void InitAllAssets()
        {
            foreach (IRequireAssetInit asset in DataGameMain.Default.DBAssetsRequireInit)
                asset.InitAsset();
        }
        private void HandleLocalDataUpdated()
        {

        }
        private void Start()
        {

        }
    }
}
