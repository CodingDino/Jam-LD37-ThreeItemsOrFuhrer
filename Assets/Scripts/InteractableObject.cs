// ************************************************************************ 
// File Name:   InteractableObject.cs 
// Purpose:    	Runs a script based on the interact button being pressed near this item
// Project:		Ludum Dare 37
// Author:      Sarah Herzog  
// Copyright: 	2016 Bounder Games
// ************************************************************************ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InteractableObject : MonoBehaviour 
{

	// ********************************************************************
	// Private Data Members 
	// ********************************************************************
	[SerializeField]
	private string m_buttonInteract = "";
	[SerializeField]
	private bool m_useMouseClick = true;
	[SerializeField]
	private GameObject m_requireNearbyObject = null;
	[SerializeField]
	private float m_interactionRadius = 3.0f;
	[SerializeField]
	public UnityEvent m_onInteract = null;

	// ********************************************************************

	void OnMouseDown()
	{
		if (m_onInteract == null)
			return;

		if (m_useMouseClick)
		{
			TryInteract();
		}
	}

	// ********************************************************************

	void Update()
	{
		if (m_onInteract == null)
			return;
		
		if (!m_buttonInteract.NullOrEmpty() && Input.GetKeyDown(m_buttonInteract))
		{
			TryInteract();
		}
	}

	// ********************************************************************

	private bool TryInteract()
	{
		bool objectNear = false;
		if (m_requireNearbyObject != null)
			objectNear = (m_requireNearbyObject.transform.position - transform.position).magnitude <= m_interactionRadius;

		if (m_requireNearbyObject == null || objectNear)
		{
			m_onInteract.Invoke();
			return true;
		}

		return false;
	}

	// ********************************************************************

}
