using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ShootingRangeGame.UI
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class MenuBoard : MonoBehaviour
    {
        public const int MainMenu = 0;
        public const int Countdown = 1;
        public const int GameSession = 2;

        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private float transitionDuration;
        [SerializeField] private float transitionDistance;

        [Space]
        [SerializeField] private TMP_Text countdownText;

        private CanvasGroup[] groups;

        private int targetIndex = 0;
        private int currentIndex = 0;
        private bool transitionActive;

        public CanvasGroup CurrentGroup => currentIndex >= 0 ? groups[currentIndex] : null;
        public CanvasGroup TargetGroup => currentIndex >= 0 ? groups[targetIndex] : null;

        private void Awake()
        {
            groups = GetComponentsInChildren<CanvasGroup>(true);

            foreach (var group in groups)
            {
                group.gameObject.SetActive(true);
                group.alpha = 0.0f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }

            TransitionTo(0);
        }

        public void TransitionTo(int targetIndex)
        {
            this.targetIndex = targetIndex;
            StartCoroutine(TransitionRoutine());
        }

        private void EnableGroup(CanvasGroup group, bool state)
        {
            group.interactable = state;
            group.blocksRaycasts = state;
        }

        private IEnumerator TransitionRoutine()
        {
            if (transitionActive) yield break;
            transitionActive = true;

            do
            {
                float percent;

                IEnumerator animate(Func<float, float> direction)
                {
                    if (!CurrentGroup) yield break;

                    percent = 0.0f;
                    while (percent < 1.0f)
                    {
                        var t = animationCurve.Evaluate(direction(percent));

                        CurrentGroup.alpha = t;
                        CurrentGroup.transform.localPosition = Vector3.down * (1.0f - t) * transitionDistance;

                        percent += Time.deltaTime / transitionDuration;
                        yield return null;
                    }

                    CurrentGroup.alpha = direction(1.0f);
                    CurrentGroup.transform.localPosition = Vector3.down * (1.0f - direction(1.0f)) * transitionDistance;
                }

                EnableGroup(CurrentGroup, false);
                yield return StartCoroutine(animate(v => 1.0f - v));
                currentIndex = targetIndex;
                EnableGroup(CurrentGroup, false);
                yield return StartCoroutine(animate(v => v));
            } while (currentIndex != targetIndex);


            EnableGroup(CurrentGroup, true);
            transitionActive = false;
        }
    }
}