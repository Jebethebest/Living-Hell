using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Projectile : MonoBehaviour
{    
    private float m_LaunchAngle = 45.0f;

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
	{
    }

    public void Fire(Vector3 target)
    {
        Vector3 launch = HitTargetByAngle(transform.position, target, Physics.gravity, m_LaunchAngle);

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(launch, ForceMode.Impulse);
    }

    Vector3 HitTargetByAngle(Vector3 startPosition, Vector3 targetPosition, Vector3 gravityBase, float limitAngle)
    {
        if (limitAngle == 90 || limitAngle == -90)
        {
            return Vector3.zero;
        }

        Vector3 AtoB = targetPosition - startPosition;
        Vector3 horizontal = GetHorizontalVector(AtoB, gravityBase);
        float horizontalDistance = horizontal.magnitude;
        Vector3 vertical = GetVerticalVector(AtoB, gravityBase);
        float verticalDistance = vertical.magnitude * Mathf.Sign(Vector3.Dot(vertical, -gravityBase));

        float angleX = Mathf.Cos(Mathf.Deg2Rad * limitAngle);
        float angleY = Mathf.Sin(Mathf.Deg2Rad * limitAngle);

        float gravityMag = gravityBase.magnitude;

        if (verticalDistance / horizontalDistance > angleY / angleX)
        {
            return Vector3.zero;
        }

        float destSpeed = (1 / Mathf.Cos(Mathf.Deg2Rad * limitAngle)) * Mathf.Sqrt((0.5f * gravityMag * horizontalDistance * horizontalDistance) / ((horizontalDistance * Mathf.Tan(Mathf.Deg2Rad * limitAngle)) - verticalDistance));
        Vector3 launch = ((horizontal.normalized * angleX) - (gravityBase.normalized * angleY)) * destSpeed;
        return launch;
    }

    Vector3 GetHorizontalVector(Vector3 AtoB, Vector3 gravityBase)
    {
        Vector3 output;
        Vector3 perpendicular = Vector3.Cross(AtoB, gravityBase);
        perpendicular = Vector3.Cross(gravityBase, perpendicular);
        output = Vector3.Project(AtoB, perpendicular);
        return output;
    }

    Vector3 GetVerticalVector(Vector3 AtoB, Vector3 gravityBase)
    {
        Vector3 output;
        output = Vector3.Project(AtoB, gravityBase);
        return output;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Puke hit!");
        Scr_EventManager.TriggerEvent("Projectile_Hit");
    }
}
