using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CustomerState
{
	ENTERING,
	GATHERING_ITEM,
	LOITERING,
	QUEUEING,
	CHECKING_OUT,
	LEAVING
}

[RequireComponent (typeof(Entity))]
public class CustomerObject : MonoBehaviour 
{
	private const int MAX_INVENTORY_ITEMS = 3;
	private const float LOITERING_TIME = 3;
	private const float LOITERING_CHANCE = 0.3f; 
	private const float LOITERING_RANGE = 10f;
	private const float SAFE_ZONE = 5f;

	[SerializeField]
	private List<Transform> m_productSlots = new List<Transform>();
	[SerializeField]
	private Entity m_entity;

	private List<string> m_targetItems = new List<string>();
	private List<string> m_pendingItems = new List<string>();
	private List<ProductObject> m_productInventory = new List<ProductObject>();
	private CustomerState m_state;
	private float m_loiteringStartTime;
	private string m_currentTargetItem;

	// SHARED DATA
	// TODO: Make static and set up
	[SerializeField]
	private List<ShelfObject> s_shelves;
	[SerializeField]
	private Transform s_door;
	[SerializeField]
	private RegisterObject s_register;
	[SerializeField]
	private CustomerQueueObject s_queue;

	public static void SetCustomerSharedData()
	{
		// TODO: POPULATE SHARED DATA
	}

	void Start()
	{
		ChangeState(CustomerState.ENTERING);
		SetupCustomer();
	}

	public void SetupCustomer()
	{
		// Determine how many items a customer wants
		int numItems = Random.Range(1,MAX_INVENTORY_ITEMS);

		// Generate items in stock
		for (int i = 0; i < numItems; ++i)
		{
			int itemIndex = Random.Range(0,s_shelves.Count);
			m_targetItems.Add(s_shelves[itemIndex].productID);
			m_pendingItems.Add(s_shelves[itemIndex].productID);
		}

		// TODO: Generate vouchers based on active rules
		// TODO: Determine if customer should be failed or not
		// TODO: Generate inconsistencies if customer should be failed
	}

	public void AddItemToInventory(string _sItemID)
	{
		ProductObject newObject = ProductObject.Create(_sItemID);
		newObject.transform.SetParent(transform);
		newObject.transform.position = m_productSlots[m_productInventory.Count].transform.position;
		m_productInventory.Add(newObject);
	}

	private bool ShouldLoiter()
	{
		float fRand = Random.Range(0f,1f);
		return fRand < LOITERING_CHANCE;
	}

	private void ChangeState(CustomerState _newState)
	{
		m_state = _newState;
		switch(m_state)
		{
		case CustomerState.ENTERING:
			{
				Vector3 enterTarget = new Vector3(s_door.transform.position.x + SAFE_ZONE, transform.position.y, transform.position.z);
				m_entity.MoveToTarget(enterTarget);
				break;
			}
		case CustomerState.GATHERING_ITEM:
			{
				int randomItem = Random.Range(0,m_pendingItems.Count-1);
				m_currentTargetItem = m_pendingItems[randomItem];
				ShelfObject targetShelf = null;
				for (int i = 0; i < s_shelves.Count; ++i)
				{
					if (s_shelves[i].productID == m_currentTargetItem)
					{
						targetShelf = s_shelves[i];
						break;
					}
				}
				Vector3 shelfTarget = new Vector3(targetShelf.transform.position.x,transform.position.y,transform.position.z);
				m_entity.MoveToTarget(shelfTarget);
				break;
			}
		case CustomerState.LOITERING:
			{
				float loiteringIncrement = Random.Range(-LOITERING_RANGE, LOITERING_RANGE);
				Vector3 loiteringTarget = transform.position + new Vector3(loiteringIncrement,0,0);
				loiteringTarget.x = Mathf.Clamp(loiteringTarget.x, s_door.transform.position.x + SAFE_ZONE, s_register.transform.position.x - SAFE_ZONE);
				m_entity.MoveToTarget(loiteringTarget);
				m_loiteringStartTime = Time.time;
				break;
			}
		case CustomerState.QUEUEING:
			{
				// Move to place in Queue.
				Vector3 target = s_queue.EnterQueue(this);
				m_entity.MoveToTarget(target);
				break;
			}
		case CustomerState.CHECKING_OUT:
			{
				// Interact with clerk
				s_register.currentCustomer = this;
				break;
			}
		case CustomerState.LEAVING:
			{
				s_queue.LeaveQueue(this);
				Vector3 exitTarget = new Vector3(s_door.transform.position.x - SAFE_ZONE, transform.position.y, transform.position.z);
				m_entity.MoveToTarget(exitTarget);
				break;
			}
		}
	}

	void Update()
	{
		// TODO: Set animation parameter for walking

		switch(m_state)
		{
		case CustomerState.ENTERING:
			{
				if (m_entity.moveTargetArrived)
				{
					if (ShouldLoiter())
						ChangeState(CustomerState.LOITERING);
					else 
						ChangeState(CustomerState.GATHERING_ITEM);
				}
				break;
			}
		case CustomerState.GATHERING_ITEM:
			{
				if (m_entity.moveTargetArrived)
				{
					// TAKE ITEM
					ShelfObject targetShelf = null;
					for (int i = 0; i < s_shelves.Count; ++i)
					{
						if (s_shelves[i].productID == m_currentTargetItem)
						{
							targetShelf = s_shelves[i];
							break;
						}
					}
					targetShelf.AddOrRemoveStock(-1);
					AddItemToInventory(m_currentTargetItem);
					m_pendingItems.Remove(m_currentTargetItem);

					if (ShouldLoiter())
						ChangeState(CustomerState.LOITERING);
					else if (m_pendingItems.Count > 0)
						ChangeState(CustomerState.GATHERING_ITEM);
					else
						ChangeState(CustomerState.QUEUEING);
				}
				break;
			}
		case CustomerState.LOITERING:
			{
				if (Time.time >= m_loiteringStartTime + LOITERING_TIME )
				{
					if (ShouldLoiter())
						ChangeState(CustomerState.LOITERING);
					else if (m_pendingItems.Count > 0)
						ChangeState(CustomerState.GATHERING_ITEM);
					else
						ChangeState(CustomerState.QUEUEING);
				}
				break;
			}
		case CustomerState.QUEUEING:
			{
				// If in front of queue, move to checking out state
				if (!m_entity.isMoving && s_queue.IsAtFrotOfQueue(this))
				{
					ChangeState(CustomerState.CHECKING_OUT);
				}
				break;
			}
		case CustomerState.CHECKING_OUT:
			{
				if (s_register.currentCustomer != this)
				{
					ChangeState(CustomerState.LEAVING);
				}
				break;
			}
		case CustomerState.LEAVING:
			{
				if (m_entity.moveTargetArrived)
				{
					Destroy(gameObject);
				}
				break;
			}
		}
	}
}
