using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterObject : MonoBehaviour {

	private CustomerObject m_currentCustomer;

	public CustomerObject currentCustomer { get { return m_currentCustomer; } set {m_currentCustomer = value; } }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
