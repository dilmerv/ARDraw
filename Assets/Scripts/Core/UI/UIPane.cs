using System.Collections;

using UnityEngine;
using UnityEngine.Events;

namespace DilmerGames.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPane : MonoBehaviour
    {
        public bool accessableToPanesClass = false;
        public string identity = "";
        public float fadeInTime = 0.25f;
        public float fadeOutTime = 0.25f;
        public bool startHidden = true;
        public bool keepInteractableState = false;

        public UnityEvent onShow;
        public UnityEvent onHide;
        public UnityEvent onEscapeBtn;

        private CanvasGroup canvas;
        private Coroutine fadeRoutine;
        private bool isShowing = false;

        public bool IsShowing
        {
            get
            {
                return isShowing;
            }

            set
            {
                isShowing = value;
            }
        }

        private void Awake()
        {
            canvas = GetComponent<CanvasGroup>();
            IsShowing = gameObject.activeSelf;
            
            if (startHidden)
                HideFast(true);
            else
                ShowFast();
        }

        private void Update()
        {
            if (isShowing)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (onEscapeBtn != null)
                    {
                        onEscapeBtn.Invoke();
                    }

                    Debug.Log(string.Format("Escape/Back was pressed on uiPane with identity of '{0}'", identity));
                }
            }
        }

        public void Show()
        {
            Show(fadeInTime);
        }

        public void Show(float time)
        {
            gameObject.SetActive(true);

            if (onShow != null)
                onShow.Invoke();

            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);
            else
                canvas.alpha = 0;

            fadeRoutine = StartCoroutine(FadeAlpha(1f, time));
        }

        public void ShowFast()
        {
            fadeRoutine = null;

            if (onShow != null)
                onShow.Invoke();

            IsShowing = true;
            canvas.alpha = 1;
            canvas.blocksRaycasts = true;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Hide(fadeOutTime);
        }

        public void Hide(float time)
        {
            if (!IsShowing)
                return;

            if (onHide != null)
                onHide.Invoke();

            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);
            else
                canvas.alpha = 1f;

            fadeRoutine = StartCoroutine(FadeAlpha(0f, time));
        }

        public void HideFast(bool startUp = false)
        {
            if (!IsShowing)
                return;

            fadeRoutine = null;

            if (onHide != null && !startUp)
                onHide.Invoke();

            IsShowing = false;
            canvas.alpha = 0;
            canvas.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        private IEnumerator FadeAlpha(float targetAlpha, float seconds)
        {
            IsShowing = (targetAlpha > 0);
            if(!keepInteractableState)
                canvas.interactable = false;

            if (IsShowing)
                gameObject.SetActive(true);

            var startAlpha = canvas.alpha;
            var t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / seconds;
                canvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return 0;
            }
            canvas.alpha = targetAlpha;

            if(!keepInteractableState)
                canvas.interactable = IsShowing;
            canvas.blocksRaycasts = IsShowing;

            if (!IsShowing)
                gameObject.SetActive(false);

            fadeRoutine = null;
        }
    }
}