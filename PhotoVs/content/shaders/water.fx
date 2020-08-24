#if OPENGL
	#define SV_POSITION POSITION
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

sampler TextureSampler : register(s0)
{
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

float2 offsetA;
float2 offsetB;
float2 cameraPos;
float contrast;
int step;
float pixWidth;
float pixHeight;
float4 water;
float4 highlightWater;
float scale;

float4 main(VertexShaderOutput input) : COLOR
{
    float4 maskColor = tex2D(TextureSampler, input.TextureCoordinates);
	if (maskColor.r != water.r && maskColor.g != water.g && maskColor.b != water.b)
		return maskColor;

	float2 c = cameraPos;
	c.x = c.x * 0.5;
    float4 noiseA = tex2D(texSamplerNoiseA, scale * ((c * pixWidth) + input.TextureCoordinates + float2(offsetA.x, offsetA.y + 0.5)));
    float4 noiseB = tex2D(texSamplerNoiseB, scale * ((c * pixHeight) + input.TextureCoordinates + float2(offsetB.x + 0.5, offsetB.y)));
	float4 avg = lerp(noiseA, noiseB, 0.5);

	avg.rgb = max(avg.rgb, 0.15);
	avg.rgb = min(avg.rgb, 0.85);

	if (avg.r >= 0.75 && maskColor.a == 1.0)
		return highlightWater;

	avg.rgb = round(avg.rgb * step) / step;
	avg.rgb = ((avg.rgb - 0.5) * max(contrast, 0)) + 0.5;
	avg.rgb += 0.15;
		
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