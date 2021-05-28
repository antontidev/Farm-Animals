using MyBox;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Coins")]
public class CoinsSettings : ScriptableObject {
    [SerializeField]
    [ReadOnly]
    private float coins;

    public float Coins {
        get {
            return coins;
        }
    }

    public void SaveCoins(float coins) {
        this.coins += coins;
    }
}