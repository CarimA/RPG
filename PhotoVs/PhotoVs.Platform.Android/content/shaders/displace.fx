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
	MipFilter = POINT;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

Texture2D texMask;
sampler2D texSamplerMask
{
    Texture = <texMask>;
	MipFilter = POINT;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = WRAP;
    AddressV = WRAP;
};

Texture2D texDisplace;
sampler2D texSamplerDisplace
{
    Texture = <texDisplace>;
	MipFilter = POINT;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = WRAP;
    AddressV = WRAP;
};

Texture2D texDisplace2;
sampler2D texSamplerDisplace2
{
    Texture = <texDisplace2>;
	MipFilter = POINT;
    MinFilter = POINT;
    MagFilter = POINT;
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
float pixWidth;
float pixHeight;

float maxDisplace = 8;

float4 water = float4(0.03529411764, 0.3725490196, 0.47843137254, 1.0);
float4 highlightWater = float4(0.37647058823, 0.70588235294, 0.84705882352, 1.0);

float4 main(VertexShaderOutput input) : COLOR
{
    float4 displace = tex2D(texSamplerDisplace, input.TextureCoordinates + float2(offsetX, offsetY));
    float4 displace2 = tex2D(texSamplerDisplace2, input.TextureCoordinates + float2(offsetX, offsetY) + 0.5);
	float4 avg = (displace + displace2) / 2;
	float2 adjusted = input.TextureCoordinates + ((avg.rg - 0.5) * float2(pixWidth * maxDisplace, 0));
	float4 output = tex2D(TextureSampler, adjusted - float2(pixWidth / 2, pixHeight / 2));

    float4 mask = tex2D(texSamplerMask, input.TextureCoordinates - float2(pixWidth / 2, pixHeight / 2));

	if (mask.b != 1)
		return float4(0.0, 0.0, 0.0, 0.0);

	if (mask.b == 1 && (output.r == 0 && output.g == 0 && output.b == 0))
		output = water;

	return output;
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};