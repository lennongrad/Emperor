Shader "Unlit/NewUnlitShader" {
    Properties {
        _MainTex ("Colors (RGB)", 2D) = "white"
        _MapTex ("Map (RGB)", 2D) = "white"
        _HeightTex ("Height (RGB)", 2D) = "white"
		_V ("V", Vector) = (0.0, 0.0, 0.0)
    }
    SubShader {    
        Cull front 
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform sampler2D _MainTex;
            uniform sampler2D _MapTex;
            uniform sampler2D _HeightTex;
			
			float4 _V;

			struct VertexInput {
				float4 vertex : POSITION;       //local vertex position
				float3 normal : NORMAL;         //normal direction
				float4 tangent : TANGENT;       //tangent direction    
				float2 texcoord : TEXCOORD0;   //uv coordinates
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;              //screen clip space position and depth
				float2 uv : TEXCOORD0;                //uv coordinates

				//below we create our own variables with the texcoord semantic. 
				float3 normalDir : TEXCOORD3;          //normal direction   
				float3 posWorld : TEXCOORD4;          //normal direction   
				UNITY_FOG_COORDS(9)                    //this initializes the unity fog
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o = (VertexOutput)0;           
				o.uv = v.texcoord;
				// o.normalDir = UnityObjectToWorldNormal(v.normal);
				// UNITY_TRANSFER_FOG(o,o.pos);
				// //TRANSFER_VERTEX_TO_FRAGMENT(o)
				float3 dcolor = tex2Dlod (_HeightTex, float4(o.uv,0,0));
				// float d = (dcolor.r + dcolor.g + dcolor.b);
				//v.vertex.xyz += v.normal; //* d;
				//v.vertex += _V * dcolor.g;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

            float4 frag(VertexOutput i) : SV_Target {			
				float4 color = tex2D(_MapTex, i.uv);				
				
				int r = color.x * 255.0;
				int g = color.y * 255.0;
				int b = color.z * 255.0;
				
				float4 result = float4(0.0, 0.0, 0.0, 0.0);
				if(g == 255){
					result = tex2D(_MainTex, float2(0.999, color.x*8.0));
					// idk why this is necessary lol
				} else {
					result = tex2D(_MainTex, float2(color.y, color.x * 8.0));
				}
				
                return result;
            }
            ENDCG
        }
    }
}