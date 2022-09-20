using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class PanelViewBehaviourFade : PanelViewBehaviour
    {
        [Required]
        public CanvasGroup canvasGroup;
        public float scaleOpened = 1f;
        public float scaleClosed = 0.6f;
        public Ease easeOpen = Ease.InCirc;
        public Ease easeClose = Ease.InCirc;
        public TimeZone speedOpen;
        public TimeZone speedClose;

        protected override IEnumerator HandlePanelOpen()
        {
            Tweener tweenerCanvas =
                canvasGroup
                .DOFade(1f, 1f)
                .SetEase(easeOpen)
                .SetTimeZone(speedOpen);
            Tweener tweenerScale = 
                transform
                .DOScale(scaleOpened, 1f)
                .SetEase(easeOpen)
                .SetTimeZone(speedOpen);

            while (tweenerCanvas.IsPlaying() || tweenerScale.IsPlaying())
                yield return null;
            yield break;
        }
        protected override IEnumerator HandlePanelClose()
        {
            Tweener tweenerCanvas =
                canvasGroup
                .DOFade(0f, 1f)
                .SetEase(easeClose)
                .SetTimeZone(speedClose);
            Tweener tweenerScale =
                transform
                .DOScale(scaleClosed, 1f)
                .SetEase(easeClose)
                .SetTimeZone(speedClose);

            while (tweenerCanvas.IsPlaying() || tweenerScale.IsPlaying())
                yield return null;
            yield break;
        }
    }
}