#if OPENGL
	#define SV_POSITION POSITION
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

// Pixel shader applies a one dimensional gaussian blur filter.
// This is used twice by the bloom postprocess, first to
// blur horizontally, and then again to blur vertically.


Texture2D Texture : register(t0);
sampler TextureSampler : register(s0)
{
    Texture = (Texture);
    AddressU = CLAMP;
    AddressV = CLAMP;
};

#define SAMPLE_COUNT 15

float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};


float4 main(VertexShaderOutput input) : COLOR
{
    float4 c = 0;
    
    // Combine a number of weighted image filter taps.
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        c += tex2D(TextureSampler, input.TextureCoordinates + SampleOffsets[i]) * SampleWeights[i];
    }
    
    return c;
}


technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};