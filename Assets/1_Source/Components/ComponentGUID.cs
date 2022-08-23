using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TeamAlpha.Source
{
    [Serializable, HideLabel, BoxGroup("GUID")]
    public class ComponentGUID : IEquatable<ComponentGUID>
    {
        public enum IdSourceType { None, Asset, Random }
        [Required, ReadOnly]
        public string id;
        [OnValueChanged("UpdateGUID"), InfoBox("GUID Source is required", Sirenix.OdinInspector.InfoMessageType.Error, VisibleIf = "ErrorVisible")]
        public IdSourceType sourceType = IdSourceType.Asset;

        [HideInInspector]
        public string assetId;
        [HideInInspector]
        public string randomId;
        public Guid GetGuid()
        {
            return new Guid(id);
        }
        [Sirenix.OdinInspector.Button]
        [ShowIf("sourceType", IdSourceType.Random)]
        public void RenerateGUID()
        {
            randomId = null;
            UpdateGUID();
        }
        public void UpdateGUID()
        {
            if (sourceType == IdSourceType.Asset)
                id = assetId;
            else if (sourceType == IdSourceType.Random)
            {
                if (randomId == null || randomId == String.Empty)
                    randomId = Guid.NewGuid().ToString().Replace("-", "");
                id = randomId;
            }
        }
        private bool ErrorVisible() => sourceType == IdSourceType.None;

        public bool Equals(ComponentGUID other) => id.Equals(other.id);
    }
}




