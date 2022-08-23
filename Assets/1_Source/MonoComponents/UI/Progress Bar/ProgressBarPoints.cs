using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class ProgressBarPoints : ProgressBar
    {
        [Required]
        public SimpleGrid holder;
        public Color colorFill = Color.white;
        public TimeZone timeZoneAnimSpeed = new TimeZone();
        [Required, AssetsOnly]
        public ProgressBarPointsElement prefabElement;

        private List<ProgressBarPointsElement> elements = new List<ProgressBarPointsElement>();
        private void Awake()
        {
            holder.transform.DestroyAllChilds();
        }
        protected override void HandleUpdateView(float current, float max, float fillAmountCur, float fillAmountPrev)
        {
            while (elements.Count != (int)max)
            {
                if (elements.Count > (int)max)
                {
                    Destroy(elements.Last().gameObject);
                    elements.RemoveAt(elements.Count - 1);
                }
                if (elements.Count < (int)max)
                {
                    ProgressBarPointsElement element = prefabElement.Instantiate(holder.transform);
                    elements.Add(element);
                }
            }

            for (int i = 0; i < elements.Count; i++)
            {
                ProgressBarPointsElement element = elements[i];

                element.imageView.DOKill();
                if (i + 1 <= (int)current)
                {
                    element.imageView.DOColor(colorFill, 1f).SetTimeZone(timeZoneAnimSpeed);
                }
                else
                {
                    Color colorFade = colorFill;
                    colorFade.a = 0f;
                    element.imageView.DOColor(colorFade, 1f).SetTimeZone(timeZoneAnimSpeed);
                }
            }

            holder.UpdateViewDelayed();
        }
    }
}
