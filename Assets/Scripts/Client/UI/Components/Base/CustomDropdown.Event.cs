using System;
using UnityEngine.Events;

namespace Client
{
    public partial class CustomDropdown
    {
        /// <summary>
        /// UnityEvent callback for when a dropdown current option is changed.
        /// </summary>
        [Serializable]
        public class DropdownEvent : UnityEvent<int>
        {
        }
    }
}
