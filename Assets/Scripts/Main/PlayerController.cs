using MyBox;
using SukharevShared;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public interface IJumper {
    void Jump(Vector3 jumpForce);
}

public interface IMover {
    void Move(Vector3 movement);
}

public interface IFlyer {
    void Fly(float capacity);
}

public interface IHungry {
    void Eat(float strengh);
}

public class PlayerController : MonoBehaviour, IJumper, IMover, IFlyer, IHungry {
    [Inject]
    private IPlayerMover playerMover;

    [Header("Camera movement")]
    [SerializeField]
    private bool cameraRelativeMovement;

    [SerializeField]
    [ConditionalField("cameraRelativeMovement")]
    private Camera cam;

    [Header("Audio events")]
    [SerializeField]
    private AudioEvent moveClip;

    [SerializeField]
    private AudioEvent jumpClip;

    [SerializeField]
    private AudioEvent flyClip;

    [SerializeField]
    private AudioSource playerSource;

    [Header("Movement")]
    public float movementSpeed = 3;

    [Header("Hunger")]
    [SerializeField]
    private float hunger;

    [SerializeField]
    private float hungerSpeed;

    [Header("Player components")]
    public Animator anim;
    public Rigidbody rb;
    public BoxCollider boxCollider;

    private bool flying = false;

    #region Tags
    [Header("Tags for interacting")]
    [SerializeField]
    private Tag interestTag;

    [SerializeField]
    private Tag killTag;

    [SerializeField]
    private Tag jumpTag;

    [SerializeField]
    private Tag flyTag;

    [SerializeField]
    private Tag winTag;

    [SerializeField]
    private Tag coinTag;

    [SerializeField]
    private Tag foodTag;
    #endregion

    #region Collisions
    [Header("Collisoin events")]
    public UnityEventGameObject coinReached;

    public UnityEventGameObject foodReached;

    public UnityEventFloat playerFlying;

    public UnityEventFloat playerHungerChanged;

    public UnityEventVector3 playerWin;

    public UnityEvent dead;
    #endregion

    void Update() {
        ControllPlayer();
        ControlHunger();
    }

    void ControlHunger() {
        // Awesome tricks
        // Кал говна
        // Rewrite this code
        if (hunger > 0.2f) {
            hunger -= Time.deltaTime * hungerSpeed;

            playerHungerChanged?.Invoke(hunger);
        }
        else if (hunger == -1f) {
            anim.SetTrigger("hunger");
            hunger = 0f;
        }
        else if (hunger == 0f) {

        }
        else {
            hunger = -1f;
        }
    }

    void ControllPlayer() {
        var move = playerMover.Move;

        float moveHorizontal = move.x;
        float moveVertical = move.y;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        Move(movement);

        // Obsolete for Last levels
        if (playerMover.Jump) {
            Jump(new Vector3(0f, 300f, 0f));
        }
    }

    public void Move(Vector3 movement) {
        if (movement != Vector3.zero) {
            if (cameraRelativeMovement) {
                movement = CameraRelativeMovement(movement);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.30f);

            transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);

            if (!flying) {
                moveClip.Play(playerSource);
            }
        }

        anim.SetFloat("Walk", movement.magnitude);
    }

    private Vector3 CameraRelativeMovement(Vector3 movement) {
        var camTransform = cam.transform;

        var forward = camTransform.forward;
        var right = camTransform.right;
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * movement.z + right * movement.x;
    }

    /// <summary>
    /// Invokes when hunger is stronger than your fingers
    /// Invokes when you fall of from platform
    /// </summary>
    private void Dead() {
        dead?.Invoke();
    }

    private void OnTriggerEnter(Collider other) {
        var gameObj = other.gameObject;

        if (gameObj.HasTag(killTag)) {
            Dead();
        }
        else if (gameObj.HasTag(coinTag)) {
            coinReached?.Invoke(gameObj);

            Debug.Log("Wow such coin");
        }
        else if (gameObj.HasTag(jumpTag)) {
            var jumper = gameObj.GetComponent<JumperLogic>() as IJumpPlatform;

            jumper.OnTryJump(this);
        }
        else if (gameObj.HasTag(flyTag)) {
            var flyer = gameObj.GetComponent<FlyerLogic>() as IFlyPlatform;

            flyer.OnTryFly(this);
        }
        else if (gameObj.HasTag(winTag)) {
            playerWin?.Invoke(transform.position);
        }
        else if (gameObj.HasTag(foodTag)) {
            foodReached?.Invoke(gameObj);

            var foodPlatform = gameObj.GetComponent<FoodLogic>() as IFoodPlatform;

            foodPlatform.OnWantEatFood(this);
        }
    }

    #region Jump
    public void Jump(Vector3 jumpForce) {
        rb.AddForce(jumpForce, ForceMode.Impulse);

        anim.SetTrigger("jump");

        jumpClip.Play(playerSource);
    }
    #endregion

    #region Fly
    public void Fly(float capacity) {
        anim.SetTrigger("wantFly");

        // This should have better perfomance than Unity Coroutines
        MainThreadDispatcher.StartUpdateMicroCoroutine(FlyCoroutine(capacity));
    }

    private IEnumerator FlyCoroutine(float capacity) {
        while (!playerMover.Press) {
            yield return null;
        }

        rb.useGravity = false;
        boxCollider.isTrigger = true;
        flying = true;

        while (capacity > 0.18f && playerMover.Press) {
            anim.SetFloat("fly", capacity);

            playerFlying?.Invoke(capacity);

            capacity -= Time.deltaTime * movementSpeed * 9;

            flyClip.Play(playerSource);

            yield return null;
        }

        flying = false;
        anim.SetTrigger("dontWantFly");
        anim.SetFloat("fly", 0);

        rb.useGravity = true;
        boxCollider.isTrigger = false;
    }
    #endregion

    #region Eat 
    public void Eat(float strengh) {
        hunger += strengh;

        Debug.Log("Tomato next platform");
    }
    #endregion
}