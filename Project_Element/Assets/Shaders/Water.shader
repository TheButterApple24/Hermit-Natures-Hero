Shader "Project Element/Water"
{
	Properties
	{
		_MainTex("Water Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
		_DepthFactor("Depth Factor", float) = 1.0
		_WaveSpeed("Wave Speed", float) = 1.0
		_WaveAmp("Wave Amplitude", float) = 0.2
		_ExtraHeight("Extra Height", float) = 0.0
		_ScrollXSpeed("X", Range(-10,10)) = 2
		_ScrollYSpeed("Y", Range(-10,10)) = 3
		_NoiseTex("Noise Texture", 2D) = "white" {}

	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"
		}
	Pass
	{
	Blend SrcAlpha OneMinusSrcAlpha

	CGPROGRAM
	#include "UnityCG.cginc"

	#pragma vertex vert
	#pragma fragment frag

	// Properties
	sampler2D _CameraDepthTexture;
	float4 _Color;
	float4 _EdgeColor;
	float  _DepthFactor;
	float _WaveSpeed;
	float _WaveAmp;
	float _ExtraHeight;
	float4 _MainTex_ST;
	fixed _ScrollXSpeed;
	fixed _ScrollYSpeed;
	sampler2D _NoiseTex;
	sampler2D _MainTex;

	struct vertexInput
	 {
		float4 vertex : POSITION;
		float4 texCoord : TEXCOORD1;
	 };

	struct vertexOutput
	 {
		float4 pos : SV_POSITION;
		float4 texCoord : TEXCOORD0;
		float4 screenPos : TEXCOORD1;
	 };

	vertexOutput vert(vertexInput input)
	  {
		vertexOutput output;

		// convert to world space
		output.pos = UnityObjectToClipPos(input.vertex);

		// apply wave animation
		float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
		output.pos.y += sin(_Time * _WaveSpeed * noiseSample) * _WaveAmp;
		output.pos.x += cos(_Time * _WaveSpeed * noiseSample) * _WaveAmp + _ExtraHeight;

		// compute depth
		output.screenPos = ComputeScreenPos(output.pos);

		// texture coordinates 
		output.texCoord = input.texCoord;

		return output;
	  }

	float4 frag(vertexOutput input) : COLOR
	{
	  fixed2 scrolledUV = input.texCoord;

	  float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
	  float depth = LinearEyeDepth(depthSample).r;

	  // apply the DepthFactor to be able to tune at what depth values
	  // the foam line actually starts
	  float foamLine = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));

	  fixed xScrollValue = _ScrollXSpeed * _Time;
	  fixed yScrollValue = _ScrollYSpeed * _Time;

	  scrolledUV += fixed2(xScrollValue, yScrollValue);

	  float mainTex = tex2D(_MainTex, float4(scrolledUV.xy * _MainTex_ST, 0, 0));

	  // multiply the edge color by the foam factor to get the edge,
	  // then add that to the color of the water
	  float4 col = _Color + mainTex + foamLine * _EdgeColor;

	  return col;
	}

	ENDCG
	}
	}
}
