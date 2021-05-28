using UnityEngine;
using UnityEngine.UI;
public enum ClassTypes {
    None,
    Lion,
    Pigeon,
    Chicken,
    Dog
}


public class CharacterCardController : MonoBehaviour
{
    public ClassTypes underlayingClass;

    [SerializeField]
    private ChosenClass choosenClass;

    [SerializeField]
    private Button clickSurface;

    void Start() {
        clickSurface.onClick.AddListener(OnClassChoose);
    }

    private void OnClassChoose() {
        transform.root.gameObject.SetActive(false);

        choosenClass.Class = underlayingClass;
    }
}
