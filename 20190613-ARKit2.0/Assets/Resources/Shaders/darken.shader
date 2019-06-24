﻿
Shader "Custom/ARDarken"
{
	Properties
	{
    	_textureY ("TextureY", 2D) = "white" {}
        _textureCbCr ("TextureCbCr", 2D) = "black" {}
        _texCoordScale ("Texture Coordinate Scale", float) = 1.0
        _isPortrait ("Device Orientation", Int) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
            ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

            uniform float _texCoordScale;
            uniform int _isPortrait;
            float4x4 _TextureRotation;

            uniform float3 _Target;

			struct Vertex
			{
				float4 position : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct TexCoordInOut
			{
				float4 position : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 target : TEXCOORD1;
			};

			TexCoordInOut vert (Vertex vertex)
			{
				TexCoordInOut o;

				o.position = UnityObjectToClipPos(vertex.position); 
				if (_isPortrait == 1)
				{
					o.texcoord = float2(vertex.texcoord.x, -(vertex.texcoord.y - 0.5f) * _texCoordScale + 0.5f);
				}
				else
				{
					o.texcoord = float2((vertex.texcoord.x - 0.5f) * _texCoordScale + 0.5f, -vertex.texcoord.y);
				}
				o.texcoord = mul(_TextureRotation, float4(o.texcoord,0,1)).xy;
	            
	      o.target = float2(vertex.texcoord.x, vertex.texcoord.y);
				return o;
			}
			
            // samplers
            sampler2D _textureY;
            sampler2D _textureCbCr;

			fixed4 frag (TexCoordInOut i) : SV_Target
			{
				// sample the texture
                float2 texcoord = i.texcoord;
                float y = tex2D(_textureY, texcoord).r;
                float4 ycbcr = float4(y, tex2D(_textureCbCr, texcoord).rg, 1.0);


                 float2 tPos = float2(1,1)-  _Target.yx;

				const float4x4 ycbcrToRGBTransform = float4x4(
						float4(1.0, +0.0000, +1.4020, -0.7010),
						float4(1.0, -0.3441, -0.7141, +0.5291),
						float4(1.0, +1.7720, +0.0000, -0.8860),
						float4(0.0, +0.0000, +0.0000, +1.0000)
					);

				float4 fCol = mul(ycbcrToRGBTransform, ycbcr);
				//fCol = float4( texcoord.x , -texcoord.y ,0,1);

				float2 dist = float2( 1-texcoord.x - _Target.y , texcoord.y-(- _Target.x));//float2( _Target.y , 0 );
				//fCol = float4( dist.x * 10 , dist.y*10 ,0,1);
                return fCol / (.4+(1 * length( dist))) ;
			}
			ENDCG
		}
	}
}