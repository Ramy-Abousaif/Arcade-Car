using System.Collections.Generic;
using UnityEngine;

public class CarSuspension : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Rigidbody m_RigidBody;

    [SerializeField]
    private List<Transform> m_SuspensionTargets;

    [Header("Parameters")]
    [SerializeField]
    private float m_ForceAmount;

    [SerializeField]
    private float m_SuspensionLength;

    [Header("Visualization")]
    [SerializeField]
    private List<Transform> m_TireTransforms;

    public bool isGrounded { get; private set; }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        List<float> list = new List<float>();
        isGrounded = false;
        foreach (Transform suspensionTarget in m_SuspensionTargets)
        {
            if (Physics.Raycast(suspensionTarget.position, -suspensionTarget.up, out var hitInfo, m_SuspensionLength))
            {
                isGrounded = true;
                Debug.DrawLine(suspensionTarget.position, hitInfo.point, Color.green);
                float num = Vector3.Distance(suspensionTarget.position, hitInfo.point);
                float num2 = (m_SuspensionLength - num) / m_SuspensionLength;
                m_RigidBody.AddForceAtPosition(suspensionTarget.up * m_ForceAmount * num2, suspensionTarget.position, ForceMode.Acceleration);
                list.Add(num);
            }
            else
            {
                Debug.DrawLine(suspensionTarget.position, suspensionTarget.position + -suspensionTarget.up * m_SuspensionLength, Color.red);
                list.Add(m_SuspensionLength);
            }
        }
        UpdateTireYOffset(list);
    }

    private void UpdateTireYOffset(List<float> offsets)
    {
        for (int i = 0; i < m_TireTransforms.Count; i++)
        {
            m_TireTransforms[i].localPosition = new Vector3(m_TireTransforms[i].localPosition.x, 0f - offsets[i], m_TireTransforms[i].localPosition.z);
        }
    }
}