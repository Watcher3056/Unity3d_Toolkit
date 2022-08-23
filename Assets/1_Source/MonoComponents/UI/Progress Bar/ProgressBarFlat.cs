using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TeamAlpha.Source
{
    public class ProgressBarFlat : ProgressBar
    {
        [Required]
        public Image imageBarMain;
        public Image imageBarDelta;
        public Color colorBarDeltaIncrease = Color.white;
        public Color colorBarDeltaDecrease = Color.white;
        public float animSpeedBarMain = 4f;
        [ShowIf("imageBarDelta")]
        public float animSpeedBarDelta = 4f;
        public float delayBarMain;
        public Ease animEaseBarMain = Ease.Flash;
        [ShowIf("imageBarDelta")]
        public Ease animEaseBarDelta = Ease.Flash;

        protected override void HandleUpdateView(float current, float max, float fillAmountCur, float fillAmountPrev)
        {
            imageBarMain.DOKill();
            if (imageBarDelta != null)
            {
                imageBarDelta.DOKill();
                if (fillAmountCur >= fillAmountPrev && imageBarDelta != null)
                {
                    imageBarDelta.color = colorBarDeltaIncrease;
                    imageBarDelta
                        .DOFillAmount(fillAmountCur, 1f / animSpeedBarDelta)
                        .SetEase(animEaseBarDelta)
                        .OnComplete(() =>
                        {
                            imageBarMain
                            .DOFillAmount(fillAmountCur, 1f / animSpeedBarMain)
                            .SetEase(animEaseBarMain)
                            .SetDelay(delayBarMain);
                        });
                }
                else if (fillAmountCur < fillAmountPrev)
                {
                    imageBarDelta.color = colorBarDeltaDecrease;
                    imageBarMain
                        .DOFillAmount(fillAmountCur, 1f / animSpeedBarDelta)
                        .SetEase(animEaseBarDelta)
                        .OnComplete(() =>
                        {
                            imageBarDelta
                            .DOFillAmount(fillAmountCur, 1f / animSpeedBarMain)
                            .SetEase(animEaseBarMain)
                            .SetDelay(delayBarMain);
                        });
                }
            }
            else
            {
                imageBarMain
                    .DOFillAmount(fillAmountCur, 1f / animSpeedBarMain)
                    .SetEase(animEaseBarMain);
            }
        }
    }
}
