using DG.Tweening;
using MyBox;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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

public class ProgressBar : MonoBehaviour, IProgressShow {
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
    private bool alwaysVisible;

    public float MaxValue {
        get {
            return MaxValue;
        }
        set {
            progress.maxValue = value;
        }
    }

    [SerializeField]
    [ReadOnly]
    private float value;

    [SerializeField]
    [ConditionalField("alwaysVisible", true)]
    private float fadeSeconds;

    private bool firstChange = true;

    private float lastChangeTime;

    private IDisposable everyTwoSeconds;

    private void Start() {
        if (!alwaysVisible) {
            gameObject.SetActive(false);

            FadeOutImmediately();
        }
    }

    public void ChangeMaxValue(float maxValue) {
        progress.maxValue = maxValue;
    }

    public void ChangeProgress(float newValue) {
        if (!alwaysVisible) {
            if (everyTwoSeconds != null) {
                everyTwoSeconds.Dispose();
            }

            if (firstChange) {
                firstChange = false;

                gameObject.SetActive(true);

                FadeIn();
            }

            everyTwoSeconds = Observable.Timer(TimeSpan.FromSeconds(1.1f)).Subscribe(time => {
                var sub = Time.time - lastChangeTime;

                if (sub > 1f) {
                    FadeOut();

                    everyTwoSeconds.Dispose();

                    firstChange = true;
                }
            });
        }

        progress.value = value = newValue;

        lastChangeTime = Time.time;
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
