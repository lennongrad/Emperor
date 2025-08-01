#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Terrain {
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

public struct Culture {
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

public struct Religion {
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

public struct Country {
	// permanent
	public int id;
	public string name;
	
	// dynamic
	public Culture? rulingCulture;
	public Religion? rulingReligion;
	
	public Country(int _id, string _name) {
		id = _id;
		name = _name;
		
		rulingCulture = null;
		rulingReligion = null;
	}
	
	public override string ToString() {
		return 
		 $@"ID: {id}
Name: {name}
Culture: {rulingCulture}
Religion: {rulingReligion}";
	}
}

public struct Province {
	// permanent
	public int id;
	public string name;
	public Terrain terrain;
	public Vector2 geographicalCenter;
	
	// dynamic
	public List<Pop>? pops;
	public int population;

	// calculated
	public Culture? dominantCulture;
	public Religion? dominantReligion;
	
	
	public Province(int _id, string _name,Terrain _terrain, Vector3 _geographicalCenter) {
		// assign permanent values
		id = _id; name = _name; terrain = _terrain; geographicalCenter = _geographicalCenter;
		
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

public struct Pop {
	// permanent
	public int id;
	public Culture baseCulture;
	public Religion baseReligion;
	
	
	public string GetName() => $"{baseCulture.name} {baseReligion.name}";
	
	public override string ToString() {
		return 
		 $@"ID: {id}
Name: {GetName()}
Culture: {baseCulture}
Religion: {baseReligion}";
	}
}