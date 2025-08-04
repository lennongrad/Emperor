using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderController : MonoBehaviour
{
    public LineRenderer lr;
	
	public float lastWidth = 0f;
	
	public void SetLineWidth(float width)
	{
		if(width != lastWidth){
			lr.widthMultiplier = width;
		}
		lr.enabled = width != 0f;
	}
}
