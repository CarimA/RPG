#if OPENGL
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

Texture2D texIndex;
sampler2D texSamplerIndex
{
    Texture = <texIndex>;
	MipFilter = POINT;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float2 viewOffset;
float2 viewSize;
float2 tileSize;
float2 inverseIndexTexSize;

float2 mapSize;

float map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
{
    return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
}

float4 main(VertexShaderOutput input) : COLOR
{
    float4 tile = tex2D(TextureSampler, input.TextureCoordinates);

    // return if not a tile
    if (tile.a == 0.0)
    {
        discard;
        return float4(0, 0, 0, 0);
    }

    // get the position of the index tile and offset by pixel needed
    float2 position = (input.Position.xy / 2.0) + 0.5;

    float2 tilePosition = floor(tile.xy * 256.0) * tileSize;

    float pX = map(input.TextureCoordinates.x, 0.0, 1.0, 0.0, mapSize.x);
    float pY = map(input.TextureCoordinates.y, 0.0, 1.0, 0.0, mapSize.y);
    float2 x = fmod(float2(pX, pY), tileSize);

    float2 pixelCoord = ((position * viewSize) + viewOffset);
    float2 tilePixelOffset = fmod(pixelCoord, tileSize);

    float4 colour = tex2D(texSamplerIndex, (tilePosition + x) * inverseIndexTexSize);
    
    return colour;
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};