using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class TestTimeZone : MonoBehaviour
    {
        public TimeZone timeZone;
        private TimeZone _timeZoneParent = new TimeZone();

        private void Start()
        {
            //StartCoroutine(_CoroutineDefault());

            for (int i = 0; i < 10000; i++)
            {
                Coroutine
                    .Start(_Coroutine())
                    .SetTimeZone(timeZone);
            }
            timeZone.TimeZoneParent = _timeZoneParent;
        }
        private void Update()
        {
            _timeZoneParent.TimeScaleFactor.SetOverrideOnce(this, value => 1f);
        }
        private IEnumerator _Coroutine()
        {
            yield return null;
            //this.Log("Coroutine Started");
            float timeStart = Time.time;
            yield return new WaitFor(1f);
            //this.Log("Coroutine Completed. time spent: " + (Time.time - timeStart));

            yield break;
        }
        private IEnumerator _CoroutineDefault()
        {
            yield return null;
            //this.Log("Coroutine Started");
            float timeStart = Time.time;
            yield return new WaitForSeconds(1f);
            //this.Log("Coroutine Default Completed. time spent: " + (Time.time - timeStart));

            yield break;
        }
    }
}
