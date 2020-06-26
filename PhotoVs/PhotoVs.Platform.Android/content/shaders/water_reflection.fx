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

float when_neq(float x, float y) {
  return abs(sign(x - y));
}

float4 water = float4(0.03529411764, 0.3725490196, 0.47843137254, 1.0);
int horizonPixels = 50;

float4 main(VertexShaderOutput input) : COLOR
{
    float4 inputColor = tex2D(TextureSampler, input.TextureCoordinates);
    float4 outputColor = float4(0.0, 0.0, 0.0, 0.0);
    float i = 0;
    float2 pos = float2(0, 0);
    float found = 0;

    [unroll(50)] while (inputColor.b == 1.0)
    {
        i++;
        pos = input.TextureCoordinates - float2(0.0, pixHeight * i);
        inputColor = tex2D(TextureSampler, pos);
        outputColor = tex2D(texInputMap, input.TextureCoordinates - float2(0.0, pixHeight * ((2 * i) - 1)));
        outputColor.a = 1 - (i / horizonPixels);

        found += when_neq(inputColor.b, 1.0);
    }

    outputColor.a *= max(input.TextureCoordinates.y - (pixHeight * horizonPixels), 0);
    outputColor.a -= (pixHeight * horizonPixels);
    outputColor.a *= 2.5;

    if (outputColor.a == 0.0 || (outputColor.r == water.r && outputColor.g == water.g && outputColor.b == water.b))
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