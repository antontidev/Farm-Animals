using UnityEngine;
using MyBox;

[CreateAssetMenu(menuName = "Settings/Choosen class")]
public class ChosenClass : ScriptableObject {
    [ReadOnly]
    public ClassTypes Class;
}