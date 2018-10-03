using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PowerUp : MonoBehaviour
{
    public enum Type
    {
        Empty, ExtraSpeed, Knockback, MultiJump
    }

    [SerializeField] private Type m_Type;
    private float m_RotationSpeed = 20.0f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		transform.Rotate(0, m_RotationSpeed * Time.deltaTime, 0);
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x, position.y + Mathf.Sin(Time.fixedTime) / 300, position.z);
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Scr_PlayerPowerUpHandler powerupHandle = other.gameObject.GetComponent<Scr_PlayerPowerUpHandler>();
            if (powerupHandle.PickupPowerup(m_Type))
            {
                Destroy(gameObject);
                Scr_ParticleManager.SpawnParticle("PowerupPop", gameObject.transform.position, Quaternion.identity);
            }
        }
    }
}
