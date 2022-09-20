using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class PanelViewBehaviour : MonoBehaviour
    {
        [Required]
        public Panel panel;

        public void HandlePanelOpen(Action onComplete = default)
        {
            void OnComplete()
            {
                if (onComplete != null)
                    onComplete.Invoke();
            }

            Coroutine
                .Start(HandlePanelOpen())
                .OnComplete(OnComplete);
        }
        protected virtual IEnumerator HandlePanelOpen()
        {
            yield break;
        }
        public void HandlePanelClose(Action onComplete = default)
        {
            void OnComplete()
            {
                if (onComplete != null)
                    onComplete.Invoke();
            }

            Coroutine
                .Start(HandlePanelClose())
                .OnComplete(OnComplete);
        }
        protected virtual IEnumerator HandlePanelClose()
        {
            yield break;
        }
    }
}