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

Texture2D texNoiseA;
sampler2D texSamplerNoiseA
{
    Texture = <texNoiseA>;
    AddressU = WRAP;
    AddressV = WRAP;
};

Texture2D texNoiseB;
sampler2D texSamplerNoiseB
{
    Texture = <texNoiseB>;
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
float offsetXB;
float offsetYB;

float contrast = 1.1;
int step = 6;

float4 water = float4(0.03529411764, 0.3725490196, 0.47843137254, 1.0);
float4 highlightWater = float4(0.37647058823, 0.70588235294, 0.84705882352, 1.0);

float4 main(VertexShaderOutput input) : COLOR
{
    float4 maskColor = tex2D(TextureSampler, input.TextureCoordinates);
	if (maskColor.b != 1.0)
		return float4(0.0, 0.0, 0.0, 0.0);

    float4 noiseA = tex2D(texSamplerNoiseA, input.TextureCoordinates + float2(offsetXA, offsetYA));
    float4 noiseB = tex2D(texSamplerNoiseB, input.TextureCoordinates + float2(offsetXB, offsetYB));
	float4 avg = (noiseA + noiseB) / 2;


	avg.rgb = max(avg.rgb, 0.15);
	avg.rgb = min(avg.rgb, 0.85);

	//if (avg.r >= 0.75)
	//	return highlightWater;

	avg.rgb = round(avg.rgb * step) / step;
	avg.rgb = ((avg.rgb - 0.5) * max(contrast, 0)) + 0.5;
	avg.rgb += 0.15;

    return water * avg;
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};