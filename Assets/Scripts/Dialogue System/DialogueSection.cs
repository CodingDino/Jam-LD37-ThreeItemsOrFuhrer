// ************************************************************************ 
// File Name:   DialogueSection.cs 
// Purpose:    	Information about a section of dialogue
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using BounderFramework;


// ************************************************************************ 
// Class: DialogueSection
// ************************************************************************ 
[System.Serializable]
public class DialogueSection : Archive {

    // Overrides (will be set to parent values if no override present) 
    public DialoguePortraitSettings portraitSettings;
    public DialogueTextSettings textSettings;

    // Animations and Effects
    public string triggerAnimation;
    public string triggerEffect;
    public bool forceIdle = false;

    // Text
	public StringBuilder text;
	public string textStatic;
	
	public DialogueSection() {}

	// Construct from defaults in DialogueConversation
	public DialogueSection(DialogueFrame _frame)
	{
		portraitSettings 	= new DialoguePortraitSettings(_frame.portraitSettings);
		textSettings 		= new DialogueTextSettings(_frame.textSettings);
	}
	
	public bool Load(JSON _JSON)
	{
		bool success = true;

		_JSON["portraitSettings"].GetArchive (ref portraitSettings);
		_JSON["textSettings"].GetArchive (ref textSettings);
		_JSON["triggerAnimation"].Get (ref triggerAnimation);
		_JSON["triggerEffect"].Get (ref triggerEffect);
		_JSON["forceIdle"].Get (ref forceIdle);
		_JSON["text"].Get (ref textStatic);
		text = new StringBuilder(textStatic);
		
		return success;
	}
	
	public JSON Save()
	{
		JSON save = new JSON();
		
		save["portraitSettings"] = portraitSettings.Save ();
		save["textSettings"] = textSettings.Save ();
		save["triggerAnimation"].data = triggerAnimation;
		save["triggerEffect"].data = triggerEffect;
		save["forceIdle"].data = forceIdle;
		save["text"].data = textStatic;
		
		return save;
	}
	
	public override string ToString ()
	{
		return Save ().ToString();
	}
}
