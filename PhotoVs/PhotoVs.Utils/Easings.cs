using System;

namespace PhotoVs.Utils
{
    public static class Easings
    {
        // based on functions from https://easings.net/#

        public enum Functions
        {
            Linear,
            EaseInSine,
            EaseOutSine,
            EaseInOutSine,
            EaseInQuad,
            EaseOutQuad,
            EaseInOutQuad,
            EaseInCubic,
            EaseOutCubic,
            EaseInOutCubic,
            EaseInQuart,
            EaseOutQuart,
            EaseInOutQuart,
            EaseInQuint,
            EaseOutQuint,
            EaseInOutQuint,
            EaseInExpo,
            EaseOutExpo,
            EaseInOutExpo,
            EaseInCirc,
            EaseOutCirc,
            EaseInOutCirc,
            EaseInBack,
            EaseOutBack,
            EaseInOutBack,
            EaseInElastic,
            EaseOutElastic,
            EaseInOutElastic,
            EaseInBounce,
            EaseOutBounce,
            EaseInOutBounce
        }

        public static float Interpolate(float input, Functions function)
        {
            return function switch
            {
                Functions.Linear => input,
                Functions.EaseInSine => EaseInSine(input),
                Functions.EaseOutSine => EaseOutSine(input),
                Functions.EaseInOutSine => EaseInOutSine(input),
                Functions.EaseInQuad => EaseInQuad(input),
                Functions.EaseOutQuad => EaseOutQuad(input),
                Functions.EaseInOutQuad => EaseInOutQuad(input),
                Functions.EaseInCubic => EaseInCubic(input),
                Functions.EaseOutCubic => EaseOutCubic(input),
                Functions.EaseInOutCubic => EaseInOutCubic(input),
                Functions.EaseInQuart => EaseInQuart(input),
                Functions.EaseOutQuart => EaseOutQuart(input),
                Functions.EaseInOutQuart => EaseInOutQuart(input),
                Functions.EaseInQuint => EaseInQuint(input),
                Functions.EaseOutQuint => EaseOutQuint(input),
                Functions.EaseInOutQuint => EaseInOutQuint(input),
                Functions.EaseInExpo => EaseInExpo(input),
                Functions.EaseOutExpo => EaseOutExpo(input),
                Functions.EaseInOutExpo => EaseInOutExpo(input),
                Functions.EaseInCirc => EaseInCirc(input),
                Functions.EaseOutCirc => EaseOutCirc(input),
                Functions.EaseInOutCirc => EaseInOutCirc(input),
                Functions.EaseInBack => EaseInBack(input),
                Functions.EaseOutBack => EaseOutBack(input),
                Functions.EaseInOutBack => EaseInOutBack(input),
                Functions.EaseInElastic => EaseInElastic(input),
                Functions.EaseOutElastic => EaseOutElastic(input),
                Functions.EaseInOutElastic => EaseInOutElastic(input),
                Functions.EaseInBounce => EaseInBounce(input),
                Functions.EaseOutBounce => EaseOutBounce(input),
                Functions.EaseInOutBounce => EaseInOutBounce(input),

                _ => throw new ArgumentOutOfRangeException(nameof(function), function, null)
            };
        }

        private static float EaseInOutBounce(float input)
        {
            return input < 0.5f
                ? (1f - EaseOutBounce(1f - 2f * input)) / 2f
                : (1f + EaseOutBounce(2f * input - 1f)) / 2f;
        }

        private static float EaseOutBounce(float input)
        {
            var n1 = 7.5625f;
            var d1 = 2.75f;

            if (input < 1f / d1)
            {
                return n1 * input * input;
            }
            else if (input < 2f / d1)
            {
                return n1 * (input -= 1.5f / d1) * input + 0.75f;
            }
            else if (input < 2.5f / d1)
            {
                return n1 * (input -= 2.25f / d1) * input + 0.9375f;
            }
            else
            {
                return n1 * (input -= 2.625f / d1) * input + 0.984375f;
            }
        }

        private static float EaseInBounce(float input)
        {
            return 1f - EaseOutBounce(1f - input);
        }

        private static float EaseInOutElastic(float input)
        {
            var c5 = (float) (2f * Math.PI) / 4.5f;

            return (float) (input == 0f
                ? 0f
                : input == 1f
                    ? 1f
                    : input < 0.5f
                        ? -(Math.Pow(2f, 20f * input - 10f) * Math.Sin((20f * input - 11.125f) * c5)) / 2f
                        : (Math.Pow(2f, -20f * input + 10f) * Math.Sin((20f * input - 11.125f) * c5)) / 2f + 1f);
        }

        private static float EaseOutElastic(float input)
        {
            var c4 = (float) (2f * Math.PI) / 3f;

            return (float) (input == 0f
                ? 0f
                : input == 1f
                    ? 1f
                    : Math.Pow(2f, -10f * input) * Math.Sin((input * 10f - 0.75f) * c4) + 1f);
        }

        private static float EaseInElastic(float input)
        {
            const float c4 = (float) (2f * Math.PI) / 3f;

            return (float) (input == 0f
                ? 0f
                : input == 1f
                    ? 1f
                    : -Math.Pow(2f, 10f * input - 10f) * Math.Sin((input * 10f - 10.75f) * c4));
        }

        private static float EaseInOutBack(float input)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;

            return (float) (input < 0.5f
                ? (Math.Pow(2f * input, 2f) * ((c2 + 1f) * 2f * input - c2)) / 2f
                : (Math.Pow(2f * input - 2f, 2f) * ((c2 + 1f) * (input * 2f - 2f) + c2) + 2f) / 2f);
        }

        private static float EaseOutBack(float input)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            return (float) (1f + c3 * Math.Pow(input - 1f, 3f) + c1 * Math.Pow(input - 1f, 2f));
        }

        private static float EaseInBack(float input)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            return (float) (c3 * input * input * input - c1 * input * input);
        }

        private static float EaseInOutCirc(float input)
        {
            return (float) (input < 0.5f
                ? (1f - Math.Sqrt(1f - Math.Pow(2f * input, 2f))) / 2f
                : (Math.Sqrt(1f - Math.Pow(-2f * input + 2f, 2f)) + 1f) / 2f);
        }

        private static float EaseOutCirc(float input)
        {
            return (float) Math.Sqrt(1f - Math.Pow(input - 1f, 2f));
        }

        private static float EaseInCirc(float input)
        {
            return (float) (1f - Math.Sqrt(1f - Math.Pow(input, 2f)));
        }

        private static float EaseInOutExpo(float input)
        {
            return (float) (input == 0f
                ? 0f
                : input == 1f
                    ? 1f
                    : input < 0.5f
                        ? Math.Pow(2f, 20f * input - 10f) / 2f
                        : (2f - Math.Pow(2f, -20f * input + 10f)) / 2f);
        }

        private static float EaseOutExpo(float input)
        {
            return (float) (input == 1f ? 1f : 1f - Math.Pow(2f, -10f * input));
        }

        private static float EaseInExpo(float input)
        {
            return (float) (input == 0f ? 0f : Math.Pow(2f, 10f * input - 10f));
        }

        private static float EaseInOutQuint(float input)
        {
            return (float) (input < 0.5f
                ? 16f * input * input * input * input * input
                : 1f - Math.Pow(-2f * input + 2f, 5f) / 2f);
        }

        private static float EaseOutQuint(float input)
        {
            return (float) (1f - Math.Pow(1f - input, 5f));
        }

        private static float EaseInQuint(float input)
        {
            return input * input * input * input * input;
        }

        private static float EaseInOutQuart(float input)
        {
            return (float) (input < 0.5f
                ? 8f * input * input * input * input
                : 1f - Math.Pow(-2f * input + 2f, 4f) / 2f);
        }

        private static float EaseOutQuart(float input)
        {
            return (float) (1f - Math.Pow(1f - input, 4f));
        }

        private static float EaseInQuart(float input)
        {
            return input * input * input * input;
        }

        private static float EaseInOutQuad(float input)
        {
            return (float) (input < 0.5f ? 2f * input * input : 1f - Math.Pow(-2f * input + 2f, 2f) / 2f);
        }

        private static float EaseOutQuad(float input)
        {
            return 1f - (1f - input) * (1f - input);
        }

        private static float EaseInQuad(float input)
        {
            return input * input;
        }

        private static float EaseInSine(float input)
        {
            return (float) (1f - Math.Cos(input * Math.PI) / 2f);
        }

        private static float EaseOutSine(float input)
        {
            return (float) Math.Sin((input * Math.PI) / 2f);
        }

        private static float EaseInOutSine(float input)
        {
            return (float) -(Math.Cos(input * Math.PI) - 1f) / 2f;
        }

        private static float EaseInCubic(float input)
        {
            return input * input * input;
        }

        private static float EaseOutCubic(float input)
        {
            return (float) (1f - Math.Pow(1f - input, 3));
        }

        private static float EaseInOutCubic(float input)
        {
            return (float) (input < 0.5f
                ? 4f * EaseInCubic(input)
                : 1f - Math.Pow(-2f * input + 2f, 3f) / 2f);
        }
    }
}
