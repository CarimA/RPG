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
    AddressU = WRAP;
    AddressV = WRAP;
};

Texture2D texMask;
sampler2D texSamplerMask
{
    Texture = <texMask>;
    AddressU = WRAP;
    AddressV = WRAP;
};

Texture2D texNoiseGrass;
sampler2D texSamplerNoiseGrass
{
    Texture = <texNoiseGrass>;
    AddressU = WRAP;
    AddressV = WRAP;
};

Texture2D texNoise;
sampler2D texSamplerNoise
{
    Texture = <texNoise>;
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
float threshold;

float4 lightGrass = float4(0.81176470588, 0.81176470588, 0.12549019607, 1.0);
float4 mediumGrass = float4(0.52156862745, 0.60784313725, 0.1725490196, 1.0);
float4 darkGrass = float4(0.33725490196, 0.44705882352, 0.19215686274, 1.0);

float4 main(VertexShaderOutput input) : COLOR
{
    float4 maskColor = tex2D(texSamplerMask, input.TextureCoordinates);

	if (maskColor.r != 1.0)
		return float4(0.0, 0.0, 0.0, 0.0);

    float4 noiseA = tex2D(texSamplerNoiseGrass, input.TextureCoordinates);
    float4 noiseB = tex2D(texSamplerNoise, input.TextureCoordinates + float2(offsetX, offsetY));
	float4 avg = (noiseA + noiseB) / 2;

	if (avg.r < threshold)
		return float4(0.0, 0.0, 0.0, 0.0);

    float4 inputColor = tex2D(TextureSampler, input.TextureCoordinates);
	
	if (inputColor.r == lightGrass.r && inputColor.g == lightGrass.g && inputColor.b == lightGrass.b)
		if (avg.r > 0.85)
			return darkGrass;
		else
			return mediumGrass;

	if (inputColor.r == mediumGrass.r && inputColor.g == mediumGrass.g && inputColor.b == mediumGrass.b)
		if (avg.r > 0.9)
			return lightGrass;
		else
			return darkGrass;

	return float4(0.0, 0.0, 0.0, 0.0);
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};