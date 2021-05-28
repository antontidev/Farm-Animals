// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TriForge/Top Down Tree"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.52
		_MainTex("Base Color", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Metallic("Metallic", 2D) = "black" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_MainTextureBrightness("Main Texture Brightness", Range( 0 , 2)) = 0.5
		_TopBrightness("Top Brightness", Range( 0 , 10)) = 3
		_WindStrength("Wind Strength", Range( 0 , 3)) = 0.5
		_WindAxisXZ("Wind Axis (X, Z)", Vector) = (0,0,1,0)
		_WindNoiseScale("Wind Noise Scale", Range( 0.01 , 1)) = 1
		_Color("Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "DisableBatching" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform float3 _WindAxisXZ;
		uniform float _WindNoiseScale;
		uniform float _WindStrength;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color;
		uniform float _MainTextureBrightness;
		uniform float _TopBrightness;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform float _Smoothness;
		uniform float _Cutoff = 0.52;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
		{
			original -= center;
			float C = cos( angle );
			float S = sin( angle );
			float t = 1 - C;
			float m00 = t * u.x * u.x + C;
			float m01 = t * u.x * u.y - S * u.z;
			float m02 = t * u.x * u.z + S * u.y;
			float m10 = t * u.x * u.y + S * u.z;
			float m11 = t * u.y * u.y + C;
			float m12 = t * u.y * u.z - S * u.x;
			float m20 = t * u.x * u.z - S * u.y;
			float m21 = t * u.y * u.z + S * u.x;
			float m22 = t * u.z * u.z + C;
			float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 worldToObjDir94 = normalize( mul( unity_WorldToObject, float4( _WindAxisXZ, 0 ) ).xyz );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 panner82 = ( ( _Time.y * 1.0 ) * float2( 0.12,0 ) + (( ase_worldPos / ( -80.0 * _WindNoiseScale ) )).xz);
			float simplePerlin3D80 = snoise( float3( panner82 ,  0.0 )*3.0 );
			simplePerlin3D80 = simplePerlin3D80*0.5 + 0.5;
			float2 panner102 = ( ( _Time.y * 0.6 ) * float2( -0.1,0.1 ) + (( ase_worldPos / ( -80.0 * _WindNoiseScale ) )).xz);
			float simplePerlin3D97 = snoise( float3( panner102 ,  0.0 )*6.0 );
			simplePerlin3D97 = simplePerlin3D97*0.5 + 0.5;
			float blendOpSrc110 = simplePerlin3D80;
			float blendOpDest110 = simplePerlin3D97;
			float lerpBlendMode110 = lerp(blendOpDest110,( blendOpSrc110 * blendOpDest110 ),0.76);
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 rotatedValue59 = RotateAroundAxis( float3(0,0,0), ase_vertex3Pos, normalize( worldToObjDir94 ), radians( ( (-0.4 + (( saturate( lerpBlendMode110 )) - 0.0) * (0.6 - -0.4) / (1.0 - 0.0)) * 30.0 ) ) );
			v.vertex.xyz += ( ( float3( v.texcoord1.xy ,  0.0 ) * ( rotatedValue59 - ase_vertex3Pos ) ) * _WindStrength );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 temp_output_47_0 = ( ( tex2DNode1 * _Color ) * _MainTextureBrightness );
			float3 desaturateInitialColor46 = temp_output_47_0.rgb;
			float desaturateDot46 = dot( desaturateInitialColor46, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar46 = lerp( desaturateInitialColor46, desaturateDot46.xxx, 0.25 );
			float4 lerpResult32 = lerp( temp_output_47_0 , float4( ( desaturateVar46 * _TopBrightness ) , 0.0 ) , saturate( pow( i.uv2_texcoord2.y , 2.93 ) ));
			o.Albedo = ( lerpResult32 * 1.0 ).rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			o.Smoothness = ( tex2D( _Metallic, uv_Metallic ).a * _Smoothness );
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
1920;66;1920;952;1701.962;137.9502;1.627717;True;False
Node;AmplifyShaderEditor.CommentaryNode;148;-4335.362,1090.705;Inherit;False;3086.106;1268.823;Wind;32;62;59;71;61;78;94;72;106;73;110;97;65;80;102;82;101;105;87;57;100;104;85;58;103;55;84;150;98;153;151;99;86;Wind;0.2795924,0.6454769,0.9716981,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-4189.767,1357.407;Inherit;False;Constant;_Float3;Float 3;7;0;Create;True;0;0;False;0;False;-80;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-4130.376,1790.034;Inherit;False;Constant;_Float5;Float 5;8;0;Create;True;0;0;False;0;False;-80;-80;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;151;-4281.971,2045.849;Inherit;False;Property;_WindNoiseScale;Wind Noise Scale;9;0;Create;True;0;0;False;0;False;1;1;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;-4111.052,1865.175;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;98;-4155.046,1643.427;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-4170.378,1435.465;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;84;-4210.433,1203.434;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;85;-4001.978,1280.376;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;103;-3925.099,1817.693;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;55;-3984.626,1392.238;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-3960.749,1478.014;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-3922.606,1903.47;Inherit;False;Constant;_Float6;Float 6;7;0;Create;True;0;0;False;0;False;0.6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;100;-3946.413,1691.728;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;101;-3802.525,1716.295;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-3734.51,1818.506;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;87;-3858.342,1301.993;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-3783.346,1395.724;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;82;-3574.464,1348.15;Inherit;False;3;0;FLOAT2;1,1;False;2;FLOAT2;0.12,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;102;-3568.398,1757.568;Inherit;False;3;0;FLOAT2;1,1;False;2;FLOAT2;-0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;52;-2621.715,-1756.242;Inherit;False;1982.951;1064.909;Top-down gradient;17;43;42;31;20;21;7;51;50;49;32;37;44;46;47;48;1;154;Main Color Gradient;0.5744526,1,0,1;0;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;80;-3373.878,1342.544;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;1,1,0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;97;-3372.694,1754.237;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;1,1,0;False;1;FLOAT;6;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;110;-3034.046,1559.235;Inherit;True;Multiply;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.76;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;154;-2586.244,-1474.478;Inherit;False;Property;_Color;Color;10;0;Create;True;0;0;False;0;False;0,0,0,0;0.8671677,0.9433962,0.6541474,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-2585.016,-1671.917;Inherit;True;Property;_MainTex;Base Color;1;0;Create;False;0;0;False;0;False;-1;None;276b1d90ab804824f908cce084bfcc6e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;106;-2748.281,1565.569;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.4;False;4;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-2641.084,1828.644;Inherit;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;False;0;False;30;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-2468.529,-1308.266;Inherit;False;Property;_MainTextureBrightness;Main Texture Brightness;5;0;Create;True;0;0;False;0;False;0.5;0.8;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;-2237.441,-1604.078;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-2127.913,-1434.423;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;65;-2554.455,1349.107;Inherit;False;Property;_WindAxisXZ;Wind Axis (X, Z);8;0;Create;True;0;0;False;0;False;0,0,1;0,0,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-2443.529,1653.851;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;61;-2151.188,1850.599;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;78;-2140.25,1681.637;Inherit;False;Constant;_Vector1;Vector 1;7;0;Create;True;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformDirectionNode;94;-2198.782,1453.186;Inherit;False;World;Object;True;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RadiansOpNode;71;-2114.752,1603.683;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-2407.264,-919.3314;Inherit;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;False;0;False;2.93;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;50;-1914.591,-1544.798;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-2472.83,-1192.645;Inherit;True;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;20;-2163.723,-1116.856;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;46;-1868.342,-1435.273;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.25;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;149;-974.6036,979.8713;Inherit;False;484.7685;409.0511;;2;53;143;Mask Wind by UV2;1,0.5394964,0,1;0;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;59;-1933.005,1452.413;Inherit;False;True;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1962.872,-1308.492;Inherit;False;Property;_TopBrightness;Top Brightness;6;0;Create;True;0;0;False;0;False;3;4;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;51;-1628.811,-1565.587;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;53;-928.7106,1051.04;Inherit;True;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;62;-1512.059,1685.395;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;49;-1351.962,-1545.786;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;31;-1860.198,-1118.221;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1619.098,-1385.986;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;3;-866.3923,235.0659;Inherit;True;Property;_Metallic;Metallic;3;0;Create;True;0;0;False;0;False;-1;None;2ab8f9685d8dba54c98ae0fe770c253d;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-849.3923,461.3057;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;32;-1204.751,-1416.38;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.7783207,1,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-642.9866,1049.106;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;146;-443.1703,1153.876;Inherit;False;Property;_WindStrength;Wind Strength;7;0;Create;True;0;0;False;0;False;0.5;0.7;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-1171.507,-1242.536;Inherit;False;Constant;_Float4;Float 4;5;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;-157.0041,1044.857;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-505.5109,393.8014;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-869.6678,-18.8577;Inherit;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;False;-1;None;44d4be3a88a229246b2351763b99080f;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-921.5496,-1328.715;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;TriForge/Top Down Tree;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;True;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.52;True;True;0;True;TransparentCutout;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;153;0;99;0
WireConnection;153;1;151;0
WireConnection;150;0;86;0
WireConnection;150;1;151;0
WireConnection;85;0;84;0
WireConnection;85;1;150;0
WireConnection;100;0;98;0
WireConnection;100;1;153;0
WireConnection;101;0;100;0
WireConnection;105;0;103;0
WireConnection;105;1;104;0
WireConnection;87;0;85;0
WireConnection;57;0;55;0
WireConnection;57;1;58;0
WireConnection;82;0;87;0
WireConnection;82;1;57;0
WireConnection;102;0;101;0
WireConnection;102;1;105;0
WireConnection;80;0;82;0
WireConnection;97;0;102;0
WireConnection;110;0;80;0
WireConnection;110;1;97;0
WireConnection;106;0;110;0
WireConnection;155;0;1;0
WireConnection;155;1;154;0
WireConnection;47;0;155;0
WireConnection;47;1;48;0
WireConnection;72;0;106;0
WireConnection;72;1;73;0
WireConnection;94;0;65;0
WireConnection;71;0;72;0
WireConnection;50;0;47;0
WireConnection;20;0;7;2
WireConnection;20;1;21;0
WireConnection;46;0;47;0
WireConnection;59;0;94;0
WireConnection;59;1;71;0
WireConnection;59;2;78;0
WireConnection;59;3;61;0
WireConnection;51;0;50;0
WireConnection;62;0;59;0
WireConnection;62;1;61;0
WireConnection;49;0;51;0
WireConnection;31;0;20;0
WireConnection;37;0;46;0
WireConnection;37;1;44;0
WireConnection;32;0;49;0
WireConnection;32;1;37;0
WireConnection;32;2;31;0
WireConnection;143;0;53;0
WireConnection;143;1;62;0
WireConnection;145;0;143;0
WireConnection;145;1;146;0
WireConnection;4;0;3;4
WireConnection;4;1;5;0
WireConnection;42;0;32;0
WireConnection;42;1;43;0
WireConnection;0;0;42;0
WireConnection;0;1;2;0
WireConnection;0;4;4;0
WireConnection;0;10;1;4
WireConnection;0;11;145;0
ASEEND*/
//CHKSM=37B16683BC873B54D64D9D7C23A3D83CF6F337F6