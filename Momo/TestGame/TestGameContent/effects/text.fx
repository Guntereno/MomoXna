//=============================================================================
// Text, untransformed vertices.
//=============================================================================

texture  gTex			: TEXTURE1;
float2 gViewHalfDim		: VIEWPORT_HALF_DIMENSION;
float4 gColour			: COLOUR1;
float4 gOutlineColour	: COLOUR2;


sampler SpriteTexS = sampler_state
{
	Texture = <gTex>;
	//MagFilter = LINEAR;
	//MinFilter = LINEAR;
	//AddressU = CLAMP;
	//AddressV = CLAMP;
};


void Sprite_VS(float3 posL : POSITION0, float2 uv : TEXCOORD0, 
		  out float4 oPosH : POSITION0, out float2 oUv : TEXCOORD0)
{
	posL.xy -= float2(0.5f + gViewHalfDim.x, 0.5f);
	posL.y = gViewHalfDim.y - posL.y;
	posL.xy /= gViewHalfDim;
	oPosH = float4(posL, 1.0f);
	oUv = uv;
}

float4 Sprite_PS(float2 uv : TEXCOORD0) : COLOR
{
	float4 texColour = tex2D(SpriteTexS, uv);
	float4 dColour = gColour - gOutlineColour;
	
	return (dColour * texColour.a) + (gOutlineColour * texColour.r);
}

technique SpriteV20
{
	pass P0
	{
		vertexShader = compile vs_2_0 Sprite_VS();
		pixelShader  = compile ps_2_0 Sprite_PS();
	}
}