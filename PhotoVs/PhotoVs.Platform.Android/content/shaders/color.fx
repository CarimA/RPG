#if OPENGL
	#define SV_POSITION POSITION
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_5_0
#endif

Texture2D LutTexture;
sampler2D LutTextureSampler = sampler_state
{
	Texture = <LutTexture>;
	Filter = POINT;
};

float LutWidth;
float LutHeight;

sampler2D s0 : register(s0);

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 main(VertexShaderOutput input) : COLOR
{
	// get colour of current pixel
	float4 color = tex2D(s0, input.TextureCoordinates);

	// check colour against lookup table
	float2 position = float2(((floor(color.r * 31.0) * 32.0) + floor(color.g * 31.0)) / LutWidth, floor(color.b * 31.0) / LutHeight);

	// return colour from lookup table
	float4 new_color = tex2D(LutTextureSampler, position);

	return new_color;
}

technique LUT
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
};