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

int ImageWidth;
int ImageHeight;
float Brightness;
float Contrast;
int ModFactor = 4;

float4 MainPS(float4 pos: SV_POSITION, float4 col : COLOR0, float2 coords : TEXCOORD0) : COLOR
{
    float4 color = tex2D(s0, coords);

    // first, pick out rgb vertical lines
    int xpos = (coords.x * ImageWidth) % ModFactor;
    float4 outColor = color;
    if (xpos == 1) {
        outColor.r = color.r;
        outColor.g = color.g * 0.85f;
        outColor.b = color.b * 0.6f;
    }
    if (xpos == 2) {
        outColor.g = color.g;
        outColor.b = color.b * 0.87f;
        outColor.r = color.r * 0.55f;
    }
    if (xpos == 3) {
        outColor.b = color.b;
        outColor.r = color.r * 0.9f;
        outColor.g = color.g * 0.7f;
    }

    // now include the horizontal scanlines
    float ypos = fmod(coords.y * ImageHeight, 4);
    if (ypos > 1 && ypos <= 2) {
        outColor *= 0.76;
    }
    if (ypos > 2 && ypos <= 3) {
        outColor *= 1.2;
    }

    //int a = saturate((input.Position.y * ImageHeight) % 4);
    //int b = saturate((input.Position.y * ImageHeight + 1) % 4);
    //float m = min(a, b);

    //outColor *= m;

    // now increase the brightness/contrast
    outColor += (Brightness / 255);
    outColor = outColor - Contrast * (outColor - 1.0) * outColor * (outColor - 0.5f);

    return outColor;
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