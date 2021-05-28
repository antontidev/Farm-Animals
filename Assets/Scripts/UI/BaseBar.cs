using MyBox;
using System;
using UniRx;
using UnityEngine;

public abstract class BaseBar : MonoBehaviour, IBarShow {
    [SerializeField]
    [ReadOnly]
    protected float value;

    [SerializeField]
    protected bool alwaysVisible;

    [SerializeField]
    [ConditionalField("alwaysVisible", true)]
    protected float fadeSeconds;

    private float lastChangeTime;

    private bool firstChange = true;

    private IDisposable everyTwoSeconds;

    public abstract float MaxValue {
        get; set;
    }

    public virtual void Start() {
        if (!alwaysVisible) {
            gameObject.SetActive(false);

            FadeOutImmediately();
        }
    }

    public void SetTimer() {
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
    }

    public virtual void ChangeValue(float value) {
        SetTimer();

        lastChangeTime = Time.time;
    }

    public abstract void ChangeMaxValue(float value);

    public abstract void FadeIn();

    public abstract void FadeOut();

    public abstract void FadeOutImmediately();
}
