using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sora;
using Sora.Extension;

namespace Sora.Manager {
    public class TransitionCanvas : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        public Animator animator {
            get => _animator;
        }
        [SerializeField] private CanvasGroup _canvasGroup;
        public CanvasGroup canvasGroup {
            get => _canvasGroup;
        }

        void Awake() {
            transform.GetComponentNullCheck<Animator>(ref _animator);
            transform.GetComponentNullCheck<CanvasGroup>(ref _canvasGroup);
        }

        
        //Fade out
        public void TransitionFadeOut(float t) {
            _animator.speed = 1f/t;
            _animator.Play("Crossfade_End", 0, 0);
            ShowTransitionCanvas(t);
        }
        //Fade in
        public void TransitionFadeIn(float t) {
            _animator.speed = 1f/t;
            _animator.Play("Crossfade_Start", 0, 0);
            ShowTransitionCanvas(-1f);
        }

        public void ShowTransitionCanvas(float t) {
            StopCoroutine(ShowTransitionCanvasCor(t));
            StartCoroutine(ShowTransitionCanvasCor(t));
        }

        IEnumerator ShowTransitionCanvasCor(float t) {
            _canvasGroup.alpha = 1f;

            //If interval less than 0 then hold it forever
            if(t < 0) {
                yield break;
            } else {
                yield return new WaitForSeconds(t);
            }

            _canvasGroup.alpha = 0f;
        }
    }
}