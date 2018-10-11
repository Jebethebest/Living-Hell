using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public class Scr_Combat : MonoBehaviour
{
    [SerializeField] private float m_PunchingPower = 100.0f;
    [SerializeField] private float m_ThrowingForce = 10.0f;

    [SerializeField] private GameObject m_PunchHitBox;
    [SerializeField] private Vector3 m_HalfExtends = new Vector3(1, 1, 1);
    [SerializeField] private float m_Length = 1.0f;

    private Scr_AnimationController m_AnimationController;
    private Scr_PlayerStateController m_PlayerState;
    private Scr_Input m_Input;
    private GameObject m_ClosestObject;
    private GameObject m_LastHitBy;

    private enum ThrowState
    {
        Not, Charging, Throwing
    }

    private ThrowState m_ThrowState = ThrowState.Not;

    // Use this for initialization
    void Start()
    {
        m_PunchHitBox.SetActive(false);

        m_AnimationController = GetComponent<Scr_AnimationController>();
        m_PlayerState = GetComponent<Scr_PlayerStateController>();
        m_Input = GetComponent<Scr_Input>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_PlayerState.IsMomInRoom)
        {
            if (m_PlayerState.PlayerState != Scr_PlayerStateController.State.BabyBox)
            {
                Punch();

                if (m_PlayerState.IsHoldingObject)
                    Throw();
                else
                    Grab();
            }
            else
                Fire();
        }        
    }

    private void Punch()
    {
        //Punching
        if (Input.GetButtonDown(m_Input.GetFire()))
        {
            GameObject closestPlayer = GetClosestObjectWithTag("Player");
            m_AnimationController.Animate("Punch");

            if (closestPlayer != null)
            {
                Scr_PlayerStateController state = closestPlayer.GetComponent<Scr_PlayerStateController>();
                if (state.PlayerState != Scr_PlayerStateController.State.BabyBox)
                {
                    Vector3 dir = transform.forward + transform.up;
                    closestPlayer.GetComponent<Scr_CharacterController>().AddImpact(dir, m_PunchingPower);
                    closestPlayer.GetComponent<Scr_Combat>().SetHitBy(gameObject);
                    Scr_AudioManager.SetRandomPitch("PlayerPunch");
                    Scr_AudioManager.Play("PlayerPunch");
                    Scr_ScoreManager.UpdateScore(gameObject, 25);
                }
            }
        }  
    }

    private void Grab()
    {
        if (Input.GetButtonDown(m_Input.GetGrab()))         //For the love of god, refactor this
        {
            if (!m_PlayerState.IsHoldingObject)
            {
                m_ClosestObject = GetClosestObjectWithTag("ThrowableObject", 0.5f);

                if (m_ClosestObject != null)
                {
                    Scr_ThrowableObject throwable = m_ClosestObject.GetComponent<Scr_ThrowableObject>();
                    if (!throwable.IsHeld())
                    {
                        Debug.Log("Grabbed an object");
                        m_ClosestObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        m_ClosestObject.GetComponent<Rigidbody>().useGravity = false;
                        m_ClosestObject.transform.parent = transform;
                        m_ClosestObject.transform.forward = transform.forward;
                        m_ClosestObject.transform.localPosition = throwable.GetHoldPosition();
                        m_ClosestObject.transform.localEulerAngles = throwable.GetHoldRotation();
                        m_PlayerState.IsHoldingObject = true;
                        throwable.SetHold(true);

                        m_AnimationController.Animate("IsHoldingItem", true);
                    }
                    else
                        m_ClosestObject = null;
                }
            }
        }
    }

    private void Throw()
    {
        if (Input.GetButtonDown(m_Input.GetThrow()) && m_PlayerState.IsHoldingObject)
        {
            Rigidbody rigidBody = m_ClosestObject.GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.None;
            rigidBody.AddForce(transform.forward * m_ThrowingForce);
            rigidBody.useGravity = true;
            
            Scr_ThrowableObject throwable = m_ClosestObject.GetComponent<Scr_ThrowableObject>();
            throwable.SetThrownBy(gameObject);
            throwable.SetHold(false);
      
            m_PlayerState.IsHoldingObject = false;
            m_ClosestObject.transform.parent = null;
            m_ClosestObject = null;

            m_AnimationController.Animate("Throw");
            m_AnimationController.Animate("IsHoldingItem", false);
        }
    }

    private void Fire()
    {
        if (Input.GetButtonDown(m_Input.GetFire()))
        {
            Scr_EventManager.TriggerEvent("Fire_Projectile");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position + transform.forward * m_Length, 
                            Vector3.Scale(transform.localScale, m_HalfExtends));
    }

    public void ReleaseHeldObject()
    {
        m_ClosestObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        m_ClosestObject.GetComponent<Rigidbody>().useGravity = true;
        m_ClosestObject.transform.parent = null;
        m_PlayerState.IsHoldingObject = false;

        Scr_ThrowableObject throwable = m_ClosestObject.GetComponent<Scr_ThrowableObject>();
        throwable.SetHold(false);
        m_ClosestObject = null;
    }

    public void SetHitBy(GameObject player)
    {
        m_LastHitBy = player;
    }

    public GameObject GetLastHit()
    {
        return m_LastHitBy;
    }

    public void SetPunchingPower(float punchingPower)
    {
        m_PunchingPower = punchingPower;
    }

    public float GetPunchingPower()
    {
        return m_PunchingPower;
    }

    private GameObject GetClosestObjectWithTag(string tag, float boxScale = 1.0f)
    {
        GameObject closestObject = null;
        Vector3 position = m_PunchHitBox.transform.position;
        Vector3 scale = Vector3.Scale(transform.localScale * boxScale, m_HalfExtends);
        Collider[] hitColliders = Physics.OverlapBox(position, scale, Quaternion.identity);
        float previousDistance = float.MaxValue;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == tag && hitCollider.gameObject != gameObject)
            {
                float currentDistance = Vector3.Distance(hitCollider.transform.position, transform.position);
                if (currentDistance < previousDistance)
                {
                    previousDistance = currentDistance;
                    closestObject = hitCollider.gameObject;
                }
            }
        }
        return closestObject;
    }

    private void OnEnable()
    {
        Scr_EventManager.StartListening("Mom_Comes", () => { 
            if (m_Input != null)
                m_Input.VibrateIncrease(0.1f, 0.3f, 5.0f); 
        });
    }

    private void OnDisable()
    {
        Scr_EventManager.StopListening("Mom_Comes", () => {
            if (m_Input != null)
                m_Input.VibrateIncrease(0.1f, 0.3f, 5.0f);
        });
    }
}