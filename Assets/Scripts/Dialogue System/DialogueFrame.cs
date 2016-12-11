// ************************************************************************ 
// File Name:   DialogueFrame.cs 
// Purpose:    	Information about a frame of dialogue
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections.Generic;
using BounderFramework;


// ************************************************************************ 
// Class: DialogueFrame
// ************************************************************************ 
[System.Serializable]
public class DialogueFrame : Archive {

    public string id;

    // Linking
    public bool endOnThisFrame = false;
    public bool displayChoices = false;
	public List<DialogueLink> links = new List<DialogueLink>();

    // Overrides (will be set to parent values if no override present) 
    public bool allowSkip = true;
    public bool waitForInput = true;
    public DialoguePortraitSettings portraitSettings;
    public DialogueTextSettings textSettings;

    // Sections
	public List<DialogueSection> sections = new List<DialogueSection>();
	
	public DialogueFrame() {}

	// Construct from defaults in DialogueConversation
	public DialogueFrame(DialogueConversation _conv)
	{
		portraitSettings 	= new DialoguePortraitSettings(_conv.portraitSettings);
		textSettings 		= new DialogueTextSettings(_conv.textSettings);
		allowSkip 			= _conv.allowSkip;
		waitForInput		= _conv.waitForInput;
	}
	
	public bool Load(JSON _JSON)
	{
		bool success = true;
		
		success &= _JSON["id"].Get (ref id);

		// Links
		_JSON["endOnThisFrame"].Get (ref endOnThisFrame);
		_JSON["displayChoices"].Get (ref displayChoices);
		foreach (JSON link in _JSON["links"])
			links.Add (link.GetArchive<DialogueLink>());

		// NPC Settings
		DialogueNPCSettings npcSettings = DialogueManager.FetchNPCSettings(_JSON["npcSettings"].GetString());
		if (npcSettings != null)
		{
			if (npcSettings.portraitSettings != null)
				portraitSettings = new DialoguePortraitSettings(npcSettings.portraitSettings);
			if (npcSettings.textSettings != null)
				textSettings = new DialogueTextSettings(npcSettings.textSettings);
		}
		else
		{
			Debug.LogError("NPC Settings NOT FOUND for: " + _JSON["npcSettings"].GetString());
		}
		
		// Overrides
		_JSON["allowSkip"].Get (ref allowSkip);
		_JSON["waitForInput"].Get (ref waitForInput);
		_JSON["portraitSettings"].GetArchive(ref portraitSettings);
		_JSON["textSettings"].GetArchive(ref textSettings);

		// Sections
		foreach (JSON section in _JSON["sections"])
			sections.Add (section.GetArchive(new DialogueSection(this)));

		success &= sections.Count > 0;

		return success;
	}
	
	public JSON Save()
	{
		JSON save = new JSON();
		
		save["id"].data = id;
		save["endOnThisFrame"].data = endOnThisFrame;
		save["displayChoices"].data = displayChoices;

		for (int i = 0; i < links.Count; ++i)
			save["links"][i] = links[i].Save();
		
		save["allowSkip"].data = allowSkip;
		save["waitForInput"].data = waitForInput;
		save["portraitSettings"] = portraitSettings.Save ();
		save["textSettings"] = textSettings.Save ();
		
		for (int i = 0; i < sections.Count; ++i)
			save["sections"][i] = sections[i].Save();

		return save;
	}
	
	public override string ToString ()
	{
		return Save ().ToString();
	}
}
