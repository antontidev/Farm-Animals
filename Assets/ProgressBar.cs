using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG;
using UniRx;
using DG.Tweening;
using System;

public interface IProgressShow {
    void ChangeProgress(float value);

    void ChangeMaxValue(float maxValue);
}

public interface IFader {
    void FadeIn(Image whatToFade, float seconds);

    void FadeOut(Image whatToFade, float seconds);
}

public class SimpleFader : IFader {
    private static SimpleFader _instance;

    public static SimpleFader Instance {
        get {
            if (_instance == null) {
                _instance = new SimpleFader();
            }

            return _instance;
        }
    }

    public void FadeIn(Image whatToFade, float seconds) {
        whatToFade.DOFade(1, seconds);
    }

    public void FadeOut(Image whatToFade, float seconds) {
        whatToFade.DOFade(0, seconds);
    }
}

public class ProgressBar : MonoBehaviour, IProgressShow
{
    #region Images
    [SerializeField]
    private Image heroImage;

    [SerializeField]
    private Image background;

    [SerializeField]
    private Image fill;
    #endregion

    [SerializeField]
    private Slider progress;

    [SerializeField]
    private float fadeSeconds;

    private bool firstChange = true;

    private float lastChangeTime;

    private IDisposable everyTwoSeconds;

    private void Start() {
        gameObject.SetActive(false);

        FadeOutImmediately();
    }

    public void ChangeMaxValue(float maxValue) {
        progress.maxValue = maxValue;
    }

    public void ChangeProgress(float value) {
        if (everyTwoSeconds != null) {
            everyTwoSeconds.Dispose();
        }

        if (firstChange) {
            firstChange = false;

            gameObject.SetActive(true);

            FadeIn();
        }

        progress.value = value;

        lastChangeTime = Time.time;

        everyTwoSeconds = Observable.Timer(TimeSpan.FromSeconds(1.1f)).Subscribe(time => {
            var sub = Time.time - lastChangeTime;

            if (sub > 1f) {
                FadeOut();

                everyTwoSeconds.Dispose();

                firstChange = true;
            }
        });
    }

    private void FadeIn() {
        var fader = SimpleFader.Instance;

        fader.FadeIn(heroImage, fadeSeconds);
        fader.FadeIn(background, fadeSeconds);
        fader.FadeIn(fill, fadeSeconds);
    }

    private void FadeOut() {
        var fader = SimpleFader.Instance;

        fader.FadeOut(heroImage, fadeSeconds);
        fader.FadeOut(background, fadeSeconds);
        fader.FadeOut(fill, fadeSeconds);
    }

    private void FadeOutImmediately() {
        gameObject.SetActive(false);

        var transparent = Color.white;
        transparent.a = 0;

        heroImage.color = transparent;
        background.color = transparent;
        fill.color = transparent;
    }
}
