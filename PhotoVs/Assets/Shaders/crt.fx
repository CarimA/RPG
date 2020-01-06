sampler2D s0;

float2 LensCenter = float2(0.5, 0.5);
float2 Scale = float2(0.4, 0.4);
float2 ScaleIn = float2(0.55, 0.55);
float4 HmdWarpParam = float4(4.8, 4.8, 4.8, 4.8);

float2 HmdWarp(float2 in01)
{
    float2 theta = (in01 - LensCenter) * ScaleIn; // Scales to [-1, 1]
    float rSq = theta.x * theta.x + theta.y * theta.y;
    float2 theta1 = theta * (HmdWarpParam.x + HmdWarpParam.y * rSq +
        HmdWarpParam.z * rSq * rSq + HmdWarpParam.w * rSq * rSq * rSq);
    return LensCenter + Scale * theta1;
}

float4 CRT(float4 pos: SV_POSITION, float4 col : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 tc = HmdWarp(coords);
    if (any(clamp(tc, float2(0.0, 0.0), float2(1.0, 1.0)) - tc))
        return 0.0;

    return tex2D(s0, tc);
}

// hlsl port of https://www.shadertoy.com/view/XsjSzR

float2 res = float2(1.0 / 320.0, 1.0 / 180.0);

// Hardness of scanline.
//  -8.0 = soft
// -16.0 = medium
float hardScan = -12.0;

// Hardness of pixels in scanline.
// -2.0 = soft
// -4.0 = hard
float hardPix = -3.0;

// Amount of shadow mask.
float maskDark = 0.5;
float maskLight = 1.5;

float ToSrgb1(float c)
{
    return c < 0.0031308 
        ? c * 12.92 
        : 1.055 * pow(c, 0.41666) - 0.055;
}

float3 ToSrgb(float3 c) {
    return float3(ToSrgb1(c.r), ToSrgb1(c.g), ToSrgb1(c.b));
}

float ToLinear1(float c)
{
    return (c <= 0.04045)
        ? c / 12.92
        : pow((c + 0.055) / 1.055, 2.4);
}

float3 ToLinear(float3 c) {
    return float3(ToLinear1(c.r), ToLinear1(c.g), ToLinear1(c.b));
}

float3 Fetch(float2 pos, float2 off)
{
    return ToLinear(tex2D(s0, pos + (off * res)).rgb);
}

float2 Dist(float2 pos)
{
    //pos = pos * res;
    pos *= (1.0 / res);
    return -((pos - floor(pos)) - float2(0.5, 0.5));
}

float Gaus(float pos, float scale)
{
    return exp2(scale * pos * pos);
}

float3 Horz3(float2 pos, float off)
{
    float3 b = Fetch(pos, float2(-1.0, off));
    float3 c = Fetch(pos, float2(0.0, off));
    float3 d = Fetch(pos, float2(1.0, off));
    float dist = Dist(pos).x;
    float scale = hardPix;
    float wb = Gaus(dist - 1.0, scale);
    float wc = Gaus(dist, scale);
    float wd = Gaus(dist + 1.0, scale);

    return (b * wb + c * wc + d * wd) / (wb + wc + wd);
}

float3 Horz5(float2 pos, float off)
{
    float3 a = Fetch(pos, float2(-2.0, off));
    float3 b = Fetch(pos, float2(-1.0, off));
    float3 c = Fetch(pos, float2(0.0, off));
    float3 d = Fetch(pos, float2(1.0, off));
    float3 e = Fetch(pos, float2(-2.0, off));
    float dist = Dist(pos).x;
    float scale = hardPix;
    float wa = Gaus(dist - 2.0, scale);
    float wb = Gaus(dist - 1.0, scale);
    float wc = Gaus(dist, scale);
    float wd = Gaus(dist + 1.0, scale);
    float we = Gaus(dist - 2.0, scale);

    return (a * wa + b * wb + c * wc + d * wd + e * we) / (wa + wb + wc + wd + we);
}

float Scan(float2 pos, float off)
{
    float dist = Dist(pos).y;
    return Gaus(dist + off, hardScan);
}

float3 Tri(float2 pos)
{
    float3 a = Horz3(pos, -1.0);
    float3 b = Horz5(pos, 0.0);
    float3 c = Horz3(pos, 1.0);
    float wa = Scan(pos, -1.0);
    float wb = Scan(pos, 0.0);
    float wc = Scan(pos, 1.0);

    return a * wa + b * wb + c * wc;
}

float3 Mask(float2 pos)
{
    //pos.x += pos.y * 3.0;
    float3 mask = float3(maskDark, maskDark, maskDark);
    pos.x *= (1.0 / res.x);
    pos.x = frac(pos.x);

    if (pos.x < 0.333)
        mask.r = maskLight;
    else if (pos.x < 0.666)
        mask.g = maskLight;
    else
        mask.b = maskLight;

    return mask;
}

float2 Warp(float2 pos)
{
    float warp = 0.0;
    pos = pos * 2.0 - 1.0;
    pos *= float2(
        1.0 + (pos.y * pos.y) * warp,
        1.0 + (pos.x * pos.x) * warp);
    return pos * 0.5 + 0.5;
}

float4 MainPS(float4 pos: SV_POSITION, float4 col : COLOR0, float2 coords : TEXCOORD0) : COLOR
{
    float4 color = float4(
        Fetch(coords, float2(0.0, 0.0)),
        1.0);//tex2D(s0, coords);

    color.rgb = Tri(coords) * Mask(coords);
    color.a = 1.0;
    color.rgb = ToSrgb(color);

    return color;

}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 CRT();
    }

    pass Pass2
    {
        PixelShader = compile ps_4_0 MainPS();
    }
}