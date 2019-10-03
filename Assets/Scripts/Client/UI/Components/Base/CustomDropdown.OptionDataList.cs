using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public partial class CustomDropdown
    {
        /// <summary>
        /// Class used internally to store the list of options for the dropdown list.
        /// </summary>
        /// <remarks>
        /// The usage of this class is not exposed in the runtime API. It's only relevant for the PropertyDrawer drawing the list of options.
        /// </remarks>
        [Serializable]
        public class OptionDataList
        {
            [SerializeField] private List<OptionData> optionsList;

            /// <summary>
            /// The list of options for the dropdown list.
            /// </summary>
            public List<OptionData> Options
            {
                get => optionsList;
                set => optionsList = value;
            }

            public OptionDataList()
            {
                Options = new List<OptionData>();
            }
        }
    }
}
