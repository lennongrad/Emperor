#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Terrain {
	// permanent
	public int id;
	public string name;
	
	public Terrain(int _id, string _name) {
		id = _id;
		name = _name;
	}
	
	public override string ToString() {
		return 
		 $@"ID: {id}
Name: {name}";
	}
}

[Serializable]
public class Culture {
	// permanent
	public int id;
	public string name;
	
	public Culture(int _id, string _name) {
		id = _id;
		name = _name;
	}
	
	public override string ToString() {
		return 
		 $@"ID: {id}
Name: {name}";
	}
}

[Serializable]
public class Religion {
	// permanent
	public int id;
	public string name;
	
	public Religion(int _id, string _name) {
		id = _id;
		name = _name;
	}
	
	public override string ToString() {
		return 
		 $@"ID: {id}
Name: {name}";
	}
}

[Serializable]
public class Country {
	// permanent
	public int id;
	public string name;
	
	// dynamic
	public Culture? rulingCulture;
	public Religion? rulingReligion;
	
	public Country(int _id, string _name) {
		id = _id;
		name = _name;
	}
	
	public override string ToString() {
		return 
		 $@"ID: {id}
Name: {name}
Culture: {rulingCulture}
Religion: {rulingReligion}";
	}
}

[Serializable]
public class Province {
	// permanent
	public int id;
	public string name;
	public Terrain terrain;
	public Vector2 geographicalCenter;
	public Color32 testColor;
	
	// dynamic
	public List<Pop>? pops;
	public int population;

	// calculated
	public Culture? dominantCulture;
	public Religion? dominantReligion;
	
	
	public Province(int _id, string _name,Terrain _terrain, Vector3 _geographicalCenter, Color32 _testColor) {
		// assign permanent values
		id = _id; name = _name; terrain = _terrain; geographicalCenter = _geographicalCenter; testColor = _testColor;
		
		// assign dynamic defaults
		pops = null;
		population = 0;
		
		// assign calculated defaults
		dominantCulture = null;
		dominantReligion = null;
	}
	
	public override string ToString() {
		return 
		 $@"ID: {id}
Name: {name}
Terrain: {terrain}
Geographical Center: {geographicalCenter}
Pops: {pops}
Population: {population}
Dominant Culture: {dominantCulture}
Dominant Religion: {dominantReligion}";
	}
}

public class Pop {
	// permanent
	public int id;
	public Culture? baseCulture;
	public Religion? baseReligion;
	
	public string GetName() => $"{baseCulture?.name} {baseReligion?.name}";
	
	public override string ToString() {
		return 
		 $@"ID: {id}
Name: {GetName()}
Culture: {baseCulture}
Religion: {baseReligion}";
	}
}