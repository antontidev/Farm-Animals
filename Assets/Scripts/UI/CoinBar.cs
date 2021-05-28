using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CoinBar : BaseBar {
    [SerializeField]
    private Button addCoinsButton;

    [SerializeField]
    private TextMeshProUGUI coins;

    [SerializeField]
    private float maxValue;

    #region Events 
    public UnityEvent OnCoinsAdd;
    #endregion

    public override float MaxValue {
        get {
            return maxValue;
        }
        set {
            maxValue = value;
        }
    }

    private void OnEnable() {
        addCoinsButton.onClick.AddListener(AddCoinClick);
    }

    private void OnDisable() {
        addCoinsButton.onClick.RemoveListener(AddCoinClick);
    }

    private void AddCoinClick() {
        OnCoinsAdd?.Invoke();
    }

    public override void ChangeMaxValue(float value) {
        maxValue = value;
    }

    public override void ChangeValue(float value) {
        base.ChangeValue(value);

        var text = Mathf.CeilToInt(value);

        coins.text = text.ToString();
    }

    public override void FadeIn() {
        Debug.LogError("Implement these method or just turn on always visible");
        throw new System.NotImplementedException();
    }

    public override void FadeOut() {
        Debug.LogError("Implement these method or just turn on always visible");
        throw new System.NotImplementedException();
    }

    public override void FadeOutImmediately() {
        Debug.LogError("Implement these method or just turn on always visible");
        throw new System.NotImplementedException();
    }
}