using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Serialization;
using System.Collections;
using Animancer;

namespace TeamAlpha.Source
{
    [RequireComponent(typeof(CanvasGroup), typeof(Animator))]
    public class Panel : MonoBehaviour
    {
        [Serializable]
        public class PanelList
        {
            [Required]
            public List<Panel> list;

            public void OpenPanels()
            {
                foreach (Panel panel in list)
                    panel.OpenPanel();
            }
            public void ClosePanels()
            {
                foreach (Panel panel in list)
                    panel.ClosePanel();
            }
        }
        public enum FadeMode { In, Out }
        public enum State { None, Opened, Closed }

        public PanelViewBehaviour viewBehaviour;
        public bool fading;
        [ShowIf("fading")]
        public bool closeOnFadeClick;
        public bool hideOnStart = true;
        public bool saveInitialPosition;
        public bool playShowHideAudio;
        [ShowIf("playShowHideAudio")]
        public bool customAudioOnShow;
        [ShowIf("ShowAudioOnShow")]
        public AudioPlayData audioOnShow;
        [ShowIf("playShowHideAudio")]
        public bool customAudioOnHide;
        [ShowIf("ShowAudioOnHide")]
        public AudioPlayData audioOnHide;

        [HideInInspector, SerializeField, Required]
        private RectTransform rectTransform;
        public event Action OnPanelShow = () => { };
        public event Action OnPanelHide = () => { };

        public State CurState => curState;
        private State curState;
        private bool ShowAudioOnShow => playShowHideAudio && customAudioOnShow;
        private bool ShowAudioOnHide => playShowHideAudio && customAudioOnHide;

        public static Panel UpperShaded => curShowedPanels.FindLast(p => p.fading);

        private static List<Panel> curShowedPanels = new List<Panel>();
        private void Awake()
        {
            UIManager.Default.panels.Add(this);
        }
        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null && !saveInitialPosition)
                rectTransform.anchoredPosition = Vector3.zero;


            //if (UIManager.Default.panelLoading == this)
            //    return;
            if (curState == State.None)
            {
                if (hideOnStart)
                {
                    ClosePanel(null, true, true);
                    gameObject.SetActive(false);
                }
                else
                    OpenPanel(null, true, true);
            }
        }
        public void TogglePanel(bool arg)
        {
            if (arg)
                OpenPanel();
            else
                ClosePanel();
        }
        public void OpenPanel(Action onComplete = default, bool muteAudio = false, bool noEvent = false)
        {
            if (curState == State.Opened)
            {
                transform.SetAsLastSibling();
                return;
            }

            curShowedPanels.Add(this);
            if (fading && PanelShadow.Default.panel != this)
                PanelShadow.Default.panel.OpenPanel();
            transform.SetAsLastSibling();
            if (playShowHideAudio && !muteAudio)
            {
                if (customAudioOnShow)
                    audioOnShow.Play(ProcessorSoundPool.PoolLevel.Global);
                else
                    DataGameMain.Default.audioOnPanelShow.Play(ProcessorSoundPool.PoolLevel.Global);
            }
            curState = State.Opened;

            gameObject.SetActive(true);
            if (viewBehaviour != null)
                viewBehaviour.HandlePanelOpen(onComplete);
            if (noEvent == false)
                OnPanelShow.Invoke();
        }
        //Do not place here logic as On Panels Hide
        public void ClosePanel(Action onComplete = default, bool muteAudio = false, bool noEvent = false)
        {
            if (curState == State.Closed)
            {
                return;
            }

            if (curShowedPanels.Contains(this))
                curShowedPanels.Remove(this);
            if (fading)
            {
                Panel panelWithFading = curShowedPanels.Find((p) => p.fading);
                if (curShowedPanels.Contains(PanelShadow.Default.panel) && panelWithFading == null)
                    PanelShadow.Default.panel.ClosePanel();
            }

            List<Panel> fadedPanels = new List<Panel>(curShowedPanels.FindAll(p => p.fading));
            for (int i = 0; i < fadedPanels.Count; i++)
            {
                Panel panel = fadedPanels[i];
                if (i + 1 == fadedPanels.Count)
                    PanelShadow.Default.panel.transform.SetAsLastSibling();
                if (panel.fading)
                    panel.transform.SetAsLastSibling();
            }
            if (playShowHideAudio && !muteAudio)
            {
                if (customAudioOnHide)
                    audioOnHide.Play(ProcessorSoundPool.PoolLevel.Global);
                else
                    DataGameMain.Default.audioOnPanelHide.Play(ProcessorSoundPool.PoolLevel.Global);
            }
            curState = State.Closed;
            if (viewBehaviour != null)
                viewBehaviour.HandlePanelClose(() =>
                {
                    gameObject.SetActive(false);
                    if (onComplete != null)
                        onComplete.Invoke();
                });
            if (noEvent == false)
                OnPanelHide.Invoke();
        }
        private void OnDrawGizmos()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            rectTransform.DrawBounds();
        }
    }
}
