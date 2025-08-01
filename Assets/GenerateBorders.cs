using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class GenerateBorders : MonoBehaviour
{
    [MenuItem("MyMenu/Update Map")]
    static void UpdateMap()
	{
		MapController mapController = Selection.activeTransform.gameObject.GetComponent<MapController>();
		
		mapController.SetColors();
		
		float maxX = 0;
		float maxY = 0;
		
        string path = "Assets/MapGeneration/borders_points.csv";
        StreamReader reader = new StreamReader(path); 
		
		string line;
		while ((line = reader.ReadLine()) != null)
		{
			int[] words = line.Split(',').Select(item => int.Parse(item)).ToArray();
			int id = words[0];
			
			for(int i = 1; i < words.Count(); i+=2){
				if(words[i] > maxX){
					maxX = words[i];
				}
				if(words[i+1] > maxY){
					maxY = words[i+1];
				}
			}
		}
		
		mapController.transform.position = new Vector3(maxX/2, -maxY/2, 0);
		mapController.transform.localScale = new Vector3(-maxX/10f, 0, maxY/10f);
		
		Debug.Log("Updated map!");
	}
	
	
    [MenuItem("MyMenu/Test")]
	static void Test() 
	{	
		var (loadedTerrains, loadedCultures, loadedReligions, loadedCountries, loadedProvinces) = LoadPermanentData.Load();
		
		foreach(var province in loadedProvinces.Values){
			Debug.Log(province);
		}
	}

	
    [MenuItem("MyMenu/Update Borders")]
    static void UpdateBorders()
    {
		BordersContainer bordersContainer = Selection.activeTransform.gameObject.GetComponent<BordersContainer>();
		
		var tempList = bordersContainer.transform.Cast<Transform>().ToList();
		foreach(var child in tempList)
		{
			if(child.GetComponent<BorderController>() != null) {
				GameObject.DestroyImmediate(child.gameObject);
			}
		}
		
        string path = "Assets/MapGeneration/borders_points.csv";
        StreamReader reader = new StreamReader(path); 
		
		string line;
		while ((line = reader.ReadLine()) != null)
		{
			int[] words = line.Split(',').Select(item => int.Parse(item)).ToArray();
			int id = words[0];

			GameObject border = Instantiate(bordersContainer.borderPrefab,  new Vector3(0,0,0), Quaternion.identity);
			border.name = id.ToString();
			border.transform.parent = bordersContainer.transform;
			
			LineRenderer lr = border.GetComponent<LineRenderer>();
			lr.positionCount = (words.Count() - 1)/2;
			
			for(int i = 1; i < words.Count(); i+=2){
				Vector3 pos = new Vector3(words[i], -words[i+1], 0);
				lr.SetPosition((i - 1) / 2, pos);
			}
			
			lr.Simplify(0.1f);
		}	
		
		Debug.Log("Updated borders!");
    }

    [MenuItem("MyMenu/Update Map", true)]
    static bool ValidateUpdateMap()
    {
        return Selection.activeTransform != null && Selection.activeTransform.gameObject.name == "Map";
    }

    [MenuItem("MyMenu/Update Borders", true)]
    static bool ValidateUpdateBorders()
    {
        return Selection.activeTransform != null && Selection.activeTransform.gameObject.name == "Borders Container";
    }
}