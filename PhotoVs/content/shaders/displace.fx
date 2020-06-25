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

float offsetX;
float offsetY;
float pixHeight;
float pixWidth;

float4 highlightWater = float4(0.37647058823, 0.70588235294, 0.84705882352, 1.0);

float4 main(VertexShaderOutput input) : COLOR
{
    float4 mask = tex2D(TextureSampler, input.TextureCoordinates);

    float4 displace = tex2D(texSamplerDisplace, input.TextureCoordinates + float2(offsetX, offsetY));
	float2 adjusted = input.TextureCoordinates + (displace.rg * float2(pixWidth * 8, 0) - float2(pixWidth * 4, 0));
	float4 output = tex2D(TextureSampler, adjusted);

	if (mask.a == 0)
		return float4(0.0, 0.0, 0.0, 0.0);

	if (mask.a != 0 && (output.r == 0 && output.g == 0 && output.b == 0))
		output = highlightWater;

	return output;
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};