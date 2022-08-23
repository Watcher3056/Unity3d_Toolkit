using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamAlpha.Source
{
    public class MonoScalerUI : MonoBehaviour
    {
        public enum ResizeTarget { localScale, rectSize }
        public enum ResizeTo { targetWidth, targetHeigth, both }
        public enum ResizeMethod { Expand, SaveInitialAspect }
        [Required]
        public RectTransform needToScale;
        [Required]
        public RectTransform scaleFrom;
        public ResizeTo resizeTo;
        public ResizeTarget resizeTarget;
        public ResizeMethod resizeMethod;

        [SerializeField, ReadOnly, ShowIf("locked")]
        private Vector2 originLocalScale;
        [SerializeField, ReadOnly, ShowIf("locked")]
        private Vector2 originRectSize;
        [SerializeField, Range(0f, 1f), ShowIf("locked"), OnValueChanged("UpdateView")]
        private float weight = 0.5f;
        [SerializeField, OnValueChanged("HandleLockChanged"), ShowIf("scaleFrom")]
        private bool locked;

        private void Start()
        {
            UpdateView();
        }
        public void UpdateView()
        {
            if (!locked || originLocalScale == Vector2.zero || originRectSize == Vector2.zero)
                return;

            Vector2 scaleFactor = Vector2.one;
            if (resizeMethod == ResizeMethod.Expand)
            {
                scaleFactor.x = ((float)scaleFrom.rect.width / needToScale.rect.width);
                scaleFactor.y = ((float)scaleFrom.rect.height / needToScale.rect.height);
            }
            else if (resizeMethod == ResizeMethod.SaveInitialAspect)
            {
                float prevAspect = originRectSize.x / originRectSize.y;
                float curAspect = needToScale.rect.width / needToScale.rect.height;
                scaleFactor.x = curAspect / prevAspect;
                scaleFactor.y = prevAspect / curAspect;
            }

            float resultScaleFactor = 1f;
            if (resizeTo == ResizeTo.targetWidth)
                resultScaleFactor = scaleFactor.x;
            else if (resizeTo == ResizeTo.targetHeigth)
                resultScaleFactor = scaleFactor.y;
            else if (resizeTo == ResizeTo.both)
                resultScaleFactor = Mathf.Max(scaleFactor.x, scaleFactor.y);
            resultScaleFactor = Mathf.Lerp(1f, resultScaleFactor, weight);

            needToScale.localScale = Vector3.one * resultScaleFactor;

        }
        private void HandleLockChanged()
        {
            if (locked)
            {
                originLocalScale = needToScale.localScale;
                originRectSize = needToScale.rect.size;
                UpdateView();
            }
            else
            {
                needToScale.localScale = originLocalScale;
            }
        }
        private void OnDrawGizmos()
        {
            UpdateView();
        }
    }
}
