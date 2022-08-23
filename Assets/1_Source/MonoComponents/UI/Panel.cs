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

        [Required]
        public NamedAnimancerComponent animancer;
        [Required]
        public AnimationClip animOpen;
        [Required]
        public AnimationClip animClose;
        public bool fading;
        public bool hideOnStart = true;
        [ShowIf("fading")]
        public bool closeOnShadowClick;
        public bool playShowHideAudio;
        public bool saveInitialPosition;
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
                    ClosePanel(true, true);
                    gameObject.SetActive(false);
                }
                else
                    OpenPanel(true, true);
            }
        }
        public void TogglePanel(bool arg)
        {
            if (arg)
                OpenPanel();
            else
                ClosePanel();
        }

        //Do not place here logic as On Panel Show
        public void OpenPanel(bool silent = false, bool noEvent = false)
        {
            gameObject.SetActive(true);
            PanelFade(FadeMode.In, silent, noEvent);
        }
        //Do not place here logic as On Panels Hide
        public void ClosePanel(bool silent = false, bool noEvent = false)
        {
            PanelFade(FadeMode.Out, silent, noEvent);
        }
        private void PanelFade(FadeMode fadeMode, bool silent = false, bool noEvent = false)
        {
            if ((curState == State.Opened && fadeMode == FadeMode.In) ||
                (curState == State.Closed && fadeMode == FadeMode.Out))
            {
                if (fadeMode == FadeMode.In)
                    transform.SetAsLastSibling();
                return;
            }

            if (fadeMode == FadeMode.In)
            {
                curShowedPanels.Add(this);
                if (fading && PanelShadow.Default.panel != this)
                    PanelShadow.Default.panel.OpenPanel();
                transform.SetAsLastSibling();
                curState = State.Opened;
                animancer.Play(animOpen);
                if (playShowHideAudio && !silent)
                {
                    if (customAudioOnShow)
                        audioOnShow.Play(ProcessorSoundPool.PoolLevel.Global);
                    else
                        DataGameMain.Default.audioOnPanelShow.Play(ProcessorSoundPool.PoolLevel.Global);
                }
                OnPanelShow.Invoke();
            }
            else if (fadeMode == FadeMode.Out)
            {
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
                curState = State.Closed;
                animancer.Play(animClose).Events.OnEnd = () =>
                {
                    gameObject.SetActive(false);
                };
                if (playShowHideAudio && !silent)
                {
                    if (customAudioOnHide)
                        audioOnHide.Play(ProcessorSoundPool.PoolLevel.Global);
                    else
                        DataGameMain.Default.audioOnPanelHide.Play(ProcessorSoundPool.PoolLevel.Global);
                }
                OnPanelHide.Invoke();
            }

            //UIManager.Default.panelLoading.transform.SetAsLastSibling();
        }
        private void OnDrawGizmos()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            rectTransform.DrawBounds();
        }
    }
}
