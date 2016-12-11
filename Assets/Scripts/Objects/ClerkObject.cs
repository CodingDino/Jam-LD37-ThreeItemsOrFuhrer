using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClerkObject : MonoBehaviour {

	[SerializeField]
	private Transform m_rightEdge;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (transform.position.x > m_rightEdge.position.x)
			transform.SetPosX(m_rightEdge.position.x);
	}
}
