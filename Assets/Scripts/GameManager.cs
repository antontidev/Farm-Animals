using Mirror;
using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ICoinHandler {
    void SaveCoins();

    void BuyCoins(float coins);
}

public interface ICollisionManager {
    void OnCoinGrab(GameObject coin);

    void OnWin(string levelName, Vector3 winPosition);

    void OnPlayerFlying(float capacity);

    void OnPlayerDeath(GameObject player);

    void OnFoodReached(GameObject food);

    void OnPlayerHungerChanged(float hunger);
}

public class GameManager : MonoBehaviour, ICoinHandler, ICollisionManager {
    [SerializeField]
    private FadeManager fadeManager;

    [SerializeField]
    private Vector3 initalPosition;

    #region Bars
    [SerializeField]
    private ProgressBar staminaFlyBar;

    [SerializeField]
    private ProgressBar hungerBar;

    [SerializeField]
    private CoinBar coinBar;

    [SerializeField]
    private GameObject heroPicker;
    #endregion

    [SerializeField]
    private CoinsSettings coinsSettings;

    [ReadOnly]
    public int coins;

    public bool showDebugInfo;

    public int loadDelay;

    private void StartSever() {
        var manager = GetComponent<NetworkManager>();

        manager.StartServer();

        Debug.Log("Starting server");
    }

    private void Awake() {
        if (showDebugInfo) {
            SceneManager.LoadScene("Shared", LoadSceneMode.Additive);

            fadeManager.Unfade(null);
        }

        var coinsInSettings = coinsSettings.Coins;

        coinBar.ChangeValue(coinsInSettings);
    }

#region Scene loaders
    private void LoadScene(string levelName) {
        StartCoroutine(LoadSceneAsync(levelName));
    }

    private IEnumerator LoadSceneAsync(string levelName) {
        yield return new WaitForSeconds(loadDelay);

        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    private void ReloadScene() {
        StartCoroutine(ReloadSceneAsync());
    }

    private IEnumerator ReloadSceneAsync() {
        yield return new WaitForSeconds(loadDelay);

        var index = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }
#endregion

#region Player event handlers
    /// <summary>
    /// Invokes when player dies
    /// </summary>
    public void OnPlayerDeath(GameObject player) {
        AudioManager.Instance.PlayDeathEvent();
        // Temporary
        player.transform.position = initalPosition;
    }

    public void OnPickHero() {
        heroPicker.SetActive(true);
    }

    /// <summary>
    /// Invokes when player reach the coin 
    /// </summary>
    /// <param name="coin">Coin GameObject</param>
    public void OnCoinGrab(GameObject coin) {
        var position = coin.transform.position;

        EffectManager.Instance.PlayCoinEffect(position);
        AudioManager.Instance.PlayCoinEvent();

        coin.SetActive(false);

        coins++;

        coinBar.ChangeValue(coinsSettings.Coins + coins);
    }

    public void OnPlayerHungerChanged(float hunger) {
        hungerBar.ChangeValue(hunger);
    }

    public void OnFoodReached(GameObject food) {
        var position = food.transform.position;

        EffectManager.Instance.PlayFoodEffect(position);
        AudioManager.Instance.PlayFoodEvent();

        food.SetActive(false);
    }

    public void OnWin(string levelName, Vector3 winPosition) {
        EffectManager.Instance.PlayWinEffect(winPosition);
        AudioManager.Instance.PlayWinEvent();

        SaveCoins();

        LoadScene(levelName);
    }

    public void OnPlayerFlying(float capacity) {
        staminaFlyBar.ChangeValue(capacity);
    }
#endregion

#region Coins logic
    public void SaveCoins() {
        coinsSettings.SaveCoins(coins);
    }

    public void BuyCoins(float coins) {
        coinsSettings.SaveCoins(coins);
    }
#endregion
}
