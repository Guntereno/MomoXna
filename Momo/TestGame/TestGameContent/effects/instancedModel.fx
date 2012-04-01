float4x4 world;
float4x4 viewProjection;


// This sample uses a simple Lambert lighting model.
float3 lightDirection = normalize(float3(0, -1, 0));
float3 diffuseLight = 0.50;
float3 ambientLight = 0.60;



texture diffuseTex;

sampler diffuseSampler = sampler_state
{
    Texture = (diffuseTex);
};


struct VertexShaderInput
{
    float4 position				: POSITION0;
    float3 normal				: NORMAL0;
    float2 texCoord				: TEXCOORD0;
	float4x4 instanceTransform	: BLENDWEIGHT;
};


struct VertexShaderOutput
{
    float4 position		: POSITION0;
    float4 colour		: COLOR0;
    float2 texCoord		: TEXCOORD0;
};



VertexShaderOutput vs( VertexShaderInput input )
{
    VertexShaderOutput output;

	float4x4 instanceWorldTransform = transpose( input.instanceTransform );
    float4 worldPosition = mul( input.position, instanceWorldTransform );
	worldPosition = mul( world, worldPosition );

    //float4 viewPosition = mul( worldPosition, view );
    output.position = mul( worldPosition, viewProjection );

    float3 worldNormal = mul( input.normal, input.instanceTransform );
    
    float diffuseAmount = max( -dot( worldNormal, lightDirection ), 0.0f );
    
    float3 lightingResult = saturate( diffuseAmount * diffuseLight + ambientLight );
    
    output.colour = float4( lightingResult, 1.0f );

    output.texCoord = input.texCoord;

    return output;
}


float4 ps( VertexShaderOutput input ) : COLOR0
{
    return tex2D( diffuseSampler, input.texCoord ) * input.colour;
}


technique HardwareInstancing
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 vs();
        PixelShader = compile ps_3_0 ps();
    }
}
