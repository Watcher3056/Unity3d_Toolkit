using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamAlpha.Source
{
    public class ProgressBar : MonoBehaviour
    {
        public TextMeshProUGUI textProgress;
        [ShowIf("textProgress")]
        [InfoBox("{cur} - Means current \n {max} - Means max")]
        public string textProgressFormat = "{cur}/{max}";
        [ShowIf("textProgress")]
        public bool formatNumbersAsKilo = true;
        [ShowIf("textProgress")]
        public float textProgressPunchAmount = 1f;
        [ShowIf("textProgress")]
        public int textProgressPunchVibrato = 5;
        [ShowIf("textProgress")]
        public float animSpeedTextProgressPunch = 5f;

        protected float lastValue = 0f;
        public float GetFillAmount(float current, float max) => current / max;
        public virtual void UpdateView(float current, float max)
        {
            float fillAmountCur = GetFillAmount(current, max);
            float fillAmountPrev = GetFillAmount(lastValue, max);

            if (fillAmountCur.Equals(float.NaN))
                return;

            List<string> args = new List<string>();
            string _textProgressFormat = textProgressFormat;
            if (_textProgressFormat.Contains("{cur}"))
            {
                _textProgressFormat =
                    _textProgressFormat.Replace("{cur}", '{' + args.Count.ToString() + '}');
                if (formatNumbersAsKilo)
                    args.Add(Mathf.CeilToInt(current).FormatNumberAsKilo());
                else
                    args.Add(Mathf.CeilToInt(current).ToString());
            }
            if (_textProgressFormat.Contains("{max}"))
            {
                _textProgressFormat =
                    _textProgressFormat.Replace("{max}", '{' + args.Count.ToString() + '}');
                if (formatNumbersAsKilo)
                    args.Add(Mathf.CeilToInt(max).FormatNumberAsKilo());
                else
                    args.Add(Mathf.CeilToInt(max).ToString());
            }

            if (textProgress != null)
            {
                textProgress.transform.DOKill(true);
                textProgress.transform
                    .DOPunchScale(Vector3.one * textProgressPunchAmount, 1f / animSpeedTextProgressPunch, textProgressPunchVibrato);
                textProgress.text = string.Format(_textProgressFormat, args.ToArray());
            }

            HandleUpdateView(current, max, fillAmountCur, fillAmountPrev);

            lastValue = current;
        }
        protected virtual void HandleUpdateView(float current, float max, float fillAmountCur, float fillAmountPrev)
        {

        }
    }
}
