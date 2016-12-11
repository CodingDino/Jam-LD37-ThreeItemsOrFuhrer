// ************************************************************************ 
// File Name:   DialogueManager.cs 
// Purpose:    	Control dialogue displayed in the dialogue panel
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BounderFramework;


// ************************************************************************ 
// Class: DialogueManager
// ************************************************************************ 
public class DialogueManager : Singleton<DialogueManager>
{

    // ********************************************************************
    // Exposed Data Members 
    // ********************************************************************
    [SerializeField]
    private string m_dialogueScriptPath;
    [SerializeField]
    private string m_NpcSettingsFilePath;
    [SerializeField]
    private string m_audioClipPath;
    [SerializeField]
    private string m_portraitPathLarge;
    [SerializeField]
    private Text m_textObject;
    [SerializeField]
    private Text m_portraitText;
    [SerializeField]
    private Text m_largePortraitText;
    [SerializeField]
    private float m_defaultTextSpeed = 3.0f;
    [SerializeField]
    private int m_numCharsPerLine = 49;
//    [SerializeField]
//    private int m_numCharsPerLinePortrait = 30;
    [SerializeField]
    private AudioSource m_audioSource;
    [SerializeField]
    private GameObject m_waitingIcon;
    [SerializeField]
    private Canvas m_canvas;
    [SerializeField]
    private Animator[] m_largePortrait = new Animator[2]; // TODO: Large and small
    [SerializeField]
	private Animator[] m_characterAnimator = new Animator[2];
	[SerializeField]
	private Transform[] m_characterRoots = new Transform[2];
    [SerializeField]
    private GameObject m_choiceRoot = null;
    [SerializeField]
    private GameObject m_choiceButtonPrototype = null;
    [SerializeField]
	private float m_choicePopInDelay = 0.05f;
	[SerializeField]
	private Animator m_panelAnimator;
    

    // ********************************************************************
    // Private Data Members 
    // ********************************************************************
    private List<DialogueConversation> m_conversations = new List<DialogueConversation>();
    private Dictionary<string, DialogueConversation> m_conversationsMap = new Dictionary<string, DialogueConversation>();
    private DialogueConversation m_currentConversation;
    private DialogueFrame m_currentFrame;
    private int m_sectionIndex;
    private DialogueSection m_currentSection;
    private Dictionary<string, DialogueNPCSettings> m_NPCSettings = new Dictionary<string, DialogueNPCSettings>();
    private bool m_dialogueLoaded = false;
    private StringBuilder m_currentDisplayString;
    private int m_displayIndex = 0;
    private bool m_shouldSkip = false;
    private bool m_waitingForNextFrame = false;
    private bool m_waitingForChoiceInput = false;
    private int m_numCharSinceLastNewline;
    private AudioClip m_audioClip;
    private string m_audioClipName;
    private int m_frameCount = 0;
    private List<Animator> m_choices = new List<Animator>();

	void Awake()
	{
		s_instance = this;
	}


    // ********************************************************************
    // Function:	Start()
    // Purpose:		Run when new instance of the object is created.
    // ********************************************************************
    void Start()
	{
		LoadDialogueFilesInFolder("");
    }


    // ********************************************************************
    // Function:	Update()
    // Purpose:		Run once per frame.
    // ********************************************************************
    void Update()
    {
        if (Input.GetButtonUp("Advance") || Input.GetMouseButtonDown(0))
        {
            if (m_waitingForNextFrame)
            {
                m_waitingForNextFrame = false;
                m_waitingIcon.SetActive(false);
                if (m_currentFrame.endOnThisFrame)
                {
                    for (int i = 0; i < 2; ++i)
					{
                        m_largePortrait[i].SetBool("Shown", false);
                    }
                    // TODO: HACK! FIX! (send event instead)
//					if (StationPanelManager.instance)
//                    	StationPanelManager.instance.Back();

					if (m_panelAnimator != null)
					{
						m_panelAnimator.SetBool("Hidden", true);
					}
                }
                else
                {
                    // Choose next frame based on which frames we meet the requirements for
                    for (int i = 0; i < m_currentFrame.links.Count; ++i )
					{
						FollowLink(i);
						break;
                    }
                }
            }
            else
            {
                m_shouldSkip = true;
            }
        }
        // TODO: Select next frame based on choice made.
        // TODO: Save choice made if marked to be saved.
    }


    // ********************************************************************
    // Function:	LoadDialogueFilesInFolder()
    // Purpose:		Loads a set of DialogueConversations from a folder
    // ********************************************************************
    public void LoadDialogueFilesInFolder(string _folderName)
    {
		Debug.Log("DialogueManager --- Loading dialogue files in " + m_dialogueScriptPath + _folderName + "...");

        Object[] files = Resources.LoadAll(m_dialogueScriptPath + _folderName);
        if (files == null)
        {
			Debug.LogError("DialogueManager --- No dialogue files found: " + m_dialogueScriptPath + _folderName);
            return;
        }

		Debug.Log("DialogueManager --- found " + files.Length + " dialogue files in " + m_dialogueScriptPath + _folderName);
        for (int i = 0; i < files.Length; ++i)
        {
            LoadDialogue(files[i] as TextAsset);
        }
    }


    // ********************************************************************
    // Function:	LoadDialogueFromFile()
    // Purpose:		Loads a set of DialogueConversations from file
    // ********************************************************************
    public void LoadDialogueFromFile(string _fileName)
    {
        Debug.Log("Loading dialogue file " + m_dialogueScriptPath + _fileName + "...");
        TextAsset dialogueFile = Resources.Load(m_dialogueScriptPath + _fileName ) as TextAsset;
        if (dialogueFile == null)
        {
            Debug.LogError("No dialogue file found: " + m_dialogueScriptPath + _fileName);
            return;
        }

        LoadDialogue(dialogueFile);
    }


    // ********************************************************************
    // Function:	LoadDialogue()
    // Purpose:		Loads a set of DialogueConversations
    // ********************************************************************
    public void LoadDialogue(TextAsset _dialogueFile)
    {
        string jsonString = _dialogueFile.text;
		Debug.Log("DialogueManager --- Dialogue JSON String loaded: " + jsonString);

        if (jsonString != "")
        {
			JSON N = JSON.ParseString(jsonString);
			
			foreach (JSON C in N["conversations"])
			{
				DialogueConversation conversation = C.GetArchive<DialogueConversation>();
				m_conversationsMap[conversation.id] = conversation;
				m_conversations.Add(conversation);
			}
        }
	}


	// ********************************************************************
	public bool StartCurrentConversation()
	{
		if (m_panelAnimator != null)
		{
			m_panelAnimator.SetBool("Hidden", false);
		}

		m_currentFrame = m_currentConversation.frames[m_currentConversation.startingFrame];

		DisplayFrame();

		return true;
	}
	// ********************************************************************


    // ********************************************************************
    // Function:	StartConversation()
    // Purpose:		Determines a conversation to use and starts it
    // ********************************************************************
    public void StartConversation()
	{
        // Determine correct conversation
        if (m_conversations.Count == 0)
        {
            Debug.LogError("No conversations loaded!");
        }
        for (int i = 0; i < m_conversations.Count; ++i)
		{
			m_currentConversation = m_conversations[i];
			Debug.Log("DialogueManager --- Conversation loaded: "+m_currentConversation.id);
			break;
        }
        if (m_currentConversation == null) // Can't load any conversation!
        {
            Debug.LogError("Don't meet requirements for any conversations!");
            return; 
        }

        // Initialize stuff for new conversation
        m_currentFrame = m_currentConversation.frames[m_currentConversation.startingFrame];

        DisplayFrame();
	}
	
	
	// ********************************************************************
	// Function:	StartConversation()
	// Purpose:		Determines a conversation to use and starts it
	// ********************************************************************
	public bool StartConversation(string _conversationID)
	{
		if (!m_conversationsMap.ContainsKey(_conversationID))
			return false;
		
		if (m_panelAnimator != null)
		{
			m_panelAnimator.SetBool("Hidden", false);
		}

		m_currentConversation = m_conversationsMap[_conversationID];
		m_currentFrame = m_currentConversation.frames[m_currentConversation.startingFrame];

		DisplayFrame();

		return true;
	}



	// ********************************************************************
	// Function:	SetConversation()
	// Purpose:		Determines a conversation to use
	// ********************************************************************
	public bool SetConversation(List<string> _conversations)
	{
		// dirty hack
		LoadDialogueFilesInFolder("");

		// Determine correct conversation
		if (m_conversations.Count == 0)
		{
			Debug.LogError("No conversations loaded!");
			return false;
		}
		if (_conversations.Count == 0)
		{
			Debug.LogError("No conversations provided!");
			return false;
		}
		for (int i = 0; i < _conversations.Count; ++i)
		{
			if (!m_conversationsMap.ContainsKey(_conversations[i]))
			{
				Debug.LogError("DialogueManager --- Conversation not found in map: "+_conversations[i]);
				continue;
			}

			m_currentConversation = m_conversationsMap[_conversations[i]];
			Debug.Log("DialogueManager --- Conversation loaded: "+_conversations[i]);
			break;
		}
		if (m_currentConversation == null) // Can't load any conversation!
		{
			Debug.LogError("DialogueManager --- Don't meet requirements for any conversations!");
			return false; 
		}

		if (m_panelAnimator != null)
		{
			m_panelAnimator.SetBool("Hidden", false);
		}

		// Initialize stuff for new conversation
		m_currentFrame = m_currentConversation.frames[m_currentConversation.startingFrame];

		return true;
		
	}

	// ********************************************************************
	// Function:	StartConversation()
	// Purpose:		Determines a conversation to use and starts it
	// ********************************************************************
	public bool StartConversation(List<string> _conversations)
	{
		// dirty hack
		LoadDialogueFilesInFolder("");

		// Determine correct conversation
		if (m_conversations.Count == 0)
		{
			Debug.LogError("No conversations loaded!");
			return false;
		}
		if (_conversations.Count == 0)
		{
			Debug.LogError("No conversations provided!");
			return false;
		}
		for (int i = 0; i < _conversations.Count; ++i)
		{
			if (!m_conversationsMap.ContainsKey(_conversations[i]))
			{
				Debug.LogError("DialogueManager --- Conversation not found in map: "+_conversations[i]);
				continue;
			}

			m_currentConversation = m_conversationsMap[_conversations[i]];
			Debug.Log("DialogueManager --- Conversation loaded: "+_conversations[i]);
			break;
		}
		if (m_currentConversation == null) // Can't load any conversation!
		{
			Debug.LogError("DialogueManager --- Don't meet requirements for any conversations!");
			return false; 
		}

		if (m_panelAnimator != null)
		{
			m_panelAnimator.SetBool("Hidden", false);
		}

		// Initialize stuff for new conversation
		m_currentFrame = m_currentConversation.frames[m_currentConversation.startingFrame];

		DisplayFrame();

		return true;
	}


    // ********************************************************************
    // Function:	DisplayFrame()
    // Purpose:		Removes current text on frame and shows new frame
    // ********************************************************************
    private void DisplayFrame()
	{
		m_shouldSkip = false;

        // Initialize stuff for new frame
        m_sectionIndex = 0;
        m_numCharSinceLastNewline = 0;
        m_textObject.text = "";
        m_currentDisplayString = new StringBuilder();
        ++m_frameCount;

        // Load correct portrait
        int side = -1;
        DialoguePortraitSettings portraitSettings = m_currentFrame.portraitSettings;
        if (portraitSettings != null && portraitSettings.active)
        {
			side = (int) portraitSettings.position;
			// TODO: Small portraits
            m_largePortrait[side].SetBool("Shown", true);

			if (m_characterAnimator[side] != null)
			{
				GameObject.Destroy(m_characterAnimator[side].gameObject);
				m_characterAnimator[side] = null;
			}

			GameObject characterObject = GameObject.Instantiate(Resources.Load<GameObject>(m_portraitPathLarge + portraitSettings.image));
			Vector3 localPos = characterObject.transform.localPosition;
			characterObject.transform.parent = m_characterRoots[side].transform;
			characterObject.transform.localPosition = localPos;
			characterObject.transform.localScale = new Vector3(Mathf.Abs(characterObject.transform.localScale.x), characterObject.transform.localScale.y, characterObject.transform.localScale.z);
			m_characterAnimator[side] = characterObject.GetComponent<Animator>();
        }
        for (int i = 0; i < 2; ++i)
		{
			if (i != side)
			{
				// TODO: Small portraits
				m_largePortrait[i].SetBool("Shown", false);
			}
        }
        StartCoroutine(DisplaySection());
    }


    // ********************************************************************
    // Function:	DisplaySection()
    // Purpose:		Performs actions described in a section
    // ********************************************************************
    private IEnumerator DisplaySection()
	{
        // Initialize stuff for new section
        m_currentSection = m_currentFrame.sections[m_sectionIndex];
        m_displayIndex = 0;

        // Set text settings
        DialogueTextSettings textSettings = m_currentSection.textSettings;
        if (textSettings.textAudio != m_audioClipName)
        {
            m_audioClipName = textSettings.textAudio;
            m_audioClip = Resources.Load(m_audioClipPath + m_audioClipName) as AudioClip;
            m_audioSource.clip = m_audioClip;
        }

        // TODO: Set portrait settings

        // Print text until we're done
        if (m_currentSection.text != null)
        {
            float textSpeed = m_defaultTextSpeed * textSettings.textSpeed;
			float secondsToWait = 1.0f / textSpeed;

			int side = -1;
			DialoguePortraitSettings portraitSettings = m_currentFrame.portraitSettings;

			if (portraitSettings != null && portraitSettings.active)
			{
				side = (int) portraitSettings.position;
				// Set portrait emotion
				m_characterAnimator[side].SetInteger("Emotion", (int)portraitSettings.emotion);
				// Set portrait to talking animation
				m_characterAnimator[side].SetBool("Talk", true);
			}

            while (m_displayIndex < m_currentSection.text.Length)
            {
				PrintText();
				if (!m_shouldSkip)
					yield return new WaitForSeconds(secondsToWait);
			}

			// Set portrait to idle animation
			if (portraitSettings != null && portraitSettings.active)
			{
				side = (int) portraitSettings.position;
				m_characterAnimator[side].SetBool("Talk", false);
			}
		}
		
		// TODO: Trigger special animations and effects
        // TODO: Wait for animation to finish if we triggered a special animation.

        // TODO: Some kind of manual "wait" system? (for cutscenes)

        // Load next section
        ++m_sectionIndex;
        if (m_sectionIndex < m_currentFrame.sections.Count)
        {
            StartCoroutine(DisplaySection());
        }
        else
        {
            // TODO: Bring up choices if applicable
            m_shouldSkip = false;

            if (m_currentFrame.displayChoices)
            {
                m_waitingForChoiceInput = true;

                List<int> validLinks = new List<int>();
                for (int i = 0; i < m_currentFrame.links.Count; ++i)
				{
					validLinks.Add(i);
                }
//                Debug.Log("Choices found for frame " + m_currentFrame.id + ": " + validLinks.Count);
                for (int i = 0; i < validLinks.Count; ++i)
                {
                    int index = validLinks[i];
                    DialogueLink link = m_currentFrame.links[index];
//                    Debug.Log("Creating button for "+index+" link conv: " + link.linkedConversation + " frame: " + link.linkedFrame);
                    GameObject choiceButton = GameObject.Instantiate(m_choiceButtonPrototype) as GameObject;
                    choiceButton.transform.SetParent(m_choiceRoot.transform);
                    choiceButton.GetComponentInChildren<Text>().text = link.text;
                    AddListenerForChoice(choiceButton.transform.GetComponentInChildren<Button>(), index);
                    m_choices.Add(choiceButton.GetComponent<Animator>());
                }

                StartCoroutine(HideChoices(false));
            }
            else
            {
                m_waitingForNextFrame = true;
                m_waitingIcon.SetActive(true);
            }
        }

        yield return null;
    }


    // ********************************************************************
    // Function:	AddListenerForChoice()
    // Purpose:		Adds a listener cause lambdas are stupid
    // ********************************************************************
    private void AddListenerForChoice(Button _button, int _linkIndex)
    {
        _button.onClick.AddListener(() => FollowLink(_linkIndex));
    }


    // ********************************************************************
    // Function:	FollowLink()
    // Purpose:		Applies the given link
    // ********************************************************************
    private void FollowLink(int _index)
    {
        if (_index >= m_currentFrame.links.Count)
        {
            Debug.LogError("Index " + _index + " is out of range for frame links from frame " + m_currentFrame.id);
        }

        DialogueLink link = m_currentFrame.links[_index];
        FollowLink(link.linkedConversation, link.linkedFrame);
    }   
    private void FollowLink(string _linkedConv, string _linkedFrame)
    {
        if (_linkedConv != null && _linkedConv != "")
        {
            m_currentConversation = m_conversationsMap[_linkedConv];
            m_currentFrame = m_currentConversation.frames[m_currentConversation.startingFrame];
        }

        if (_linkedFrame != null && _linkedFrame != "")
        {
            m_currentFrame = m_currentConversation.frames[_linkedFrame];
        }

        DisplayFrame();

        if (m_waitingForChoiceInput)
        {
            m_waitingForChoiceInput = false;
            StartCoroutine(HideChoices(true));
        }
    }


    // ********************************************************************
    // Function:	HideChoices()
    // Purpose:		Hides or shows choices
    // ********************************************************************
    private IEnumerator HideChoices(bool _hide)
    {
        for (int i = 0; i < m_choices.Count; ++i)
        {
            m_choices[i].SetBool("Hidden", _hide);
            yield return new WaitForSeconds(m_choicePopInDelay);
        }

        if (_hide)
        {
            yield return new WaitForSeconds(1.0f);
            for (int i = 0; i < m_choices.Count; ++i)
            {
                GameObject.Destroy(m_choices[i].gameObject);
            }
            m_choices.Clear();
        }
        
        yield return null;
    }


    // ********************************************************************
    // Function:	PrintText()
    // Purpose:		Prints the next character of text
    // ********************************************************************
    private void PrintText()
    {
		// TODO: Handling for rich text tags (<color> etc)

        char currentChar = m_currentSection.text[m_displayIndex];
        if (currentChar == ' ' )
        {
            // Insert lines for wrapping between words
            // When at a space, check number of characters til next space, 
            //     if that pushes us over the limit, insert a newline instead 
            //     of a space here.
            int indexToCheck = m_displayIndex+1;
            while (indexToCheck + 1 < m_currentSection.text.Length && m_currentSection.text[indexToCheck] != ' ')
            {
                ++indexToCheck;
            }
            if (m_numCharSinceLastNewline + (indexToCheck - m_displayIndex) > m_numCharsPerLine) // TODO - num char per line different if portrait present
            {
                currentChar = '\n'; // We will wrap when this word is fully printed, so put in a newline
            }
        }

        if (currentChar == '\n')
            m_numCharSinceLastNewline = 0;
        else
            ++m_numCharSinceLastNewline;

        m_currentDisplayString.Append(currentChar);
        m_textObject.text = m_currentDisplayString.ToString();

        ++m_displayIndex;

        // Play a sound
        if (currentChar != '\n' && currentChar != ' ' && !m_shouldSkip)
        {
            float randomPitchVariation = Mathf.PerlinNoise(((float)m_displayIndex) * 0.1f, (float)m_frameCount);
            m_audioSource.pitch = m_currentSection.textSettings.textPitch + randomPitchVariation * m_currentSection.textSettings.textPitchVariation;
            m_audioSource.Play();
        }
    }


    // ********************************************************************
    // Function:	FetchNPCSettings()
    // Purpose:		Returns settings for an NPC, loads from file if needed
    // ********************************************************************
    public static DialogueNPCSettings FetchNPCSettings(string _NPCName)
    {
        LoadNPCSettings();
        if (instance.m_NPCSettings.ContainsKey(_NPCName))
        {            
            return instance.m_NPCSettings[_NPCName];
        }
        return null;
    }


    // ********************************************************************
    // Function:	LoadNPCSettings()
    // Purpose:		Loads NPC Settings from file
    // ********************************************************************
    public static void LoadNPCSettings()
    {
        if (!instance.m_dialogueLoaded)
        {
            instance.m_dialogueLoaded = true;
            TextAsset NPCFile = Resources.Load(instance.m_NpcSettingsFilePath) as TextAsset;
            string jsonString = NPCFile.text;
			Debug.Log("DialogueManager --- NPC JSON String loaded: " + jsonString);

            if (jsonString != "")
			{
				JSON N = JSON.ParseString(jsonString);
				
				foreach (JSON NPC in N["NPCs"])
				{
					DialogueNPCSettings newNPC = NPC.GetArchive<DialogueNPCSettings>();
					instance.m_NPCSettings[newNPC.id] = newNPC;
				}

				Debug.Log("DialogueManager --- Loaded dialogue settings for " + instance.m_NPCSettings.Count + " NPCs");
            }
        }
    }


	// ********************************************************************
	// Function:	Hide()
	// Purpose:		Hides portraits and panel
	// ********************************************************************
    public void Hide()
    {
        m_waitingIcon.SetActive(false);
        m_textObject.text = "";
        StartCoroutine(HideChoices(true));
        for (int i = 0; i < 2; ++i)
		{
            m_largePortrait[i].SetBool("Shown", false);
        }
    }
}
