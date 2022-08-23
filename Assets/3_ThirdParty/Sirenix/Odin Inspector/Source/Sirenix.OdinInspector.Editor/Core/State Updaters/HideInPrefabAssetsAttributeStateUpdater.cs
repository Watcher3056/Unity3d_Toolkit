#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="HideInPrefabAssetsAttributeStateUpdater.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.RegisterStateUpdater(typeof(Sirenix.OdinInspector.Editor.StateUpdaters.HideInPrefabAssetsAttributeStateUpdater))]

namespace Sirenix.OdinInspector.Editor.StateUpdaters
{
#pragma warning disable

    using UnityEditor;

    public sealed class HideInPrefabAssetsAttributeStateUpdater : AttributeStateUpdater<HideInPrefabAssetsAttribute>
    {
        private bool hide;

        protected override void Initialize()
        {
            var unityObjectTarget = this.Property.Tree.WeakTargets[0] as UnityEngine.Object;

            if (unityObjectTarget != null)
            {
                var type = PrefabUtility.GetPrefabType(unityObjectTarget);
                this.hide =
                    type == PrefabType.Prefab ||
                    type == PrefabType.ModelPrefab;
            }
        }

        public override void OnStateUpdate()
        {
            this.Property.State.Visible = !this.hide;
        }
    }
}
#endif