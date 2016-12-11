using UnityEngine;
using System.Collections;

public class DialogueSystemTester : MonoBehaviour {

	public string m_conversationID;

	private bool m_loadedConversation = false;

	// Use this for initialization
	void Update () {
		if (!m_loadedConversation)
			m_loadedConversation = DialogueManager.instance.StartConversation(m_conversationID);
	}
}
