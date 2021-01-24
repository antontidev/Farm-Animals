using UnityEngine;

public interface IPlatform<T> {
    void OnPlatformAction(T flyer);
}

public abstract class AbstractPlatform<T> : IPlatform<T> {
    protected Animator anim;

    public AbstractPlatform(Animator anim) {
    }

    public abstract void OnPlatformAction(T actionExecutor);
}

public interface IFlyPlatform {
    void OnTryFly(IFlyer flyer);

    void PlayFlyAnimation();
}

public class FlyerLogic : MonoBehaviour, IFlyPlatform {
    #region Assignable
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private int capacity;
    #endregion

    public void OnTryFly(IFlyer flyer) {
        PlayFlyAnimation();

        flyer.Fly(capacity);
    }

    public void PlayFlyAnimation() {
        animator.SetTrigger("Jump");
    }
}
