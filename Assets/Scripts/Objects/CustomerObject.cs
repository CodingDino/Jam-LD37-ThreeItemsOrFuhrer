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
	private ProductObject m_productPrototype;

	private List<string> m_targetItems = new List<string>();
	private List<string> m_pendingItems = new List<string>();
	private List<ProductObject> m_productInventory = new List<ProductObject>();
	private CustomerState m_state;
	private float m_loiteringStartTime;
	private Entity m_entity;
	private string m_currentTargetItem;

	// SHARED DATA
	private static List<ShelfObject> s_shelves;
	private static Transform s_door;
	private static Transform s_register;

	public static void SetCustomerSharedData()
	{
		// TODO: POPULATE SHARED DATA
	}

	public void SetupCustomer(List<string> _productsInStock)
	{
		// Determine how many items a customer wants
		int numItems = Random.Range(1,MAX_INVENTORY_ITEMS);

		// Generate items in stock
		for (int i = 0; i < numItems; ++i)
		{
			int itemIndex = Random.Range(0,_productsInStock.Count);
			m_targetItems.Add(_productsInStock[itemIndex]);
		}
		m_pendingItems = m_targetItems;

		// TODO: Generate vouchers based on active rules
		// TODO: Determine if customer should be failed or not
		// TODO: Generate inconsistencies if customer should be failed
	}

	public void AddItemToInventory(string _sItemID)
	{
		ProductObject newObject = Instantiate(m_productPrototype.gameObject).GetComponent<ProductObject>();
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
				int randomItem = Random.Range(0,m_pendingItems.Count);
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
				// TODO: Move to place in Queue.
				break;
			}
		case CustomerState.CHECKING_OUT:
			{
				// TODO: Interact with clerk
				break;
			}
		case CustomerState.LEAVING:
			{
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
					else 
						ChangeState(CustomerState.GATHERING_ITEM);
				}
				break;
			}
		case CustomerState.LOITERING:
			{
				if (Time.time >= m_loiteringStartTime + LOITERING_TIME )
				{
					if (ShouldLoiter())
						ChangeState(CustomerState.LOITERING);
					else if (m_targetItems.Count > m_productInventory.Count)
						ChangeState(CustomerState.GATHERING_ITEM);
					else
						ChangeState(CustomerState.QUEUEING);
				}
				break;
			}
		case CustomerState.QUEUEING:
			{
				// TODO: Move to place in Queue.
				// TODO: If in front of queue, move to checking out state
				break;
			}
		case CustomerState.CHECKING_OUT:
			{
				// TODO: Interact with clerk
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
