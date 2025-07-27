Shader "Unlit/NewUnlitShader" {
    Properties {
        _MainTex ("Main (RGB)", 2D) = "white"
        _MapTex ("Map (RGB)", 2D) = "white"
    }
    SubShader {    
        Cull front 
        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform sampler2D _MainTex;
            uniform sampler2D _MapTex;

            float4 frag(v2f_img i) : SV_Target {			
				float4 color = tex2D(_MapTex, i.uv);				
				
				int r = color.x * 255.0;
				int g = color.y * 255.0;
				int b = color.z * 255.0;
				
				float4 result = float4(0.0, 0.0, 0.0, 0.0);
				if(r < 32){
					result = tex2D(_MainTex, float2(color.y, color.x * 8.0));
				}
				
                return result;
            }
            ENDCG
        }
    }
}