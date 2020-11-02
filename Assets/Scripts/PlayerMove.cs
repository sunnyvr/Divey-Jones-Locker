using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //TODO: tweak all the multipliers and drag and stuff
    //TODO: remove player from mask for hook raycast, possibly only allow certain things to be hooked?
    //TODO: if you go into a wall with high momentum you keep getting pushed into it until momentum runs out, fix? (if velocity.magnitude < 1: momentum = 0)?
    //      could be a feature, splat into the wall, slows you a bit.
    //      Land before hook again?

    //class variables marked with // are only public for testing, should be made private
    public float speed;
    public float jumpHeight;

    public Vector3 velocity; //
    private float yVelocity;
    private Vector3 momentum;
    public float momentumDrag; //
    public float momentumMultiplier; //

    private float gravity = -10f;
    public bool isGrounded; //
    public Transform groundCheck;
    public LayerMask groundMask;

    private CharacterController cc;
    private Camera pCamera;

    public State state; //
    public enum State //
    {
        Normal,
        Thrown,
        Hooked
    }
    public GameObject testBox;
    private Vector3 hookPos;
    public Transform hookShotTransform; // made public because things were breaking
    private float hookSize;
    public float hookedSpeedMultiplier; //
    public float hookMinSpeed; // Move these two down to HookMovement when done testing. (15, 50) seems ok
    public float hookMaxSpeed; //
    public float hookMaxDist;  //

    private GameController gameController;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        pCamera = transform.Find("Camera").GetComponent<Camera>();

        hookShotTransform.gameObject.SetActive(false);

        GameObject gc = GameObject.Find("Game");
        gameController = gc.GetComponent<GameController>();

        state = State.Normal;
    }

    void Update()
    {
        switch (state)
        {
            case State.Normal:
                GroundMovement();
                break;
            case State.Thrown:
                HookThrown();
                break;
            case State.Hooked:
                HookMovement();
                break;
        }

        HookStart();
    }

    private void GroundMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, groundMask);

        // (right vector*hInput + forward vector*vInput) * speed
        velocity = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical")) * speed;

        //if player is grounded make velocity constant, player can jump while grounded
        if (isGrounded && yVelocity < 0)
        {
            yVelocity = -1f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // gravity, multiply by time twice because maths, also yVelocity because otherwise it gets reset
        yVelocity += gravity * Time.deltaTime;
        velocity.y = yVelocity;

        // apply momentum then dampen it 
        velocity += momentum;
        if (momentum.magnitude > 0f)
        {
            momentum -= momentum * momentumDrag * Time.deltaTime;
            if (momentum.magnitude < 0.1f)
            {
                momentum = Vector3.zero;
            }
        }

        cc.Move(velocity * Time.deltaTime);
    }

    private void HookStart()
    {
        // Casts a ray, if it hits player is hooked
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(pCamera.transform.position, pCamera.transform.forward, out RaycastHit raycastHit, hookMaxDist))
            {
                testBox.transform.position = raycastHit.point;
                hookPos = raycastHit.point;
                hookSize = 0f;
                hookShotTransform.gameObject.SetActive(true);
                state = State.Thrown;
            }
        }
    }

    private void HookThrown()
    {
        hookShotTransform.LookAt(hookPos);

        float hookThrowSpeed = 300f;
        hookSize += hookThrowSpeed*Time.deltaTime;
        hookShotTransform.localScale = new Vector3(1,1,hookSize);

        if(hookSize >= Vector3.Distance(transform.position, hookPos)) state = State.Hooked;
    }

    private void HookMovement()
    {
        hookShotTransform.LookAt(hookPos);

        Vector3 hookDir = (hookPos - transform.position).normalized;
        float hookSpeed = Vector3.Distance(transform.position, hookPos);
        hookSpeed = Mathf.Clamp(hookSpeed, hookMinSpeed, hookMaxSpeed);

        cc.Move(hookDir * hookSpeed * hookedSpeedMultiplier * Time.deltaTime);

        // Hook done when distance to hookPos < 1.5f (if it's too low gets stuck on some slants)
        // or when player clicks again
        if (Vector3.Distance(transform.position, hookPos) < 1.5f || Input.GetMouseButtonDown(0))
        {
            HookStop();
        }

        // add momentum on jump, based on hookDir, current hookSpeed, and a multiplier
        // also adds a little vertical boost, currently double usual jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            momentum = hookDir * hookSpeed * momentumMultiplier;
            momentum += Vector3.up * 2 * Mathf.Sqrt(jumpHeight * -2f * gravity);
            HookStop();
        }
    }
    
    private void HookStop()
    {
        state = State.Normal;
        yVelocity = 0;
        hookShotTransform.gameObject.SetActive(false);
    }

    //temp
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PickUp")
        {
            other.gameObject.SetActive(false);
            gameController.collect();
        }
    }
}
