#if OPENGL
	#define SV_POSITION POSITION
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_5_0
#endif

Texture2D texA;
sampler2D texSamplerA = sampler_state
{
	Texture = <texA>;
	Filter = POINT;
};

Texture2D texB;
sampler2D texSamplerB = sampler_state
{
	Texture = <texB>;
	Filter = POINT;
};

float phase;

sampler2D s0 : register(s0);

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 main(VertexShaderOutput input) : COLOR
{
	// check colour against lookup table
	float4 colorA = tex2D(texSamplerA, input.TextureCoordinates) * (phase);
    float4 colorB = tex2D(texSamplerB, input.TextureCoordinates) * (1 - phase);

	float4 new_color = colorA + colorB;

	return new_color;
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};