using Mirror;
using MyBox;
using SukharevShared;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Cinemachine;

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

public abstract class PlayerController : NetworkBehaviour, IJumper, IMover, IFlyer, IHungry {
    [Inject]
    private IPlayerMover playerMover;

    [Inject]
    [Tooltip("Used to call game manager specific methods")]
    private ICollisionManager collisionManager;

    [Header("Camera movement")]
    [SerializeField]
    private bool cameraRelativeMovement = true;

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

    [Header("Movement")]
    public float movementSpeed = 3;
    public float attackForce;
    public float jumpFoce;

    [Header("Hunger")]
    [SerializeField]
    private float hunger;

    [SerializeField]
    private float hungerSpeed;

    private Animator anim;
    private Rigidbody rb;
    private BoxCollider boxCollider;
    private AudioSource playerSource;

    private bool flying = false;

    private void Start() {
        if (!isLocalPlayer)
            return;
        
        if (cam == null) {
            cam = Camera.main;
        }

        var brain = cam.GetComponent<CinemachineBrain>();

        var vCam = (CinemachineVirtualCamera)brain.ActiveVirtualCamera;

        vCam.Follow = transform;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        playerSource = GetComponent<AudioSource>();
    }

    void Update() {
        if (!isLocalPlayer)
            return;

        ControllPlayer();
        //ControlHunger();
    }

    void ControlHunger() {
        // Awesome tricks
        // Кал говна
        // Rewrite this code
        if (hunger > 0.2f) {
            hunger -= Time.deltaTime * hungerSpeed;

            collisionManager.OnPlayerHungerChanged(hunger);
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

    public virtual void ControllPlayer() {
        var move = playerMover.Move;

        float moveHorizontal = move.x;
        float moveVertical = move.y;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        Move(movement);

        if (playerMover.Jump) {
            Jump(new Vector3(0f, jumpFoce, 0f));
        }

        if (playerMover.Attack) {
            var attackDirection = transform.forward;
            Attack(attackDirection);
        }
    }

    private void Attack(Vector3 attackDirection) {
        var attack = attackDirection * attackForce;

        rb.AddForce(attack, ForceMode.Impulse);
    }

    public virtual void Move(Vector3 movement) {
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

        anim.SetFloat("Walk", movement.magnitude * movementSpeed);
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

    public virtual void OnTriggerEnter(Collider other) {
        var gameObj = other.gameObject;

        if (gameObj.HasTag("Killer")) {
            collisionManager.OnPlayerDeath(gameObject);
        }
        else if (gameObj.HasTag("Coin")) {
            collisionManager.OnCoinGrab(gameObj);
        }
        else if (gameObj.HasTag("Jump")) {
            var jumper = gameObj.GetComponent<JumperLogic>() as IJumpPlatform;

            Firebase.Analytics.FirebaseAnalytics.LogEvent("custom_jump_event");

            jumper.OnTryJump(this);
        }
        else if (gameObj.HasTag("Fly")) {
            var flyer = gameObj.GetComponent<FlyerLogic>() as IFlyPlatform;

            flyer.OnTryFly(this);
        }
        else if (gameObj.HasTag("Win")) {
            var switcher = gameObj.GetComponent<LevelSwticherLogic>();

            var levelName = switcher.levelName;

            collisionManager.OnWin(levelName, transform.position);
        }
        else if (gameObj.HasTag("Food")) {
          //  var foodPlatform = gameObj.GetComponent<FoodLogic>() as IFoodPlatform;

           // foodPlatform.OnWantEatFood(this);

            //collisionManager.OnFoodReached(gameObj);
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

            collisionManager.OnPlayerFlying(capacity);

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
    }
    #endregion
}