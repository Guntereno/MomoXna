//=============================================================================
// Text, untransformed vertices.
//=============================================================================

texture gTex			: TEXTURE1;
float2 gViewHalfDim		: VIEWPORT_HALF_DIMENSION;
float4 gColour			: COLOUR1;
float4 gOutlineColour	: COLOUR2;


sampler TexSampler = sampler_state
{
	Texture = <gTex>;
	//MagFilter = LINEAR;
	//MinFilter = LINEAR;
	//AddressU = CLAMP;
	//AddressV = CLAMP;
};


void TextVS(float2 posL : POSITION0, float2 uv : TEXCOORD0, 
		  out float4 oPosH : POSITION0, out float2 oUv : TEXCOORD0)
{
	posL.xy -= float2(0.5f + gViewHalfDim.x, 0.5f);
	posL.y = gViewHalfDim.y - posL.y;
	posL.xy /= gViewHalfDim;

	oPosH = float4(posL, 0.0f, 1.0f);
	oUv = uv;
}


float4 TextPS(float2 uv : TEXCOORD0) : COLOR
{
	float texColour = tex2D(TexSampler, uv).r;

	//float4 final = lerp(gOutlineColour, gColour, texColour.y);
	//final.a = final.a * texColour.x;

	return gColour * texColour;
}


float4 TextOutlinePS(float2 uv : TEXCOORD0) : COLOR
{
	float texColour = tex2D(TexSampler, uv).a;

	return gOutlineColour * texColour;
}


technique SpriteV20
{
	pass TextOutline
	{
		vertexShader = compile vs_3_0 TextVS();
		pixelShader  = compile ps_3_0 TextOutlinePS();
	}

	pass Text
	{
		vertexShader = compile vs_3_0 TextVS();
		pixelShader  = compile ps_3_0 TextPS();
	}
}