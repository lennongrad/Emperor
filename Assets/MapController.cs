using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{	
	public List<Color32> colors;
	public Texture2D map;
	
	List<Color32> backupColors = new List<Color32>();
	Texture2D texture;
	
	public int selected = -1;
	
	Color32 MultiplyColor(Color32 a, Color32 b) {
		return new Color32((byte)(a.r * b.r / 255), (byte)(a.g * b.g / 255), (byte)(a.b * b.b / 255), (byte)(a.a * b.a / 255));
	}

	Color32 GetProvinceColor(int id){
		Color32 baseColor = new Color32(0,0,0,255);
		if (id < colors.Count){
			baseColor = colors[id];
		} else {			
			while(id >= backupColors.Count){
				backupColors.Add(new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255),255));
			}
			baseColor = backupColors[id];
		}
		
		if(selected != -1){
			if(id == selected){
				return new Color32(255,255,255,255);
			}
			return Color32.Lerp(baseColor, new Color32(0,0,0,255), 0.75f);
		}
		
		return baseColor;
	}
	
	public void SetColors() {
        if(texture == null){
			texture = (Texture2D)GetComponent<Renderer>().sharedMaterial.mainTexture;
			
			//new Texture2D(256, 32, TextureFormat.RGBA32, false);
			//GetComponent<Renderer>().material.mainTexture = texture;
			//texture.filterMode = FilterMode.Point;
		}
		
        var data = texture.GetRawTextureData<Color32>();

        int index = 0;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                data[index] = GetProvinceColor(index);
				index += 1;
            }
        }
		
        texture.Apply();
	}
	
	public Vector3 lastPosition = new Vector3(0,0,-1);
	public Camera playerCamera;
	public float baseZoom = 400f;
	float targetZoom = -1f;
	void Update(){
		SetColors();
		
		if(targetZoom < 0){
			targetZoom = baseZoom;
		}
		targetZoom -= Input.mouseScrollDelta.y * 15f;
		playerCamera.orthographicSize = (int)Mathf.Lerp(playerCamera.orthographicSize, targetZoom, 0.1f);
		
		if(Input.GetMouseButton(2)){
			Vector3 currentPosition = Input.mousePosition;
			float modifier = playerCamera.orthographicSize / baseZoom;
			if(lastPosition.z != -1){
				playerCamera.transform.position = new Vector3(
					playerCamera.transform.position.x + (lastPosition.x - currentPosition.x) * modifier, 
					playerCamera.transform.position.y + (lastPosition.y - currentPosition.y) * modifier, 
					playerCamera.transform.position.z);
			}
			lastPosition = currentPosition;
		} else {
			lastPosition = new Vector3(0,0,-1);
		}
        if(Input.GetMouseButtonDown(0))
		{
			Vector3 p = Input.mousePosition;
			p.z = 20;
			Vector3 pos = Camera.main.ScreenToWorldPoint(p);
			pos.y *= -2;
			pos.x *= 2;
			
			if(pos.x < map.width && pos.x > 0 && pos.y < map.height && pos.y > 0)
			{
				Color pixel = (map.GetPixel((int)pos.x, map.height - (int)pos.y));
				//print($"({pos.x},{pos.y}):{pixel.g * 255},{pixel.r * 255}");
				int id = (int)(pixel.r * 255f * 256f) + (int)(pixel.g * 255f);
				selected = id;
			} else {
				selected = -1;
			}
		}  
    }
	
}
