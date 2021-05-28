// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TriForge/SimpleWater"
{
	Properties
	{
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		_WaterColor("Water Color", Color) = (0,0,0,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_Specularity("Specularity", Range( 0 , 1)) = 1
		_Opacity("Opacity", Float) = 0.5
		[HDR]_EmissiveColor("Emissive Color", Color) = (0,0,0,0)
		_NormalMapScale("Normal Map Scale", Range( 0 , 10)) = 1
		_DepthFadeDistance("Depth Fade Distance", Float) = 2
		_FresnelPower("Fresnel Power", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 1
		_NormalIntensity("Normal Intensity", Range( 0 , 5)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma surface surf StandardSpecular alpha:fade keepalpha 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalMap;
		uniform float _NormalIntensity;
		uniform float _NormalMapScale;
		uniform float4 _WaterColor;
		uniform float4 _EmissiveColor;
		uniform float _Opacity;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthFadeDistance;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float _Specularity;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float temp_output_33_0_g23 = _NormalIntensity;
			float temp_output_11_0_g23 = ( 1.0 * _Time.y );
			float3 ase_worldPos = i.worldPos;
			float2 panner44 = ( 1.0 * _Time.y * float2( 0.02,0.1 ) + (( ase_worldPos / ( 3.0 * _NormalMapScale ) )).xz);
			float2 temp_output_32_0_g23 = panner44;
			float2 panner5_g23 = ( temp_output_11_0_g23 * float2( 0.1,0.1 ) + temp_output_32_0_g23);
			float2 panner16_g23 = ( temp_output_11_0_g23 * float2( -0.1,-0.1 ) + ( temp_output_32_0_g23 + float2( 0.418,0.355 ) ));
			float2 panner19_g23 = ( temp_output_11_0_g23 * float2( -0.1,0.1 ) + ( temp_output_32_0_g23 + float2( 0.865,0.148 ) ));
			float2 panner23_g23 = ( temp_output_11_0_g23 * float2( 0.1,-0.1 ) + ( temp_output_32_0_g23 + float2( 0.651,0.752 ) ));
			float temp_output_33_0_g24 = _NormalIntensity;
			float temp_output_11_0_g24 = ( 0.6 * _Time.y );
			float2 panner50 = ( 1.0 * _Time.y * float2( 0.1,0.06 ) + (( ase_worldPos / ( 8.0 * _NormalMapScale ) )).xz);
			float2 temp_output_32_0_g24 = panner50;
			float2 panner5_g24 = ( temp_output_11_0_g24 * float2( 0.1,0.1 ) + temp_output_32_0_g24);
			float2 panner16_g24 = ( temp_output_11_0_g24 * float2( -0.1,-0.1 ) + ( temp_output_32_0_g24 + float2( 0.418,0.355 ) ));
			float2 panner19_g24 = ( temp_output_11_0_g24 * float2( -0.1,0.1 ) + ( temp_output_32_0_g24 + float2( 0.865,0.148 ) ));
			float2 panner23_g24 = ( temp_output_11_0_g24 * float2( 0.1,-0.1 ) + ( temp_output_32_0_g24 + float2( 0.651,0.752 ) ));
			float3 temp_output_51_0 = ( ( ( ( UnpackScaleNormal( tex2D( _NormalMap, panner5_g23 ), temp_output_33_0_g23 ) + UnpackScaleNormal( tex2D( _NormalMap, panner16_g23 ), temp_output_33_0_g23 ) ) + ( UnpackScaleNormal( tex2D( _NormalMap, panner19_g23 ), temp_output_33_0_g23 ) + UnpackScaleNormal( tex2D( _NormalMap, panner23_g23 ), temp_output_33_0_g23 ) ) ) * 1.0 ) + ( ( ( UnpackScaleNormal( tex2D( _NormalMap, panner5_g24 ), temp_output_33_0_g24 ) + UnpackScaleNormal( tex2D( _NormalMap, panner16_g24 ), temp_output_33_0_g24 ) ) + ( UnpackScaleNormal( tex2D( _NormalMap, panner19_g24 ), temp_output_33_0_g24 ) + UnpackScaleNormal( tex2D( _NormalMap, panner23_g24 ), temp_output_33_0_g24 ) ) ) * 1.0 ) );
			o.Normal = temp_output_51_0;
			o.Albedo = _WaterColor.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth60 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth60 = saturate( abs( ( screenDepth60 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFadeDistance ) ) );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV61 = dot( normalize( temp_output_51_0 ), ase_worldViewDir );
			float fresnelNode61 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV61, _FresnelPower ) );
			float temp_output_64_0 = ( ( _Opacity * distanceDepth60 ) * ( fresnelNode61 + 0.1 ) );
			o.Emission = ( _EmissiveColor * temp_output_64_0 ).rgb;
			float3 temp_cast_2 = (_Specularity).xxx;
			o.Specular = temp_cast_2;
			o.Smoothness = _Smoothness;
			o.Alpha = saturate( temp_output_64_0 );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
1920;0;1920;1018;3543.176;1124.786;2.514509;True;False
Node;AmplifyShaderEditor.RangedFloatNode;72;-2703.687,132.5824;Inherit;False;Property;_NormalMapScale;Normal Map Scale;6;0;Create;True;0;0;False;0;False;1;1.5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-2504.637,610.1011;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;False;8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-2505.582,-65.42422;Inherit;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-2338.717,4.095032;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-2499.456,504.1626;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;47;-2523.796,311.4239;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;40;-2502.575,-347.1512;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;48;-2251.758,453.7401;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;41;-2230.537,-204.835;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;49;-2057.806,476.4099;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;43;-2036.584,-182.1653;Inherit;False;True;False;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-1895.326,845.965;Inherit;False;Constant;_Float4;Float 4;4;0;Create;True;0;0;False;0;False;0.6;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;50;-1795.084,419.8389;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.06;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;44;-1773.862,-238.7363;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.02,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1816.741,156.3245;Inherit;False;Constant;_Float;Float;6;0;Create;True;0;0;False;0;False;1;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1809.747,259.3245;Inherit;False;Constant;_WaterSpeed;Water Speed;4;0;Create;True;0;0;False;0;False;1;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;6;-3090.831,57.21352;Inherit;True;Property;_NormalMap;Normal Map;0;1;[Normal];Create;True;0;0;False;0;False;104c20515b2912c40b3086db07672866;104c20515b2912c40b3086db07672866;True;bump;LockedToTexture2D;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-1902.743,334.5134;Inherit;False;Property;_NormalIntensity;Normal Intensity;10;0;Create;True;0;0;False;0;False;0;0.6;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-1830.944,701.0502;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;102;-1496.405,39.00959;Inherit;True;4WayChaos;-1;;23;39eb4b53527d6704289fdaf682a3d49e;0;5;32;FLOAT2;0,0;False;31;SAMPLER2D;0,0,0,0;False;29;FLOAT;0.2;False;33;FLOAT;1;False;10;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;101;-1517.627,697.5848;Inherit;True;4WayChaos;-1;;24;39eb4b53527d6704289fdaf682a3d49e;0;5;32;FLOAT2;0,0;False;31;SAMPLER2D;0,0,0,0;False;29;FLOAT;0.2;False;33;FLOAT;1;False;10;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-1195.133,928.7459;Inherit;False;Property;_FresnelPower;Fresnel Power;8;0;Create;True;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-1200.794,1031.778;Inherit;False;Property;_FresnelScale;Fresnel Scale;9;0;Create;True;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-1033.225,561.906;Inherit;False;Property;_DepthFadeDistance;Depth Fade Distance;7;0;Create;True;0;0;False;0;False;2;4.47;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-1027.205,403.7337;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;61;-1018.69,692.8844;Inherit;False;Standard;WorldNormal;ViewDir;True;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-900.6069,906.092;Inherit;False;Constant;_Float5;Float 5;6;0;Create;True;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;60;-779.29,545.9843;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-703.9589,422.2071;Inherit;False;Property;_Opacity;Opacity;4;0;Create;True;0;0;False;0;False;0.5;2.35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-710.8071,792.9922;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-479.9902,520.3843;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;69;-346.4672,248.6022;Inherit;False;Property;_EmissiveColor;Emissive Color;5;1;[HDR];Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-467.707,670.792;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-504.9768,-103.9883;Inherit;False;Property;_Specularity;Specularity;3;0;Create;True;0;0;False;0;False;1;0.95;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-748.1723,-362.2325;Inherit;False;Property;_WaterColor;Water Color;1;0;Create;True;0;0;False;0;False;0,0,0,0;0,0.2242087,0.3018867,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;63;-75.07253,673.2747;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-493.1433,5.728869;Inherit;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;False;0;False;1;0.95;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-80.94649,326.7948;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;282.5068,-45.14286;Float;False;True;-1;2;ASEMaterialInspector;0;0;StandardSpecular;TriForge/SimpleWater;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;True;True;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;71;0;42;0
WireConnection;71;1;72;0
WireConnection;73;0;46;0
WireConnection;73;1;72;0
WireConnection;48;0;47;0
WireConnection;48;1;73;0
WireConnection;41;0;40;0
WireConnection;41;1;71;0
WireConnection;49;0;48;0
WireConnection;43;0;41;0
WireConnection;50;0;49;0
WireConnection;44;0;43;0
WireConnection;102;32;44;0
WireConnection;102;31;6;0
WireConnection;102;29;11;0
WireConnection;102;33;103;0
WireConnection;102;10;12;0
WireConnection;101;32;50;0
WireConnection;101;31;6;0
WireConnection;101;29;100;0
WireConnection;101;33;103;0
WireConnection;101;10;53;0
WireConnection;51;0;102;0
WireConnection;51;1;101;0
WireConnection;61;0;51;0
WireConnection;61;2;75;0
WireConnection;61;3;76;0
WireConnection;60;0;74;0
WireConnection;66;0;61;0
WireConnection;66;1;65;0
WireConnection;59;0;37;0
WireConnection;59;1;60;0
WireConnection;64;0;59;0
WireConnection;64;1;66;0
WireConnection;63;0;64;0
WireConnection;70;0;69;0
WireConnection;70;1;64;0
WireConnection;0;0;8;0
WireConnection;0;1;51;0
WireConnection;0;2;70;0
WireConnection;0;3;13;0
WireConnection;0;4;10;0
WireConnection;0;9;63;0
ASEEND*/
//CHKSM=D15974C85BF6C0DD56335BA45E0A98AD5581AB14