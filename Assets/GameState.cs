using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class GameState : MonoBehaviour
{
	private static GameState _Instance;
	public static GameState Instance
	{
		get
		{
			if (!_Instance)
			{
				_Instance = GameObject.FindObjectOfType<GameState>();
			}
			return _Instance;
		}
	}
	 
	public SerializedDictionary<int, Terrain> Terrains = new SerializedDictionary<int, Terrain>();
	public SerializedDictionary<int, Culture> Cultures = new SerializedDictionary<int, Culture>();
	public SerializedDictionary<int, Religion> Religions = new SerializedDictionary<int, Religion>(); 
	public SerializedDictionary<int, Country> Countries = new SerializedDictionary<int, Country>();
	public SerializedDictionary<int, Province> Provinces = new SerializedDictionary<int, Province>();
	
	public int selected = -1;
	
    public void Load()
    {
		var (loadedTerrains, loadedCultures, loadedReligions, loadedCountries, loadedProvinces) = LoadPermanentData.Load();
		
		// InitializeData.InitData(
			// loadedTerrains, loadedCultures, loadedReligions, loadedCountries, loadedProvinces
		// );
		
		Terrains = loadedTerrains;
		Cultures = loadedCultures;
		Religions = loadedReligions;
		Countries = loadedCountries;
		Provinces = loadedProvinces;
	}
	
	public void Test(){
		Debug.Log(Provinces[0]);
    }
}
