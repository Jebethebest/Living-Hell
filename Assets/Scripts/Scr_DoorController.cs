using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_DoorController : MonoBehaviour 
{
    [SerializeField] private GameObject[] m_Doors;
    [SerializeField] private GameObject[] m_Moms;

    private int m_OpenDoorIndex = -1;
    private Scr_CheckRoom m_MomScript;
    private Transform[] m_MomPositions;
    private Animator[] m_DoorAnimators;
    private Animator[] m_MomAnimators;

    // Use this for initialization
    private void Start()
    {
        m_MomScript = GetComponent<Scr_CheckRoom>();   
        m_DoorAnimators = new Animator[m_Doors.Length];
        m_MomPositions = new Transform[m_Doors.Length];
        m_MomAnimators = new Animator[m_Doors.Length];

        for (int i = 0; i < m_Doors.Length; ++i)
        {
            m_DoorAnimators[i] = m_Doors[i].GetComponent<Animator>();
            m_MomAnimators[i] = m_Moms[i].GetComponent<Animator>();
            m_MomPositions[i] = m_Doors[i].transform.Find("MomPos");
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OpenRandomDoor()
    {
        if (m_OpenDoorIndex <= -1)
            StartCoroutine(PlayOpenDoorAnimation());     
    }

    private void CloseDoor()
    {
        StartCoroutine(PlayCloseDoorAnimation());
        StartCoroutine(SetMomInactive());
    }

    private IEnumerator PlayOpenDoorAnimation()
    {
        m_OpenDoorIndex = Random.Range(0, m_Doors.Length);
        Scr_AudioManager.Play("DoorOpens");
        m_DoorAnimators[m_OpenDoorIndex].SetBool("OpenDoor", true);

        yield return new WaitForSeconds(0.4f);

        m_Moms[m_OpenDoorIndex].SetActive(true);
        SpawnParticle();

        Transform momPos = m_MomPositions[m_OpenDoorIndex];
        m_MomScript.GetPlayerOutOfBox();
        m_MomScript.SetIsSeeking(true, momPos.position);

        Debug.Log("Door opens: " + m_OpenDoorIndex);
    }

    private IEnumerator PlayCloseDoorAnimation()
    {
        yield return new WaitForSeconds(1.0f);

        if (m_OpenDoorIndex >= 0)
            m_DoorAnimators[m_OpenDoorIndex].SetBool("OpenDoor", false);
        Scr_AudioManager.Play("DoorCloses");

        m_MomScript.SetIsSeeking(false, Vector3.zero);

        Debug.Log("Door closes");
    }

    private IEnumerator SetMomInactive()
    {
        yield return new WaitForSeconds(1.5f);

        Scr_EventManager.TriggerEvent("Mom_Leaves");
        if (m_OpenDoorIndex >= 0)
            m_Moms[m_OpenDoorIndex].SetActive(false);
        m_OpenDoorIndex = -1;
    }

    private void PlayMomAnimation()
    {
        m_MomAnimators[m_OpenDoorIndex].SetTrigger("Angry");

        Vector3 rotation = Vector3.zero;
        Vector3 pos = m_Moms[m_OpenDoorIndex].transform.position;
        pos.y += 2.0f;

        if (m_OpenDoorIndex == 1)
            rotation.y = 90.0f;

        Scr_ParticleManager.SpawnParticle("MomScreech", pos, Quaternion.Euler(rotation));
    }

    private void SpawnParticle()
    {
        if (m_OpenDoorIndex >= 0)
        {
            Vector3 pos = m_Moms[m_OpenDoorIndex].transform.position;
            pos.y += 1.5f;

            if (m_OpenDoorIndex == 0)
                pos.z += 1.5f;
            else
                pos.x += 1.5f;

            Scr_ParticleManager.SpawnParticle("MomSmokeSmall", pos, Quaternion.identity);
            Scr_ParticleManager.SpawnParticle("MomSmokeBig", pos, Quaternion.identity);
            Scr_ParticleManager.SpawnParticle("MomFragments", pos, Quaternion.identity);
        }
    }

    private void OnEnable()
    {
        Scr_EventManager.StartListening("Mom_In_Room", OpenRandomDoor);
        Scr_EventManager.StartListening("Mom_Is_Leaving", CloseDoor);
        Scr_EventManager.StartListening("PlayerPunish", PlayMomAnimation);
    }

    private void OnDisable()
    {
        Scr_EventManager.StopListening("Mom_In_Room", OpenRandomDoor);
        Scr_EventManager.StopListening("Mom_Is_Leaving", CloseDoor);
    }
}
