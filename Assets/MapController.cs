using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{	
	public List<Color32> colors;
	public Texture2D map;
	
	Texture2D texture;
	Texture2D secondaryTexture;

	Color32 GetProvinceColor(int id){
		Color32 baseColor = new Color32(0,0,0,255);
		
		if(GameState.Instance.Provinces.ContainsKey(id)){
			baseColor = GameState.Instance.Provinces[id].testColor;
		}
		
		if(GameState.Instance.Selected != -1){
			if(id == GameState.Instance.Selected){
				return new Color32(255,255,255,255);
			}
			//return //Color32.Lerp(baseColor, new Color32(0,0,0,255), 0.75f);
		}
		
		return baseColor;
	}
	
	Color32 GetSecondaryProvinceColor(int id){
		return GetProvinceColor(id);
	}
	
	public void SetColors() {
		
		texture = (Texture2D)GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex");
		secondaryTexture = (Texture2D)GetComponent<Renderer>().sharedMaterial.GetTexture("_SecondaryTex");
        var data = texture.GetRawTextureData<Color32>();
        var secondaryData = secondaryTexture.GetRawTextureData<Color32>();

        int index = 0;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                data[index] = GetProvinceColor(index);
				secondaryData[index] = GetSecondaryProvinceColor(index);
				index += 1;
            }
        }
		
        texture.Apply();
        secondaryTexture.Apply();
	}
	
	public Camera playerCamera;
	public float baseZoom = 400f;
	float targetZoom = 400f;
	public Vector3 drawDownPosition = new Vector3(0,0,-1);
	
	int i = 0;
	
	void Update(){
		SetColors();
		
		if(Input.GetKeyDown("d")){
			GameState.Instance.Test();
		}
		
		targetZoom = Mathf.Min(baseZoom, Mathf.Max(100f, targetZoom - Input.mouseScrollDelta.y * 15f));
		playerCamera.orthographicSize = (int)Mathf.Lerp(playerCamera.orthographicSize, targetZoom, 0.1f);
		
		float minX = 0.00109633f * playerCamera.orthographicSize * Screen.width;
		float maxX = 3844f - playerCamera.orthographicSize * Screen.width * 0.00110352f;
		float minY = playerCamera.orthographicSize/0.976479f - 2167f;
		float maxY = -playerCamera.orthographicSize;
		
		if(Input.GetMouseButtonDown(2)){
			drawDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		} else if (Input.GetMouseButton(2)){
			Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			playerCamera.transform.position = new Vector3(
				playerCamera.transform.position.x + (drawDownPosition.x - currentPosition.x),
				playerCamera.transform.position.y + (drawDownPosition.y - currentPosition.y),
				playerCamera.transform.position.z
			);
		}
		
		playerCamera.transform.position = new Vector3(
			Mathf.Min(maxX, Mathf.Max(minX, playerCamera.transform.position.x)),
			Mathf.Min(maxY, Mathf.Max(minY, playerCamera.transform.position.y)),
			playerCamera.transform.position.z
		);
		
		// i += 1;
		// if(i > 100){
			// Debug.Log(Screen.width * playerCamera.orthographicSize);
			// i = 0;
		// }
		
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
				
				var selectedProvince = GameState.Instance.Provinces[id];
				if(selectedProvince.terrain.is_ocean){
					GameState.Instance.Selected = -1;
				} else {
					GameState.Instance.Selected = id;
				}
			} else {
				GameState.Instance.Selected = -1;
			}
		}  
    }
	
}
