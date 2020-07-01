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

Texture2D texNoise;
sampler2D texSamplerNoise
{
    Texture = <texNoise>;
	MipFilter = POINT;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = WRAP;
    AddressV = WRAP;
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
float time;
float waveSpeed;
float waveFreq;
float waveAmp;
float2 texelSize;
float2 direction;

float4 main(VertexShaderOutput input) : COLOR
{
    float4 mask = tex2D(texSamplerMask, input.TextureCoordinates);
    if (mask.g == 0.0)
        return tex2D(TextureSampler, input.TextureCoordinates);

    float PI = 3.14159265;
    float2 uv = input.TextureCoordinates;
    float d = uv.x + 0.3 * uv.y + tex2D(texSamplerNoise, uv).x * 0.03 - exp(-1.0 + uv.x) + exp(-uv.y);
    float a = fmod(waveSpeed * time + d * waveFreq, 4.0 * PI);

    //uv.x += cos(a) * nwaveAmp;
    //uv.y += sin(a) * waveAmp;

    uv.y += (((sin(a * 3.5 + time * 0.35) + sin(a * 4.8 + time * 1.05) + sin(a * 7.3 + time * 0.45) + sin(a)) / 4.0) * (texelSize.x * mask.g * waveAmp)) * waveSpeed;
    uv.x += (((cos(a * 4.0 + time * 0.5) + cos(a * 6.8 + time * 0.75) + cos(a * 11.3 + time * 0.2) + cos(a)) / 4.0) * (texelSize.y * mask.g * waveAmp)) * waveSpeed;

    float2 pos = uv; // + float2(0, waveAmp * sin(time + 1.5 * PI * input.TextureCoordinates.x));

    float4 color = tex2D(TextureSampler, pos - float2(offsetX, offsetY));
        
	return color;
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};