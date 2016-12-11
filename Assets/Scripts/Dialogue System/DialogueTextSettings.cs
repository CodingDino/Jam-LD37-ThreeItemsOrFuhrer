// ************************************************************************ 
// File Name:   DialogueTextSettings.cs 
// Purpose:    	Settings for text printout during a section of dialogue.
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
// Class: DialogueTextSettings
// ************************************************************************
[System.Serializable]
public class DialogueTextSettings : Archive {

    public float textSpeed = 1.0f;
    public float textPitch = 1.0f;
    public float textPitchVariation = 0.0f;
    public string textAudio = "Dialogue-Default";
	
	public DialogueTextSettings() {}

	public DialogueTextSettings(DialogueTextSettings _orig)
	{
		textSpeed = _orig.textSpeed;
		textPitch = _orig.textPitch;
		textPitchVariation = _orig.textPitchVariation;
		textAudio = _orig.textAudio;
	}

	public bool Load(JSON _JSON)
	{
		bool success = true;
		
		_JSON["textSpeed"].Get (ref textSpeed);
		_JSON["textPitch"].Get (ref textPitch);
		_JSON["textPitchVariation"].Get (ref textPitchVariation);
		_JSON["textAudio"].Get (ref textAudio);

		return success;
	}
	
	public JSON Save()
	{
		JSON save = new JSON();
		
		save["textSpeed"].data = textSpeed;
		save["textPitch"].data = textPitch;
		save["textPitchVariation"].data = textPitchVariation;
		save["textAudio"].data = textAudio;
		
		return save;
	}
	
	public override string ToString ()
	{
		return Save ().ToString();
	}
}