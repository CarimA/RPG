#if OPENGL
	#define SV_POSITION POSITION
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

Texture2D Texture : register(t0);
sampler TextureSampler : register(s0)
{
    Texture = (Texture);
    AddressU = CLAMP;
    AddressV = CLAMP;
};

Texture2D texDisplace;
sampler2D texSamplerDisplace
{
    Texture = <texDisplace>;
    AddressU = WRAP;
    AddressV = WRAP;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 when_eq(float4 x, float4 y) {
  return 1.0 - abs(sign(x - y));
}

float offsetXA;
float offsetYA;


float4 main(VertexShaderOutput input) : COLOR
{
    float4 maskColor = tex2D(TextureSampler, input.TextureCoordinates);


    return water * avg;
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};