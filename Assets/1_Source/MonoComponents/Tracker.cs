using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class Tracker : MonoBehaviour
    {
        public enum TrackMode { SamePosition, SaveInitialLocalPos }
        [Required]
        public Transform target;
        public TrackMode trackMode;

        private Vector3 localPosToTarget;

        private void Awake()
        {
            if (target != null)
                SetTarget(target);
        }
        private void LateUpdate()
        {
            if (target == null)
                return;
            if (trackMode == TrackMode.SamePosition)
                transform.position = target.position;
            else if (trackMode == TrackMode.SaveInitialLocalPos)
                transform.position = target.TransformPoint(localPosToTarget);
        }
        public void SetTarget(Transform target)
        {
            this.target = target;
            localPosToTarget = target.InverseTransformPoint(transform.position);
        }
    }
}
