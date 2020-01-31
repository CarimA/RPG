#if OPENGL
	#define SV_POSITION POSITION
	#define PS_SHADERMODEL ps_2_0
#else
	#define PS_SHADERMODEL ps_4_0_level_9_1
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

float4 zeroes = float4(0, 0, 0, 0);
float4 ones = float4(1, 1, 1, 1);
float4 units = float4(31, 31, 31, 31);

float4 main(VertexShaderOutput input) : COLOR
{
	// get colour of current pixel
	float4 color = tex2D(s0, input.TextureCoordinates);
	color = floor(lerp(zeroes, units, color));

	// check colour against lookup table
	float2 position = float2(((color.r * 32) + color.g) / LutWidth, color.b / LutHeight);
	
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