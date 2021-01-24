using MyBox;
using UnityEngine;

public interface IJumpPlatform {
    bool Available {
        get;
    }

    void MakeUnavalaibleVisible();

    void OnTryJump(IJumper jumper);

    void PlayJumpAnimation();
}

public class JumperLogic : MonoBehaviour, IJumpPlatform {
    #region Assignable
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float force;

    [SerializeField]
    private bool infinityJumps;

    [ConditionalField("infinityJumps", true)]
    public int JumpCount = 1;

    #endregion

    public bool Available {
        get {
            return JumpCount > 0;
        }
    }

    public void MakeUnavalaibleVisible() {
        animator.SetTrigger("Unavalaible");
    }

    public void OnTryJump(IJumper jumper) {
        if (Available) {
            PlayJumpAnimation();

            jumper.Jump(new Vector3(0, force, 0));

            if (!infinityJumps) {
                JumpCount--;
            }
        }

        if (!Available) {
            MakeUnavalaibleVisible();
        }
    }

    public void PlayJumpAnimation() {
        animator.SetTrigger("Jump");
    }
}
