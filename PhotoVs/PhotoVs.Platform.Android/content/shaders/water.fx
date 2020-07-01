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
    AddressU = WRAP;
    AddressV = WRAP;
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
float time;
float contrast;
int step;
float pixWidth;
float pixHeight;
float4 water;
float4 highlightWater;

float4 main(VertexShaderOutput input) : COLOR
{
    float4 maskColor = tex2D(TextureSampler, input.TextureCoordinates - float2(pixWidth / 2, pixHeight / 2));
	if (maskColor.b != 1.0)
		return float4(0.0, 0.0, 0.0, 0.0);

	float y = sin(time) / 2.0;

    float4 noiseA = tex2D(texSamplerNoiseA, input.TextureCoordinates + float2(offsetXA + y, offsetYA + y + 0.5));
    float4 noiseB = tex2D(texSamplerNoiseB, input.TextureCoordinates + float2(offsetXB + y + 0.5, offsetYB + y));
	float4 avg = lerp(noiseA, noiseB, 0.5);

	avg.rgb = max(avg.rgb, 0.15);
	avg.rgb = min(avg.rgb, 0.85);

	/*if (maskColor.a != 1.0) 
	{
		float4 result = water * (maskColor.a);
		result.a = 1;
		return result;
	}*/

	if (avg.r >= 0.75 && maskColor.a == 1.0)
		return highlightWater;

	avg.rgb = round(avg.rgb * step) / step;
	avg.rgb = ((avg.rgb - 0.5) * max(contrast, 0)) + 0.5;
	avg.rgb += 0.15;

	if (maskColor.a != 1.0)
	{
		float4 result = water * lerp(avg, 0.5, 1.0 - (maskColor.a - 0.25));
		result.a = 1;
		return result;
	}
		
	float4 result = water * avg;
	result.a = 1;
    return result;
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};