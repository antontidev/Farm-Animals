using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public interface IBarShow {
    void ChangeValue(float value);

    void ChangeMaxValue(float value);
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

public class ProgressBar : BaseBar {
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

    public override float MaxValue {
        get {
            return progress.maxValue;
        }
        set {
            progress.maxValue = value;
        }
    }

    public override void ChangeMaxValue(float maxValue) {
        progress.maxValue = maxValue;
    }

    public override void ChangeValue(float newValue) {
        base.ChangeValue(newValue);

        progress.value = value = newValue;
    }

    public override void FadeIn() {
        var fader = SimpleFader.Instance;

        fader.FadeIn(heroImage, fadeSeconds);
        fader.FadeIn(background, fadeSeconds);
        fader.FadeIn(fill, fadeSeconds);
    }

    public override void FadeOut() {
        var fader = SimpleFader.Instance;

        fader.FadeOut(heroImage, fadeSeconds);
        fader.FadeOut(background, fadeSeconds);
        fader.FadeOut(fill, fadeSeconds);
    }

    public override void FadeOutImmediately() {
        gameObject.SetActive(false);

        var transparent = Color.white;
        transparent.a = 0;

        heroImage.color = transparent;
        background.color = transparent;
        fill.color = transparent;
    }
}
