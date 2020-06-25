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

Texture2D texInput;
sampler2D texInputMap
{
    Texture = <texInput>;
	  Filter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

float pixHeight;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

/*
water algorithm
    - if pixel is equal to colour water color, sample up to 50 pixels above point
        - if pixel x-i = water colour, add 1 to i
        - if pixel x-i != water colour, retrieve color from (2*i - 1) pixels above
*/

float when_lt(float x, float y) {
  return max(sign(y - x), 0.0);
}

float when_ge(float x, float y) {
  return 1.0 - when_lt(x, y);
}

float when_gt(float x, float y) {
  return max(sign(x - y), 0.0);
}

float when_eq(float x, float y) {
  return 1.0 - abs(sign(x - y));
}
float when_neq(float x, float y) {
  return abs(sign(x - y));
}

float map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
{
    return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
}

int horizonPixels = 21;
float4 main(VertexShaderOutput input) : COLOR
{
    float4 inputColor = tex2D(TextureSampler, input.TextureCoordinates);
    float4 outputColor = float4(0.0, 0.0, 0.0, 0.0);
    float i = 0;
    float2 pos = float2(0, 0);
    float found = 0;

    [unroll(21)] while (inputColor.b == 1.0)
    {
        i++;
        pos = input.TextureCoordinates - float2(0.0, pixHeight * i);
        inputColor = tex2D(TextureSampler, pos);
        outputColor = tex2D(texInputMap, input.TextureCoordinates - float2(0.0, pixHeight * ((2 * i) - 1))) * when_ge(pos.y, pixHeight * horizonPixels);
        outputColor.a = 1 - (i / horizonPixels);

        found += when_neq(inputColor.b, 1.0);
    }

    outputColor.a *= (input.TextureCoordinates.y * 1.4285) - 0.4285 * when_gt(found, 0.0);

    if (outputColor.a == 0.0)
      return float4(0, 0, 0, 0);
    else
      return outputColor;
}

technique LUT
{
	pass P0
	{
    AlphaBlendEnable = TRUE;
    DestBlend = INVSRCALPHA;
    SrcBlend = SRCALPHA;
		PixelShader = compile PS_SHADERMODEL main();
	}
};