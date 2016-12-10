using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BounderFramework;

public class ProductDefinition : Archive, Data
{
	private string m_id;
	public string id { get { return m_id; } }

	public bool Load(JSON _JSON)
	{
		bool success = true;

		// Required
		success = success && _JSON["id"].Get(ref m_id);

		return success;
	}

	public JSON Save()
	{
		JSON save = new JSON();

		save["id"].data = m_id;

		return save;
	}
}
