using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions; 

public class LoadPermanentData
{
	public static (Dictionary<int, Terrain> te, Dictionary<int, Culture> cu, Dictionary<int, Religion> re, Dictionary<int, Country> co, Dictionary<int, Province> pr) Load() {
		var loadedTerrains = loadTerrains();
		var loadedCultures = loadCultures();
		var loadedReligions = loadReligions();
		var loadedCountries = loadCountries();
		var loadedProvinces = loadProvinces(loadedTerrains, loadedCountries);
		
		InitializeData.InitData(
			loadedTerrains, loadedCultures, loadedReligions, loadedCountries, loadedProvinces
		);
		
		return (loadedTerrains, loadedCultures, loadedReligions, loadedCountries, loadedProvinces);
	}
	
	// terrains
	public static Dictionary<int, Terrain> loadTerrains() 
	{
		var loadedTerrains = new Dictionary<int, Terrain>();
		
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
	public static Dictionary<int, Culture> loadCultures() 
	{
		var loadedCultures = new Dictionary<int, Culture>();
		
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
	public static Dictionary<int, Religion> loadReligions() 
	{
		var loadedReligions = new Dictionary<int, Religion>();
		
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
	public static Dictionary<int, Country> loadCountries() 
	{
		var loadedCountries = new Dictionary<int, Country>();
		
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
	public static Dictionary<int, Province> loadProvinces(
		Dictionary<int, Terrain> loadedTerrains,
		Dictionary<int, Country> loadedCountries
	)
	{
		var loadedProvinces = new Dictionary<int, Province>();
		
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
			
			Province newProvince = new Province(
				province.id,
				province.Name,
				identifiedTerrain,
				geograpicalCenter
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
		public int CenterX  { get; set; }
		public int CenterY  { get; set; }
	}
}
