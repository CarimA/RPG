#if OPENGL
	#define SV_POSITION POSITION
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_5_0
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

Texture2D texNoiseA;
sampler2D texSamplerNoiseA
{
    Texture = <texNoiseA>;
	MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};

Texture2D texNoiseB;
sampler2D texSamplerNoiseB
{
    Texture = <texNoiseB>;
	MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 when_gt(float4 x, float4 y) {
  return max(sign(x - y), 0.0);
}

float2 offsetA;
float2 offsetB;

float4 colorA;
float4 colorB;
float4 colorC;

float2 maskSize;
float2 noiseSize;

float pulses;

float4 main(VertexShaderOutput input) : COLOR
{
    // first, check if we want to do anything here
    float4 mask = tex2D(TextureSampler, input.TextureCoordinates);

    if (mask.r != 1.0 || mask.g != 0.0 || mask.b != 1.0)
        return mask;

    // mix the two noise textures
    float4 noiseA = tex2D(texSamplerNoiseA, (input.TextureCoordinates * (maskSize / noiseSize) * 3.0) + offsetA);
    float4 noiseB = tex2D(texSamplerNoiseB, (input.TextureCoordinates * (maskSize / noiseSize) * 3.0) + offsetB);
    float avg = noiseA.r + noiseB.r;
    avg = fmod(round(avg * pulses), 3.0);

    if (avg < 1)
        return colorA;
    else if (avg - 1.0 < 1)
        return colorB;
    else if (avg - 2.0 < 1)
        return colorC;

    return float4(0, 0, 0, 0);
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};