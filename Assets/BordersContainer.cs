using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class BordersContainer : MonoBehaviour
{
	public SerializedDictionary<int, BorderController> borders;
	public GameObject borderPrefab;
	
	int getBorderValue(int id)
	{
		return 0;
		// if(id < 20){
			// return 1;
		// }
		// if(id < 40){
			// return 0;
		// }
		// return -1;
	}
	
	void UpdateBorders() 
	{
		foreach(var border in borders){
			int calculatedBorder = getBorderValue(border.Key);
			
			for(int i = 0; i < border.Value.Lines.Length; i++){
				border.Value.Lines[i].enabled = (i == calculatedBorder);
			}
		}
	}
	
	void Start() {
		UpdateBorders();
	}
}
