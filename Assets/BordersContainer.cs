using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class BordersContainer : MonoBehaviour
{
	public SerializedDictionary<int, BorderController> borders;
	public GameObject borderPrefab;
	
	public float[] Widths;
	
	int getBorderValue(int id, int[] selectedBorders)
	{
		if(selectedBorders.Length != 0 && Array.IndexOf(selectedBorders, id) != -1){
			return 3;
		}
		
		return 1;
	}
	
	void UpdateBorders() 
	{		
		var selectedProvince = GameState.Instance.GetSelectedProvince();
		int[] selectedBorders = {};
		
		if(selectedProvince != null){
			selectedBorders = selectedProvince.GetAllBorders();
		}
		
		foreach(var border in borders){
			int calculatedBorder = getBorderValue(border.Key, selectedBorders);
			
			border.Value.SetLineWidth(Widths[calculatedBorder]);
		}
	}
	
	void FixedUpdate() {
		UpdateBorders();
	}
}
