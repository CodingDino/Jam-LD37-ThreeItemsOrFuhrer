using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductObject : MonoBehaviour 
{
	private string m_productID;

	[SerializeField]
	private string m_modelPath;
	[SerializeField]
	private MeshFilter m_mesh;

	public void SetupForProduct(string _productID)
	{
		m_productID = _productID;
		ProductDefinition productDef = ProductDatabase.GetData(_productID);
		m_mesh.mesh = Instantiate(Resources.Load(m_modelPath+productDef.modelName)) as Mesh;
	}
}
