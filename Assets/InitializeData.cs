using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions; 

public class InitializeData
{
	public static void InitData(
		Dictionary<int, Terrain> loadedTerrains,
		Dictionary<int, Culture> loadedCultures,
		Dictionary<int, Religion> loadedReligions,
		Dictionary<int, Country> loadedCountries,
		Dictionary<int, Province> loadedProvinces
	) 
	{
		InitializeProvinces(loadedTerrains, loadedCultures, loadedReligions, loadedCountries, loadedProvinces);
	}
	
	
	// countries
	
	
	// provinces
	public static void InitializeProvinces(
		Dictionary<int, Terrain> loadedTerrains,
		Dictionary<int, Culture> loadedCultures,
		Dictionary<int, Religion> loadedReligions,
		Dictionary<int, Country> loadedCountries,
		Dictionary<int, Province> loadedProvinces
	){
        string path = "Assets/Data/provinces_init.yaml";
        StreamReader reader = new StreamReader(path); 

		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		var provinceList = deserializer.Deserialize<ProvinceList>(reader);
		
		foreach (var initProvince in provinceList.Provinces)
		{
			if(loadedProvinces.ContainsKey(initProvince.id)){
				loadedProvinces[initProvince.id].population = initProvince.Population;
			}
		}
	}
		
	public class ProvinceList 
	{
		public List<LoadedProvince> Provinces { get; set; } 
	}
	
	public class LoadedProvince 
	{
		public int id  { get; set; }
		public int Population { get; set; }
	}
}
