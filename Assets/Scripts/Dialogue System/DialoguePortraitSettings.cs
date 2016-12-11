// ************************************************************************ 
// File Name:   DialoguePortraitSettings.cs 
// Purpose:    	Settings for the portrait during a section of dialogue.
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using System.Collections.Generic;
using BounderFramework;


// ************************************************************************ 
// Class: DialoguePortraitSettings
// ************************************************************************
[System.Serializable]
public class DialoguePortraitSettings : Archive {

    public bool active = true;
    public bool large = false;
    
    public string image;
    public string displayName;
	public enum PortraitPosition { left, right }
	public enum PortraitEmotion { 
		NEUTRAL,	// = 0
		ANGRY,		// = 1
		SAD,		// = 2
		HAPPY 		// = 3
	}
    public PortraitPosition position = PortraitPosition.left;
    
	public PortraitEmotion emotion = PortraitEmotion.NEUTRAL;
	
	public DialoguePortraitSettings() {}

	public DialoguePortraitSettings(DialoguePortraitSettings _orig)
	{
		active = _orig.active;
		large = _orig.large;
		image = _orig.image;
		displayName = _orig.displayName;
		position = _orig.position;
		emotion = _orig.emotion;
	}

	public bool Load(JSON _JSON)
	{
		bool success = true;
		
		_JSON["active"].Get (ref active);
		_JSON["image"].Get (ref image);
		_JSON["displayName"].Get (ref displayName);
		_JSON["position"].GetEnum (ref position);
		_JSON["large"].Get (ref large);
		_JSON["emotion"].GetEnum (ref emotion);

		return success;
	}
	
	public JSON Save()
	{
		JSON save = new JSON();
		
		save["active"].data = active;
		save["image"].data = image;
		save["displayName"].data = displayName;
		save["position"].data = position.ToString();
		save["large"].data = large;
		save["emotion"].data = emotion.ToString();

		return save;
	}
	
	public override string ToString ()
	{
		return Save ().ToString();
	}
}