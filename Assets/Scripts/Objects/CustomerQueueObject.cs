using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerQueueObject : MonoBehaviour {

	[SerializeField]
	private Transform m_queueFront;
	[SerializeField]
	private float m_queueSpacing;

	private List<CustomerObject> m_customerQueue = new List<CustomerObject>();

	public Vector2 EnterQueue(CustomerObject _customer)
	{
		m_customerQueue.Add(_customer);
		int queueIndex = m_customerQueue.Count - 1;
		Vector3 queuePos = m_queueFront.position + new Vector3(queueIndex*m_queueSpacing,0,0);
		return queuePos;
	}

	public void LeaveQueue(CustomerObject _customer)
	{
		m_customerQueue.Remove(_customer);
	}

	public Vector2 GetQueuePosition(CustomerObject _customer)
	{
		int queueIndex = 0;
		for (int i = 0; i < m_customerQueue.Count; ++i)
		{
			if (m_customerQueue[i] == _customer)
			{
				queueIndex = i;
				break;
			}
		}
		Vector3 queuePos = m_queueFront.position + new Vector3(queueIndex*m_queueSpacing,0,0);
		return queuePos;
	}

	public bool IsAtFrotOfQueue(CustomerObject _customer)
	{
		int queueIndex = 0;
		for (int i = 0; i < m_customerQueue.Count; ++i)
		{
			if (m_customerQueue[i] == _customer)
			{
				queueIndex = i;
				break;
			}
		}
		return queueIndex == 0;
	}
}
