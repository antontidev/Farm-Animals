using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour {
    [SerializeField]
    private Image fadeImage;

    [SerializeField]
    private float fadeTime = 2f;

    public void Fade(Action afterFade) {
        fadeImage.raycastTarget = true;

        gameObject.SetActive(true);

        fadeImage.DOFade(1f, fadeTime).OnComplete(() => {
            afterFade?.Invoke();
        });
    }

    // Internal fade without callback
    private void Fade() {
        fadeImage.raycastTarget = true;

        fadeImage.DOFade(1f, 0f);
    }

    public void Unfade(Action afterUnfade) {
        fadeImage.raycastTarget = false;

        fadeImage.DOFade(0f, fadeTime).OnComplete(() => {
            afterUnfade?.Invoke();

            gameObject.SetActive(false);
        });
    }
}
