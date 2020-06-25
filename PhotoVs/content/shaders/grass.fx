#if OPENGL
	#define SV_POSITION POSITION
	#define PS_SHADERMODEL ps_2_0
#else
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D texMap;
sampler2D texSamplerMap = sampler_state
{
	Texture = <texMap>;
	Filter = POINT;
};

Texture2D texInput;
sampler2D texSamplerInput = sampler_state
{
	Texture = <texInput>;
	Filter = POINT;
};

float threshold;

sampler2D s0 : register(s0);

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 when_eq(float4 x, float4 y) {
  return 1.0 - abs(sign(x - y));
}

float when_eq(float x, float y) {
  return 1.0 - abs(sign(x - y));
}

float when_gt(float x, float y) {
  return max(sign(x - y), 0.0);
}

float4 main(VertexShaderOutput input) : COLOR
{
    float4 inputColor = tex2D(texSamplerMap, input.TextureCoordinates);
    float4 sampleColor = tex2D(texSamplerInput, input.TextureCoordinates);

    float outputTrans = sampleColor.r * when_gt(sampleColor.r, threshold);
    float outputColor = 1 * when_gt(outputTrans, threshold);
    outputColor *= when_eq(inputColor.r, 1);
    outputTrans *= when_eq(sampleColor.r, threshold);

	return float4(outputColor, outputColor, outputColor, outputTrans);
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};