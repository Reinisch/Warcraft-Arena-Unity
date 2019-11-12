using System;
using UnityEngine;

namespace Client
{
    public partial class CustomDropdown
    {
        [Serializable]
        public class OptionData
        {
            [SerializeField] private string text;
            [SerializeField] private Sprite image;

            /// <summary>
            /// The text associated with the option.
            /// </summary>
            public string Text
            {
                get => text;
                set => text = value;
            }

            /// <summary>
            /// The image associated with the option.
            /// </summary>
            public Sprite Image
            {
                get => image;
                set => image = value;
            }

            public OptionData()
            {
            }

            public OptionData(string text)
            {
                Text = text;
            }

            public OptionData(Sprite image)
            {
                Image = image;
            }

            /// <summary>
            /// Create an object representing a single option for the dropdown list.
            /// </summary>
            /// <param name="text">Optional text for the option.</param>
            /// <param name="image">Optional image for the option.</param>
            public OptionData(string text, Sprite image)
            {
                Text = text;
                Image = image;
            }
        }
    }
}
