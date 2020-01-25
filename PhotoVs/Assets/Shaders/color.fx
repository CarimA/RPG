#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// reminder: OGL uses the wrong bloody coordinate system

sampler s0 : register(s0);

texture2D palette;
sampler s_palette = sampler_state { 
	Texture = <palette>;
	AddressU = WRAP;
	AddressV = WRAP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};

float ind = 32;
float tex_width;
float tex_height;

float4 zeroes = float4(0, 0, 0, 0);
float4 ones = float4(1, 1, 1, 1);
float4 units = float4(31, 31, 31, 31);

float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR0
{
	float4 m = floor(lerp(zeroes, units, lerp(zeroes, ones, tex2D(s0, coords))));
	return tex2D(s_palette, float2(((m.r * ind) + m.g) / tex_width, (m.b / tex_height)));
}

technique BasicColorDrawing
{
	pass P0
	{
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};