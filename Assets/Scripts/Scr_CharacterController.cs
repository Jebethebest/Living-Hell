using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_CharacterController : MonoBehaviour 
{
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] private float m_Mass;
    [SerializeField] private int m_MaxJumps;
    [SerializeField] private GameObject m_Camera;
    [SerializeField] private Transform m_SpawnPoint;
    [SerializeField] private GameObject m_Target;
    [SerializeField] private float m_TargetSpeed;
    [SerializeField] private float m_RotationSpeed;
    [SerializeField] private float m_DeadZone;

    private Scr_Input m_Input;
    private CharacterController m_CharacterController;
    private Scr_AnimationController m_AnimationController;
    private Scr_PlayerStateController m_PlayerState;
    private Vector3 m_LookDirection = Vector3.zero;
    private Vector3 m_MoveDirection = Vector3.zero;
    private Vector3 m_ImpactDirection = Vector3.zero;
    private float m_Gravity = 20.0f;
    private int m_JumpCount = 0;
    private bool m_IsHit = false;
    private bool m_CanMove = true;

    private float m_SlowSpeed;
    private float m_OriginalSpeed;
    private float m_SlowTimer = 0.0f;

	// Use this for initialization
	private void Start () 
    {
        m_Input = GetComponent<Scr_Input>();
		m_CharacterController = GetComponent<CharacterController>();
        m_PlayerState = GetComponent<Scr_PlayerStateController>();
        m_AnimationController = GetComponent<Scr_AnimationController>();

        //Rotate player with Camera's Y
        float yRot = m_Camera.transform.rotation.eulerAngles.y;
        transform.localEulerAngles = new Vector3(0, yRot, 0);

        m_OriginalSpeed = m_Speed;
        m_SlowSpeed = m_Speed * 0.5f;

        Respawn();
    }
	
	// Update is called once per frame
	private void Update () 
    {
        if (m_PlayerState.PlayerState != Scr_PlayerStateController.State.BabyBox)
        {
            Move();            
            UpdateImpact();
        }
        else
        {
            MoveTarget();
        }

        //Lock mid-air movement when punched
        if (m_CharacterController.isGrounded && !m_CanMove)
            m_CanMove = true;

        //Control slow state
        if (m_SlowTimer > 0.0f)
        {
            m_SlowTimer -= Time.deltaTime;

            if (m_SlowTimer <= 0.0f)
                m_Speed = m_OriginalSpeed;
        }
    }

    private void Move()
    {
        if (m_CanMove)
        {
            MoveSingleStick();

            //Jump
            if (m_CharacterController.isGrounded)
            {
                m_MoveDirection.y = -m_Gravity * Time.deltaTime;
                m_JumpCount = 0;
                m_AnimationController.Animate("IsJumping", false);
            }
            if (Input.GetButtonDown(m_Input.GetJump()) && m_JumpCount < m_MaxJumps)
            {
                m_MoveDirection.y = m_JumpSpeed;
                ++m_JumpCount;
                m_AnimationController.Animate("IsJumping", true);

                if (m_JumpCount > 1)
                {
                    Scr_ParticleManager.SpawnParticle("JumpCloud", gameObject.transform.position, Quaternion.identity);
                    Scr_ParticleManager.SpawnParticle("JumpTrail", gameObject.transform.position, Quaternion.identity);
                    Scr_ParticleManager.SpawnParticle("JumpCircle", gameObject.transform.position, Quaternion.identity);
                }
            }

            //Add force impact
            if (m_IsHit)
            {
                m_MoveDirection += m_ImpactDirection;
                m_IsHit = false;
                m_CanMove = false;
            }
        }

        //Apply gravity
        if (!m_CharacterController.isGrounded)
            m_MoveDirection.y -= m_Gravity * Time.deltaTime;

        m_CharacterController.Move(m_MoveDirection * Time.deltaTime);
    }

    private void MoveSingleStick()
    {
        //Move
        float moveX = Input.GetAxis(m_Input.GetHorizontalMove());
        float moveZ = Input.GetAxis(m_Input.GetVerticalMove());
        m_LookDirection = new Vector3(moveX, 0, moveZ);

        //Deadzone
        if (m_LookDirection.magnitude < m_DeadZone)
            m_LookDirection = Vector3.zero;
        else
            m_LookDirection = m_LookDirection.normalized * ((m_LookDirection.magnitude - m_DeadZone) / (1 - m_DeadZone));

        float magnitude = m_LookDirection.magnitude;

        if (m_LookDirection != Vector3.zero)
        {
            //Less snappy rotation
            Quaternion targetRotation = Quaternion.LookRotation(m_LookDirection);
            Quaternion offset = Quaternion.AngleAxis(m_Camera.transform.eulerAngles.y, Vector3.up);
            Quaternion result = offset * targetRotation;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, result, m_RotationSpeed * Time.deltaTime);
        }

        m_MoveDirection = new Vector3(transform.forward.x, m_MoveDirection.y, transform.forward.z);
        m_MoveDirection.x *= magnitude * m_Speed;
        m_MoveDirection.z *= magnitude * m_Speed;

        if (magnitude > 0.0f)
            m_AnimationController.Animate("IsRunning", true);
        else
            m_AnimationController.Animate("IsRunning", false);
    }

    private void UpdateImpact()
    {
        m_ImpactDirection = Vector3.Lerp(m_ImpactDirection, Vector3.zero, 5 * Time.deltaTime);
    }

    private void MoveTarget()
    {
        float moveX = Input.GetAxis(m_Input.GetHorizontalMove());
        float moveZ = Input.GetAxis(m_Input.GetVerticalMove());

        Vector3 direction = new Vector3(-moveX, -moveZ, 0).normalized;
        m_Target.transform.Translate(direction * m_TargetSpeed * Time.deltaTime);

        //Clamp target
        Vector3 pos = m_Target.transform.position;
        pos.x = Mathf.Clamp(pos.x, -6.0f, 9.0f);
        pos.z = Mathf.Clamp(pos.z, -4.0f, 6.0f);
        m_Target.transform.position = pos;
    }

    public void Respawn()
    {
        transform.position = m_SpawnPoint.position;

        float yRot = m_Camera.transform.rotation.eulerAngles.y;
        transform.localEulerAngles = new Vector3(0, yRot, 0);

        Vector3 targetPos = new Vector3(transform.position.x, m_Target.transform.position.y, transform.position.z);
        m_Target.transform.position = targetPos;
        m_Target.SetActive(false);

        m_JumpCount = 0;
        m_IsHit = false;
        m_CanMove = true;

        m_Speed = m_OriginalSpeed;

        Scr_PlayerPowerUpHandler powerHandle = GetComponent<Scr_PlayerPowerUpHandler>();
        powerHandle.ResetPlayerState();

        //Remove held object and fix states
        Scr_PlayerStateController state = GetComponent<Scr_PlayerStateController>();
        state.IsHoldingObject = false;

        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "ThrowableObject")
                child.gameObject.transform.parent = null;
        }

        if (m_AnimationController != null)
            m_AnimationController.Animate("IsHoldingItem", false);
}

    public void GetInBox(Vector3 pos)
    {
        transform.position = pos;
        m_PlayerState.PlayerState = Scr_PlayerStateController.State.BabyBox;

        float yRot = m_Camera.transform.rotation.eulerAngles.y + 180.0f;
        transform.localEulerAngles = new Vector3(0, yRot, 0);

        m_Target.SetActive(true);
    }

    public void AddImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0) 
            direction.y = -direction.y; // reflect down force on the ground

        m_ImpactDirection += direction * force / m_Mass;
        m_IsHit = true;
    }

    public float GetSpeed()
    {
        return m_Speed;
    }

    public void SetSpeed(float speed)
    {
        m_Speed = speed;
    }

    public int GetMaxJumps()
    {
        return m_MaxJumps;
    }

    public void SetMaxJumps(int amount)
    {
        m_MaxJumps = amount;
    }

    public void Slow()
    {
        m_SlowTimer = 10.0f;
        m_Speed = m_SlowSpeed;
        Debug.Log("Slowed!");
    }

    public bool IsSlowed()
    {
        return m_SlowTimer > 0.0f;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        float pushPower = 2.0f;

        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
    }
}