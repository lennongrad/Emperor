using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions; 
using AYellowpaper.SerializedCollections;

public class LoadPermanentData
{
	public static (SerializedDictionary<int, Terrain> te, SerializedDictionary<int, Culture> cu, SerializedDictionary<int, Religion> re, SerializedDictionary<int, Country> co, SerializedDictionary<int, Province> pr) Load() {
		var loadedTerrains = loadTerrains();
		var loadedCultures = loadCultures();
		var loadedReligions = loadReligions();
		var loadedCountries = loadCountries();
		var loadedProvinces = loadProvinces(loadedTerrains, loadedCountries);
		
		return (loadedTerrains, loadedCultures, loadedReligions, loadedCountries, loadedProvinces);
	}
	
	// terrains
	public static SerializedDictionary<int, Terrain> loadTerrains() 
	{
		var loadedTerrains = new SerializedDictionary<int, Terrain>();
		
        string path = "Assets/Data/terrains_perm.yaml";
        StreamReader reader = new StreamReader(path); 

		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		var terrainList = deserializer.Deserialize<TerrainList>(reader);
		
		foreach (var terrain in terrainList.Terrains)
		{
			Terrain newTerrain = new Terrain(
				terrain.id,
				terrain.Name
			);
			
			loadedTerrains.Add(terrain.id, newTerrain);
		}
		
		return loadedTerrains;
	}
	
	public class TerrainList
	{
		public List<LoadedTerrain> Terrains {get; set;}
	}
	
	public class LoadedTerrain
	{
		public int id { get; set; }
		public string Name { get; set; }
	}
	
	// cultures
	public static SerializedDictionary<int, Culture> loadCultures() 
	{
		var loadedCultures = new SerializedDictionary<int, Culture>();
		
        string path = "Assets/Data/cultures_perm.yaml";
        StreamReader reader = new StreamReader(path); 

		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		var cultureList = deserializer.Deserialize<CultureList>(reader);
		
		foreach (var culture in cultureList.Cultures)
		{
			Culture newCulture = new Culture(
				culture.id,
				culture.Name
			);
			
			loadedCultures.Add(culture.id, newCulture);
		}
		
		return loadedCultures;
	}
	
	public class CultureList
	{
		public List<LoadedCulture> Cultures {get; set;}
	}
	
	public class LoadedCulture
	{
		public int id { get; set; }
		public string Name { get; set; }
	}
	
	// religions
	public static SerializedDictionary<int, Religion> loadReligions() 
	{
		var loadedReligions = new SerializedDictionary<int, Religion>();
		
        string path = "Assets/Data/religions_perm.yaml";
        StreamReader reader = new StreamReader(path); 

		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		var religionList = deserializer.Deserialize<ReligionList>(reader);
		
		foreach (var religion in religionList.Religions)
		{
			Religion newReligion = new Religion(
				religion.id,
				religion.Name
			);
			
			loadedReligions.Add(religion.id, newReligion);
		}
		
		return loadedReligions;
	}
	
	public class ReligionList
	{
		public List<LoadedReligion> Religions {get; set;}
	}
	
	public class LoadedReligion
	{
		public int id { get; set; }
		public string Name { get; set; }
	}
	
	// countries
	public static SerializedDictionary<int, Country> loadCountries() 
	{
		var loadedCountries = new SerializedDictionary<int, Country>();
		
        string path = "Assets/Data/countries_perm.yaml";
        StreamReader reader = new StreamReader(path); 

		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		var countryList = deserializer.Deserialize<CountryList>(reader);
		
		foreach (var country in countryList.Countries)
		{
			Country newCountry = new Country(
				country.id,
				country.Name
			);
			
			loadedCountries.Add(country.id, newCountry);
		}
		
		return loadedCountries;
	}
	
	public class CountryList
	{
		public List<LoadedCountry> Countries {get; set;}
	}
	
	public class LoadedCountry
	{
		public int id { get; set; }
		public string Name { get; set; }
	}
	
	// provinces
	public static SerializedDictionary<int, Province> loadProvinces(
		SerializedDictionary<int, Terrain> loadedTerrains,
		SerializedDictionary<int, Country> loadedCountries
	)
	{
		var loadedProvinces = new SerializedDictionary<int, Province>();
		
        string path = "Assets/Data/provinces_perm.yaml";
        StreamReader reader = new StreamReader(path); 

		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		var provinceList = deserializer.Deserialize<ProvinceList>(reader);
		
		Terrain unknownTerrain = new Terrain(-1, "Unknown");
		
		foreach (var province in provinceList.Provinces)
		{
			var identifiedTerrain = loadedTerrains.ContainsKey(province.Terrain) ? loadedTerrains[province.Terrain] : unknownTerrain;
			var geograpicalCenter = new Vector2(province.CenterX, province.CenterY);
			
			string[] colorComponents = province.Color.Split(",");
			var testColor = new Color32((byte)int.Parse(colorComponents[0]), (byte)int.Parse(colorComponents[1]), (byte)int.Parse(colorComponents[2]), 255);
			
			var borders = new SerializedDictionary<int, int[]>();
			foreach(var borderIDs in province.Borders){
				borders[borderIDs.Province] = borderIDs.Borders;
			}
			
			Province newProvince = new Province(
				province.id,
				province.Name,
				identifiedTerrain,
				geograpicalCenter,
				testColor,
				borders
			);
			
			loadedProvinces.Add(province.id, newProvince);
		}
		
		return loadedProvinces;
	}
		
	public class ProvinceList 
	{
		public List<LoadedProvince> Provinces { get; set; } 
	}
	
	public class LoadedProvince 
	{
		public int id  { get; set; }
		public string Name  { get; set; }
		public int Terrain  { get; set; }
		public string Color { get; set; }
		public int CenterX  { get; set; }
		public int CenterY  { get; set; }
		public int Area { get; set; }
		public List<LoadedBorderIDs> Borders { get; set; }
	}
	
	public class LoadedBorderIDs
	{
		public int[] Borders { get; set; }
		public int Province { get; set; }
	}
}
