using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductObject : MonoBehaviour 
{
	private string m_productID;

	public static ProductObject Create(string _productID)
	{
		if (!ProductDatabase.IsInitialised())
			ProductDatabase.Initialise();
		ProductDefinition productDef = ProductDatabase.GetData(_productID);
		GameObject newObject = Instantiate(Resources.Load("Products/Models/"+productDef.modelName)) as GameObject;
		newObject.AddComponent<ProductObject>();
		ProductObject newProduct = newObject.GetComponent<ProductObject>();
		newProduct.m_productID = _productID;
		return newProduct;
	}
}
