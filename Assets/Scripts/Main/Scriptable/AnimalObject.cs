using UnityEngine;


[CreateAssetMenu(fileName = "Animal", menuName = "Animals/Animal", order = 1)]
public class AnimalObject : ScriptableObject {
    public GameObject prefab;
    public Color bacgroundColor;
}
