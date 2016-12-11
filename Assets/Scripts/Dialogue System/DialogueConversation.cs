// ************************************************************************ 
// File Name:   DialogueConversation.cs 
// Purpose:    	Information about a conversation for the dialogue system
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BounderFramework;


// ************************************************************************ 
// Class: DialogueConversation
// ************************************************************************ 
[System.Serializable]
public class DialogueConversation : Archive {

    public string id;

    // Defaults (Can be overridden by frames and sections)
    public bool allowSkip = true;
    public bool waitForInput = true;
    public DialoguePortraitSettings portraitSettings = new DialoguePortraitSettings();
	public DialogueTextSettings textSettings = new DialogueTextSettings();

    // Frames
    public string startingFrame;
    public Dictionary<string, DialogueFrame> frames = new Dictionary<string, DialogueFrame>();
	
	public bool Load(JSON _JSON)
	{
		bool success = true;

		success &= _JSON["id"].Get (ref id);
		
		// Defaults
		_JSON["allowSkip"].Get (ref id);
		_JSON["waitForInput"].Get (ref id);
		_JSON["portraitSettings"].GetArchive(ref portraitSettings);
		_JSON["textSettings"].GetArchive(ref textSettings);

		// Frames
		_JSON["startingFrame"].Get (ref startingFrame);
		DialogueFrame lastFrame = null;
		bool lastFrameLinkNeeded = false;
		foreach (JSON frame in _JSON["frames"])
		{
			DialogueFrame newFrame = frame.GetArchive(new DialogueFrame(this));
			frames[newFrame.id] = newFrame;
			
			if (lastFrameLinkNeeded)
			{
				DialogueLink link = new DialogueLink();
				link.linkedFrame = newFrame.id;
				lastFrame.links = new List<DialogueLink>();
				lastFrame.links.Add(link);
			}
			
			lastFrameLinkNeeded = (!newFrame.endOnThisFrame && (newFrame.links == null || newFrame.links.Count == 0));
			lastFrame = newFrame;
			
			if (startingFrame == null || startingFrame == "")
				startingFrame = newFrame.id;
		}
		if (lastFrameLinkNeeded)
			lastFrame.endOnThisFrame = true;

		success &= (frames.Count > 0);

		return success;
	}

	public JSON Save()
	{
		JSON save = new JSON();

		save["id"].data = id;
		save["allowSkip"].data = allowSkip;
		save["waitForInput"].data = waitForInput;
		save["portraitSettings"].data = portraitSettings;
		save["textSettings"].data = textSettings;

		save["startingFrame"].data = startingFrame;
		int j = 0;
		foreach (KeyValuePair<string, DialogueFrame> frame in frames)
		{
			save["startingFrame"][j] = frame.Value.Save();
			++j;
		}

		return save;
	}

	public override string ToString ()
	{
		return Save ().ToString();
	}

}
