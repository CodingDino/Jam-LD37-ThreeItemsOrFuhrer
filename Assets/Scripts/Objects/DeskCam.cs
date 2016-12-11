using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskCam : MonoBehaviour {

	[SerializeField]
	Camera m_camera = null;
	[SerializeField]
	float m_speed = 0;
	[SerializeField]
	CustomerQueueObject m_customerQueue = null;
	[SerializeField]
	private Transform m_clerkPosition;
	[SerializeField]
	private Transform m_interactLocation;
	[SerializeField]
	private float m_interactDistance = 0;


	bool m_shown = false;
	bool m_customerPresent = false;
	bool m_clerkPresent = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		m_customerPresent = m_customerQueue.IsCustomerAtFrontOfQueue();
		m_clerkPresent = (m_interactLocation.position - m_clerkPosition.position).magnitude <= m_interactDistance;

		m_shown = m_customerPresent && m_clerkPresent;

		Rect viewport = m_camera.rect;

		if (m_shown && viewport.x < 0)
			viewport.x += m_speed * Time.deltaTime;
		else if (!m_shown && viewport.x > -0.6)
			viewport.x -= m_speed * Time.deltaTime;

		viewport.x = Mathf.Clamp(viewport.x,-0.6f,0f);
		
		m_camera.rect = viewport;
	}
}
