using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform followPoint;
    [SerializeField]
    private Rigidbody m_RigidBody;

    [SerializeField]
    private CarSuspension m_Suspension;

    [SerializeField]
    private List<TrailRenderer> m_SkidmarkRenderers;

    [SerializeField]
    private List<ParticleSystem> m_SmokeParticles;

    [Header("Parameter")]
    [Header("Normal Acc/Turn")]
    [SerializeField]
    private float m_Acc;

    [SerializeField]
    private float m_MaxSpeed;

    [SerializeField]
    private float m_TurnForce;

    [SerializeField]
    private float m_MaxAngularVelocity;

    [SerializeField]
    private float m_Grip;

    [Header("Drift Acc/Turn")]
    [SerializeField]
    private float m_DriftAcc;

    [SerializeField]
    private float m_MaxDriftSpeed;

    [SerializeField]
    private float m_DriftTurnForce;

    [SerializeField]
    private float m_MaxDriftAngularVelocity;

    [SerializeField]
    private float m_DriftGrip;

    [Header("Other")]
    [SerializeField]
    private float m_MinVelForDrift = 0.5f;

    private float m_AccInput;

    private float m_SteeringInput;

    //private CarControls m_Controlls;

    private bool m_IsDrivingForward;

    private bool m_SkidmarksActive;

    public Vector2 velocity { get; private set; }

    public float driftVal { get; private set; }

    public float speedVal { get; private set; }

    public bool isDrifting { get; private set; }

    private void OnValidate()
    {
    }

    private void OnEnable()
    {
        //m_Controlls.Enable();
    }

    private void OnDisable()
    {
        //m_Controlls.Disable();
    }

    private void Awake()
    {
        EndDrift();
    }

    private void Update()
    {
        m_AccInput = Input.GetAxis("Vertical");
        m_SteeringInput = Input.GetAxis("Horizontal");

        if(Input.GetKeyDown(KeyCode.Space))
            StartDrift();

        if (Input.GetKeyUp(KeyCode.Space))
            EndDrift();

        followPoint.position = transform.position;
    }

    private void FixedUpdate()
    {
        if (m_Suspension.isGrounded)
        {
            Vector3 vector = base.transform.forward;
            if (Physics.Raycast(base.transform.position, -base.transform.up, out var hitInfo, 100f))
            {
                Debug.DrawLine(base.transform.position, hitInfo.point, Color.cyan);
                vector = Quaternion.AngleAxis(Vector3.SignedAngle(base.transform.up, hitInfo.normal, base.transform.right), base.transform.right) * base.transform.forward;
                Debug.DrawLine(base.transform.position, base.transform.position + vector * 50f, Color.green);
            }
            if (Mathf.Abs(m_AccInput) > 0f)
            {
                float num = (isDrifting ? m_DriftAcc : m_Acc);
                m_RigidBody.AddForce(vector * num * m_AccInput, ForceMode.Acceleration);
            }
            m_IsDrivingForward = Vector3.Angle(m_RigidBody.velocity, vector) < Vector3.Angle(m_RigidBody.velocity, -vector);
            float num3 = (isDrifting ? m_MaxDriftSpeed : m_MaxSpeed);
            m_RigidBody.velocity = Vector3.ClampMagnitude(m_RigidBody.velocity, num3);
            speedVal = Mathf.InverseLerp(0f, num3, m_RigidBody.velocity.magnitude);
            if (m_Suspension.isGrounded)
            {
                m_SteeringInput *= speedVal;
                if (!m_IsDrivingForward)
                {
                    m_SteeringInput = -m_SteeringInput;
                }
                float num4 = (isDrifting ? m_DriftTurnForce : m_TurnForce);
                m_RigidBody.AddTorque(base.transform.up * m_SteeringInput * num4, ForceMode.Acceleration);
                float num5 = (isDrifting ? m_MaxDriftAngularVelocity : m_MaxAngularVelocity);
                m_RigidBody.angularVelocity = new Vector3(m_RigidBody.angularVelocity.x, num5 * m_SteeringInput, m_RigidBody.angularVelocity.z);
            }
            float value = Vector3.SignedAngle(m_RigidBody.velocity, base.transform.forward, Vector3.up);
            driftVal = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-80f, 80f, value));
            if (m_RigidBody.velocity.magnitude > m_MinVelForDrift)
            {
                m_RigidBody.AddForce(base.transform.right * driftVal * (isDrifting ? m_DriftGrip : m_Grip), ForceMode.Acceleration);
            }
            if (!m_SkidmarksActive && isDrifting)
            {
                m_SkidmarksActive = true;
                foreach (TrailRenderer skidmarkRenderer in m_SkidmarkRenderers)
                {
                    skidmarkRenderer.emitting = true;
                }
                foreach (ParticleSystem smokeParticles in m_SmokeParticles)
                {
                    smokeParticles.Play();
                }
            }
        }
        else if (m_SkidmarksActive && isDrifting)
        {
            m_SkidmarksActive = false;
            foreach (TrailRenderer skidmarkRenderer2 in m_SkidmarkRenderers)
            {
                skidmarkRenderer2.emitting = false;
            }
            foreach (ParticleSystem smokeParticles in m_SmokeParticles)
            {
                smokeParticles.Stop();
            }
        }
        velocity = m_RigidBody.velocity;
    }

    private void StartDrift()
    {
        isDrifting = true;
        foreach (TrailRenderer skidmarkRenderer in m_SkidmarkRenderers)
        {
            skidmarkRenderer.emitting = true;
        }
        foreach (ParticleSystem smokeParticles in m_SmokeParticles)
        {
            smokeParticles.Play();
        }
    }

    private void EndDrift()
    {
        isDrifting = false;
        foreach (TrailRenderer skidmarkRenderer in m_SkidmarkRenderers)
        {
            skidmarkRenderer.emitting = false;
        }
        foreach (ParticleSystem smokeParticles in m_SmokeParticles)
        {
            smokeParticles.Stop();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(m_RigidBody.centerOfMass + base.transform.position, 0.3f);
        Gizmos.color = (m_IsDrivingForward ? Color.green : Color.red);
        Gizmos.DrawSphere(m_RigidBody.position + m_RigidBody.transform.up * 2.5f, 0.3f);
    }
}
