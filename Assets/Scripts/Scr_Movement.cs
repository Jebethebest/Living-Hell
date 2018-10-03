using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Movement : MonoBehaviour
{
    [SerializeField] private float m_PlayerSpeed = 100.0f;
    [SerializeField] private float m_JumpHeight = 250.0f;
    [SerializeField] private int m_MaxNrOfJumps = 1;

    [SerializeField] private GameObject m_CameraObject;

    [SerializeField] private Transform m_SpawnPoint;

    [SerializeField] private GameObject m_Target;
    [SerializeField] private float m_TargetSpeed;

    private Scr_PlayerStateController m_PlayerState;
    private Scr_Input m_Input;
    private Rigidbody m_RigidBody;

    private bool m_CanMove = true;
    private bool m_IsGrounded = true;
    private float m_GroundDistance;
    private int m_NrOfJumpsRemaining;

    // Use this for initialization
    void Start ()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_GroundDistance = GetComponent<Collider>().bounds.extents.y;
        m_PlayerState = GetComponent<Scr_PlayerStateController>();
        m_Input = GetComponent<Scr_Input>();

        //Rotate player with Camera's Y
        float yRot = m_CameraObject.transform.rotation.eulerAngles.y;
        transform.localEulerAngles = new Vector3(0, yRot, 0);

        Respawn();
    }
	
	// Update is called once per frame
	void Update ()
	{
        if (m_PlayerState.PlayerState != Scr_PlayerStateController.State.BabyBox)
        {
            if (m_CanMove)
            {
                Move();
                Jump();
            }
        }
        else
        {
            MoveTarget();
        }
    }

    private void Move()
    {
        //float moveX = Input.GetAxis(m_Input.GetHorizontal());
        //float moveZ = Input.GetAxis(m_Input.GetVertical());

        //Vector3 movementDirection = new Vector3(moveX, 0.0f, moveZ).normalized;
        //if (movementDirection != Vector3.zero)
        //{
        //    transform.localRotation = Quaternion.LookRotation(movementDirection);
        //    transform.Rotate(new Vector3(0, m_CameraObject.transform.rotation.eulerAngles.y, 0));
        //}

        //if (!Mathf.Approximately(Mathf.Abs(moveX), 0.0f) || !Mathf.Approximately(Mathf.Abs(moveZ), 0.0f))
        //{
        //    Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        //    transform.Translate(localForward * m_PlayerSpeed * Time.deltaTime, Space.Self);
        //}
    }

    private void MoveTarget()
    {
        //float moveX = Input.GetAxis(m_Input.GetHorizontal());
        //float moveZ = Input.GetAxis(m_Input.GetVertical());

        //Vector3 direction = new Vector3(moveX, moveZ, 0).normalized;
        //m_Target.transform.Translate(direction * m_TargetSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        //Problem: if you press really fast, you can still double jump
        m_IsGrounded = Physics.Raycast(transform.position, Vector3.down, m_GroundDistance);

        if (m_IsGrounded)
            m_NrOfJumpsRemaining = m_MaxNrOfJumps;

        if (Input.GetButtonDown(m_Input.GetJump()) && m_NrOfJumpsRemaining >= 1)
        {
            m_RigidBody.AddForce(Vector3.up * m_JumpHeight * Time.deltaTime, ForceMode.Impulse);
            --m_NrOfJumpsRemaining;
        }
    }

    public void Respawn()
    {
        transform.position = m_SpawnPoint.position;
        m_RigidBody.velocity = Vector3.zero;
        float yRot = m_CameraObject.transform.rotation.eulerAngles.y;
        transform.localEulerAngles = new Vector3(0, yRot, 0);

        Vector3 targetPos = new Vector3(transform.position.x, m_Target.transform.position.y, transform.position.z);
        m_Target.transform.position = targetPos;
        m_Target.SetActive(false);
    }

    public void GetInBox(Vector3 pos)
    {
        transform.position = pos;
        m_PlayerState.PlayerState = Scr_PlayerStateController.State.BabyBox;

        float yRot = m_CameraObject.transform.rotation.eulerAngles.y;
        transform.localEulerAngles = new Vector3(0, yRot, 0);

        m_Target.SetActive(true);
    }

    public bool GetCanMove()
    {
        return m_CanMove;
    }

    public void SetCanMove(bool canMove)
    {
        m_CanMove = canMove;
    }

    public void OnCollisionEnter(Collision collision)
    {
        m_CanMove = true;
    }

    public void SetNumberOfJumps(int nrOfJumps)
    {
        m_MaxNrOfJumps = nrOfJumps;
    }

    public int GetNumberOfMaxJumps()
    {
        return m_MaxNrOfJumps;
    }

    public void SetPlayerSpeed(float speed)
    {
        m_PlayerSpeed = speed;
    }

    public float GetPlayerSpeed()
    {
        return m_PlayerSpeed;
    }
}
