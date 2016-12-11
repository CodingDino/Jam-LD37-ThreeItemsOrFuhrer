// ************************************************************************ 
// File Name:   DialogueNPCSettings.cs 
// Purpose:    	Information about dialogue settings for a specific NPC
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using System.Collections.Generic;
using UnityEngine;
using BounderFramework;


// ************************************************************************ 
// Class: DialogueNPCSettings
// ************************************************************************ 
[System.Serializable]
public class DialogueNPCSettings : Archive {

    public string id;
    public DialoguePortraitSettings portraitSettings;
    public DialogueTextSettings textSettings;

    // Runtime Objects
    public Sprite largePortrait; // TODO: Animation?

	public bool Load(JSON _JSON)
	{
		bool success = true;
		
		success &= _JSON["id"].Get (ref id);
		_JSON["portraitSettings"].GetArchive (ref portraitSettings);
		_JSON["textSettings"].GetArchive (ref textSettings);
		
		return success;
	}
	
	public JSON Save()
	{
		JSON save = new JSON();
		
		save["id"].data = id;
		save["portraitSettings"] = portraitSettings.Save();
		save["textSettings"] = textSettings.Save();
		
		return save;
	}
	
	public override string ToString ()
	{
		return Save ().ToString();
	}

}
