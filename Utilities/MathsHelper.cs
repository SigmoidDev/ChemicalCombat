using UnityEngine;

namespace Sigmoid.Utilities
{
    /// <summary>
    /// Provides various helper functions to do with positions and distances
    /// </summary>
    public static class MathsHelper
    {
        /// <summary>
        /// Returns the angle of this vector to the positive direction (atan2 of the vector)
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float GetAngle(this Vector2 vector) => Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        /// <summary>
        /// Lerps between two angles (in radians), accounting for full rotations being equal
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float LerpAngle(float a, float b, float t)
        {
            float twoPi = 2f * Mathf.PI;
            float delta = (b - a) % twoPi;
            if(delta > Mathf.PI) delta -= twoPi;
            if(delta < -Mathf.PI) delta += twoPi;
            return a + t * delta;
        }

        /// <summary>
        /// Fixes C#'s strange behaviour when taking the modulus of a negative number<br/>
        /// For example, Mod(-2, 5) gives 3 rather than -2
        /// </summary>
        /// <param name="number"></param>
        /// <param name="modulus"></param>
        /// <returns></returns>
        public static float Mod(float number, float modulus) => (number % modulus + modulus) % modulus;

        /// <summary>
        /// Gets the square root, but allows for negatives too (takes it out and adds back after)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static float SafeRoot(float number) => Mathf.Sign(number) * Mathf.Sqrt(Mathf.Abs(number));

        /// <summary>
        /// Evaluates an exponential decay for the falloff of things like screenshake based on distance<br/>
        /// v = f^-d/r
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="radius"></param>
        /// <param name="falloff"></param>
        /// <returns></returns>
        public static float DistanceFalloff(float distance, float radius, float falloff) => Mathf.Pow(falloff, - distance / radius);

        /// <summary>
        /// Gets the semi-accurate torque that should be applied to a Rigidbody2D if a force is applied at a given position
        /// </summary>
        /// <param name="body"></param>
        /// <param name="force"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static float ForceToTorque(Rigidbody2D body, Vector2 force, Vector2 position)
        {
            Vector2 diff = body.worldCenterOfMass - position;
            float angle = Mathf.Atan2(diff.y, diff.x) - Mathf.Atan2(force.y, force.x);
            return diff.magnitude * force.magnitude * Mathf.Sin(angle) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Gets the angle between the positive direction of the center of the screen and the mouse
        /// </summary>
        /// <returns></returns>
        public static float GetMouseAngle()
        {
            Vector2 mousePosition = Input.mousePosition / new Vector2(Screen.width, Screen.height);
			Vector2 mouseOffset = mousePosition - new Vector2(0.5f, 0.5f);
			return mouseOffset.GetAngle();
        }

        /// <summary>
        /// Adds a vector-like distance extension method to the Color class
        /// </summary>
        /// <param name="colA"></param>
        /// <param name="colB"></param>
        /// <returns>The 4D-Euclidean distance between the two colours</returns>
        public static float DistanceTo(this Color colA, Color colB)
        {
            float r = colA.r - colB.r;
            float g = colA.g - colB.g;
            float b = colA.b - colB.b;
            float a = colA.a - colB.a;
            return Mathf.Sqrt(r * r + g * g + b * b + a * a);
        }

        /// <summary>
        /// Returns the sum of the absolutes of the components
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float Magnitude1D(this Vector2 vector) => Mathf.Abs(vector.x) + Mathf.Abs(vector.y);
        public static int Magnitude1D(this Vector2Int vector) => (int) Magnitude1D((Vector2) vector);

        /// <summary>
        /// Returns the hex code of a given colour (idk why this isn't built-in)
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        public static string GetHexCode(this Color colour)
        {
            int r = (int) (255f * colour.r);
            int g = (int) (255f * colour.g);
            int b = (int) (255f * colour.b);
            return "#" + GetHexCode(r) + GetHexCode(g) + GetHexCode(b);
        }

        private static readonly string[] HEX_DIGITS = new string[]{"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F"};

        /// <summary>
        /// Converts a decimal number (range 0-255) to a 2 character hex string;
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static string GetHexCode(int value)
        {
            int first = value >> 4;
            int second = value % 16;
            return HEX_DIGITS[first] + HEX_DIGITS[second];
        }
    }
}