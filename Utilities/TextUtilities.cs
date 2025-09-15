using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sigmoid.UI;
using UnityEngine;
using TMPro;


namespace Sigmoid.Utilities
{
	public static class TextUtilities
	{
        /// <summary>
        /// Adds spaces between capital letters based on some weird Regex that I copied from somewhere<br/>
        /// e.g. SampleText1 -> Sample Text 1
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		public static string PascalToTitle(string input) => Regex.Replace(input, "(\\p{Ll})(\\p{Lu})|(?<=\\p{Lu})(\\p{Lu})(?=\\p{Ll}|\\d)", "$1 $2");



        /// <summary>
        /// Performs a huge switch to abbreviate some common keycode names<br/>
        /// e.g. LeftCommand -> LCmd
        /// </summary>
        /// <param name="keycode"></param>
        /// <returns></returns>
        public static string AbbreviateKeycode(KeyCode keycode) => keycode switch
        {
            KeyCode.LeftShift => "LShift",
            KeyCode.RightShift => "RShift",
            KeyCode.LeftControl => "LCtrl",
            KeyCode.RightControl => "RCtrl",
            KeyCode.LeftAlt => "LAlt",
            KeyCode.RightAlt => "RAlt",
            KeyCode.LeftCommand => "LCmd",
            KeyCode.RightCommand => "RCmd",

            KeyCode.Mouse0 => "LClick",
            KeyCode.Mouse1 => "RClick",
            KeyCode.Mouse2 => "MClick",
            KeyCode.Mouse3 => "M3",
            KeyCode.Mouse4 => "M4",
            KeyCode.Mouse5 => "M5",
            KeyCode.Mouse6 => "M6",
            KeyCode.UpArrow => "^",
            KeyCode.DownArrow => "v",
            KeyCode.LeftArrow => "<",
            KeyCode.RightArrow => ">",

            KeyCode.Return => "Enter",
            KeyCode.Backspace => "Back",
            KeyCode.Tab => "Tab",
            KeyCode.CapsLock => "Caps",
            KeyCode.Space => "Space",

            KeyCode.BackQuote => "`",
            KeyCode.Minus => "-",
            KeyCode.Equals => "=",
            KeyCode.LeftBracket => "[",
            KeyCode.RightBracket => "]",
            KeyCode.Backslash => "\\",
            KeyCode.Semicolon => ";",
            KeyCode.Quote => "'",
            KeyCode.Comma => ",",
            KeyCode.Period => ".",
            KeyCode.Slash => "/",
            KeyCode.Hash => "#",

            KeyCode.Alpha0 => "0",
            KeyCode.Alpha1 => "1",
            KeyCode.Alpha2 => "2",
            KeyCode.Alpha3 => "3",
            KeyCode.Alpha4 => "4",
            KeyCode.Alpha5 => "5",
            KeyCode.Alpha6 => "6",
            KeyCode.Alpha7 => "7",
            KeyCode.Alpha8 => "8",
            KeyCode.Alpha9 => "9",

            KeyCode.Keypad0 => "N0",
            KeyCode.Keypad1 => "N1",
            KeyCode.Keypad2 => "N2",
            KeyCode.Keypad3 => "N3",
            KeyCode.Keypad4 => "N4",
            KeyCode.Keypad5 => "N5",
            KeyCode.Keypad6 => "N6",
            KeyCode.Keypad7 => "N7",
            KeyCode.Keypad8 => "N8",
            KeyCode.Keypad9 => "N9",
            KeyCode.KeypadPeriod => "N.",
            KeyCode.KeypadDivide => "N/",
            KeyCode.KeypadMultiply => "N*",
            KeyCode.KeypadMinus => "N-",
            KeyCode.KeypadPlus => "N+",
            KeyCode.KeypadEnter => "NEnter",

            _ => keycode.ToString()
        };



        /// <summary>
        /// Slowly reveals the text of a TextMeshPro element over a given duration<br/>
        /// </summary>
        /// <param name="component"></param>
        /// <param name="fullText"></param>
        /// <param name="duration"></param>
        /// <param name="unscaled"></param>
        /// <returns></returns>
		public static IEnumerator RevealText(TextMeshProUGUI component, string fullText, float duration, bool unscaled = false)
        {
            duration *= Options.Current.AnimationTimeMultiplier;

            string text = "";
            int length = fullText.Length;
            int current = 0;
            float reciprocal = 1f / duration;

            for(float elapsed = 0f; elapsed < duration; elapsed += unscaled ? Time.unscaledDeltaTime : Time.deltaTime)
            {
                yield return null;

                int index = (int) (elapsed * reciprocal * length);
                if(index == current) continue;

                for(int i = current; i < index; i++)
                    text += fullText[i];

                current = index;
                component.SetText(text);
            }

            component.SetText(fullText);
        }



        /// <summary>
        /// Forms a string with each item of the IEnumerable representing an individual line
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string BuildText(this IEnumerable<string> lines)
        {
            StringBuilder builder = new StringBuilder();
            foreach(string line in lines)
                builder.AppendLine(line);

            return builder.ToString();
        }
    }
}
