#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public partial class DataGameMain
    {
        #region EDITOR_SETTINGS

        #endregion
        private DateTime dtLastEditorUpdate = DateTime.MinValue;
        private bool OnValidateOdin(object obj)
        {
            _EditorUpdateData();
            return true;
        }
        [OnInspectorInit]
        public void _EditorUpdateData()
        {
            //Limit data update to one per second, to avoid unnecessary overload
            if (dtLastEditorUpdate.Second == DateTime.Now.Second)
                return;
            dtLastEditorUpdate = DateTime.Now;

            string assetsPath = "Assets/2_Content";

            dbAssetsAll = new List<UnityEngine.Object>();
            dbAssetsAll.AddRange(GetAllAssets<ScriptableObject>(assetsPath).OfType<IGUID>().OfType<UnityEngine.Object>());
            dbAssetsAll.AddRange(GetAllAssets<GameObject>(assetsPath).Get<IGUID>().OfType<UnityEngine.Object>());
            dbAssetsAll.AddRange(GetAllAssets<ScriptableObject>(assetsPath).OfType<IRequireAssetInit>().OfType<UnityEngine.Object>());
            dbAssetsAll.AddRange(GetAllAssets<GameObject>(assetsPath).Get<IRequireAssetInit>().OfType<UnityEngine.Object>());

            //Force refresh
            InitDataIfNeeded(forceRefresh: true);
            UnityEditor.EditorUtility.SetDirty(this);
            this.Log("Data Was Updated");
        }
        private List<T> GetAllAssets<T>(string assetPath) where T : UnityEngine.Object
        {
            List<T> result = new List<T>();
            string[] guids =
                UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name, new string[] { assetPath });
            foreach (string guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                T _type = obj as T;
                if (_type != null)
                    result.Add(_type);
            }
            return result;
        }
    }
}
#endif
