using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions; 
using AYellowpaper.SerializedCollections;

public class GenerateBorders : MonoBehaviour
{
    [MenuItem("MyMenu/Update Map")]
    static void UpdateMap()
	{
		MapController mapController = Selection.activeTransform.gameObject.GetComponent<MapController>();
		
		mapController.SetColors();
		
		float maxX = 0;
		float maxY = 0;
		
        string path = "Assets/Data/borders_perm.yaml";
        StreamReader reader = new StreamReader(path); 

		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		var bordersList = deserializer.Deserialize<BordersList>(reader);
		
		foreach (var borderDetails in bordersList.Borders)
		{			
			foreach (var coord in borderDetails.Points) {
				var splitCoord = coord.Split(",");
				maxX = Mathf.Max(maxX, int.Parse(splitCoord[0]));
				maxY = Mathf.Max(maxY, int.Parse(splitCoord[1]));
			}
		}
		
		mapController.transform.position = new Vector3(maxX/2, -maxY/2, 0);
		mapController.transform.localScale = new Vector3(-maxX/10f, 0, maxY/10f);
		
		Debug.Log("Updated map!");
	}
	
	
    [MenuItem("MyMenu/Load Game State")]
	static void LoadGameState() 
	{	
		GameState.Instance.Load();
		Debug.Log("Finished loading game state!");
	}

	
    [MenuItem("MyMenu/Update Borders")]
    static void UpdateBorders()
    {
		BordersContainer bordersContainer = Selection.activeTransform.gameObject.GetComponent<BordersContainer>();
		
		var tempList = bordersContainer.transform.Cast<Transform>().ToList();
		foreach(var child in tempList)
		{
			if(true){//child.GetComponent<BorderController>() != null) {
				GameObject.DestroyImmediate(child.gameObject);
			}
		}
		
        string path = "Assets/Data/borders_perm.yaml";
        StreamReader reader = new StreamReader(path); 

		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		var bordersList = deserializer.Deserialize<BordersList>(reader);
		var generatedBorders = new SerializedDictionary<int, BorderController>();
		
		
		foreach (var borderDetails in bordersList.Borders)
		{			
			GameObject border = Instantiate(bordersContainer.borderPrefab,  new Vector3(0,0,0), Quaternion.identity);
			
			border.name = $"Border #{borderDetails.Index}";
			border.transform.parent = bordersContainer.transform;
			
			var bc = border.GetComponent<BorderController>();
			
			foreach(var lr in bc.Lines){				
				lr.positionCount = borderDetails.Points.Count();
				
				int i = 0;
				foreach (var coord in borderDetails.Points) {
					var splitCoord = coord.Split(",");
					Vector3 pos = new Vector3(int.Parse(splitCoord[0]), -int.Parse(splitCoord[1]), 0);
					lr.SetPosition(i, pos);
					i++;
				}
				
				lr.Simplify(0.1f);
			}
				
			generatedBorders[borderDetails.Index] = bc;
		}
		
		bordersContainer.borders = generatedBorders;

		Debug.Log("Updated borders!");
	}
		
	public class BordersList 
	{
		public List<LoadedBorder> Borders{ get; set; } 
	}
	
	public class LoadedBorder 
	{
		public int Index  { get; set; }
		public List<string> Points  { get; set; }
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