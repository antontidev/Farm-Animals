using UnityEngine;

public interface IFoodPlatform {
    void OnWantEatFood(IHungry hungry);
}

public class FoodLogic : MonoBehaviour, IFoodPlatform {
    [SerializeField]
    private float strengh;

    public void OnWantEatFood(IHungry hungry) {
        hungry.Eat(strengh);
    }
}