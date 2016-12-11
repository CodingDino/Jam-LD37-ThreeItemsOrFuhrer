// ************************************************************************ 
// File Name:   DialogueLink.cs 
// Purpose:    	Information about how two frames link together
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
// Class: DialogueLink
// ************************************************************************ 
[System.Serializable]
public class DialogueLink : Archive {

    public string linkedConversation;
    public string linkedFrame;
    public string text;
	public bool saveChoice;
	
	public bool Load(JSON _JSON)
	{
		bool success = true;
		
		_JSON["linkedConversation"].Get (ref linkedConversation);
		_JSON["linkedFrame"].Get (ref linkedFrame);
		_JSON["text"].Get (ref text);
		_JSON["saveChoice"].Get (ref saveChoice);
		
		return success;
	}
	
	public JSON Save()
	{
		JSON save = new JSON();
		
		save["linkedConversation"].data = linkedConversation;
		save["linkedFrame"].data = linkedFrame;
		save["text"].data = text;
		save["saveChoice"].data = saveChoice;
		
		return save;
	}
	
	public override string ToString ()
	{
		return Save ().ToString();
	}
}
