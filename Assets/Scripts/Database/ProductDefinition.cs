using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BounderFramework;

public class ProductDefinition : Archive, Data
{
	private string m_id;
	private string m_displayName;
	private string m_modelName;

	public string id { get { return m_id; } }
	public string displayName { get { return m_displayName; } }
	public string modelName { get { return m_modelName; } }

	public bool Load(JSON _JSON)
	{
		bool success = true;

		// Required
		success = success && _JSON["id"].Get(ref m_id);
		success = success && _JSON["displayName"].Get(ref m_displayName);
		success = success && _JSON["modelName"].Get(ref m_modelName);

		return success;
	}

	public JSON Save()
	{
		JSON save = new JSON();

		save["id"].data = m_id;
		save["displayName"].data = m_displayName;
		save["modelName"].data = m_modelName;

		return save;
	}
}
