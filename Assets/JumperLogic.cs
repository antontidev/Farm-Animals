using System.Collections;
using System.Collections.Generic;
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

    public int JumpCount;

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

            JumpCount--;
        }

        if (!Available) {
            MakeUnavalaibleVisible();
        }
    }

    public void PlayJumpAnimation() {
        animator.SetTrigger("Jump");
    }
}
