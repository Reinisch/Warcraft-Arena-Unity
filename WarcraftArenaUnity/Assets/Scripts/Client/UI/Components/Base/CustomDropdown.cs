using System.Collections;
using System.Collections.Generic;
using Client.Tweener;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Common;
using JetBrains.Annotations;

namespace Client
{
    [AddComponentMenu("UI/Dropdown - Custom", 35)]
    [RequireComponent(typeof(RectTransform))]
    public partial class CustomDropdown : Selectable, IPointerClickHandler, ISubmitHandler, ICancelHandler
    {
        [Header("Dropdown"), Space]
        [SerializeField, UsedImplicitly] private RectTransform template;
        [SerializeField, UsedImplicitly] private TMP_Text captionText;
        [SerializeField, UsedImplicitly] private Image captionImage;
        [Header("Item"), Space]
        [SerializeField, UsedImplicitly] private TMP_Text itemText;
        [SerializeField, UsedImplicitly] private Image itemImage;
        [Header("Options"), Space]
        [SerializeField, UsedImplicitly] private int value;
        [SerializeField, UsedImplicitly] private bool explicitNavigation;
        [SerializeField, UsedImplicitly] private bool notifyOnSameValue;
        [SerializeField, UsedImplicitly] private OptionDataList options = new OptionDataList();
        [Header("Event"), Space]
        [SerializeField, UsedImplicitly] private DropdownEvent onValueChanged = new DropdownEvent();

        private static readonly OptionData NoOptionData = new OptionData();

        private bool validTemplate;
        private GameObject dropdown;
        private GameObject blocker;
        private TweenRunner<FloatTween> alphaTweenRunner;
        private readonly List<Item> items = new List<Item>();

        public RectTransform Template
        {
            get => template;
            set
            {
                template = value;
                RefreshShownValue();
            }
        }

        public TMP_Text CaptionText
        {
            get => captionText;
            set
            {
                captionText = value;
                RefreshShownValue();
            }
        }

        public Image CaptionImage
        {
            get => captionImage;
            set
            {
                captionImage = value;
                RefreshShownValue();
            }
        }

        public TMP_Text ItemText
        {
            get => itemText;
            set
            {
                itemText = value;
                RefreshShownValue();
            }
        }

        public Image ItemImage
        {
            get => itemImage;
            set
            {
                itemImage = value;
                RefreshShownValue();
            }
        }

        public List<OptionData> Options
        {
            get => options.Options;
            set
            {
                options.Options = value;
                RefreshShownValue();
            }
        }

        public DropdownEvent OnValueChanged
        {
            get => onValueChanged;
            set => onValueChanged = value;
        }

        public int Value { get => value; set => SetValue(value); }

        public bool IsExpanded => dropdown != null;

        protected override void Awake()
        {
            if (!Application.isPlaying)
                return;

            alphaTweenRunner = new TweenRunner<FloatTween>();
            alphaTweenRunner.Init(this);

            if (captionImage)
                captionImage.enabled = (captionImage.sprite != null);
            if (template)
                template.gameObject.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();

            RefreshShownValue();
        }
        
        protected override void OnDisable()
        {
            ImmediateDestroyDropdownList();

            if (blocker != null)
                DestroyBlocker(blocker);

            blocker = null;
        }

        /// <summary>
        /// Create a blocker that blocks clicks to other controls while the dropdown list is open.
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to obtain a blocker GameObject.
        /// </remarks>
        /// <param name="rootCanvas">The root canvas the dropdown is under.</param>
        /// <returns>The created blocker object</returns>
        protected GameObject CreateBlocker(Canvas rootCanvas)
        {
            // Create blocker GameObject.
            GameObject newBlocker = new GameObject("Blocker");

            // Setup blocker RectTransform to cover entire root canvas area.
            RectTransform blockerRect = newBlocker.AddComponent<RectTransform>();
            blockerRect.SetParent(rootCanvas.transform, false);
            blockerRect.anchorMin = Vector3.zero;
            blockerRect.anchorMax = Vector3.one;
            blockerRect.sizeDelta = Vector2.zero;

            // Make blocker be in separate canvas in same layer as dropdown and in layer just below it.
            Canvas blockerCanvas = newBlocker.AddComponent<Canvas>();
            blockerCanvas.overrideSorting = true;
            Canvas dropdownCanvas = dropdown.GetComponent<Canvas>();
            blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
            blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;

            // Add raycaster since it's needed to block.
            newBlocker.AddComponent<GraphicRaycaster>();

            // Add image since it's needed to block, but make it clear.
            Image blockerImage = newBlocker.AddComponent<Image>();
            blockerImage.color = Color.clear;

            // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
            Button blockerButton = newBlocker.AddComponent<Button>();
            blockerButton.navigation = new Navigation {mode = Navigation.Mode.None};
            blockerButton.onClick.AddListener(Hide);
            return newBlocker;
        }

        /// <summary>
        /// Create the dropdown list to be shown when the dropdown is clicked. The dropdown list should correspond to the provided template GameObject, equivalent to instantiating a copy of it.
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to obtain a dropdown list GameObject.
        /// </remarks>
        /// <param name="template">The template to create the dropdown list from.</param>
        /// <returns>The created drop down list gameobject.</returns>
        protected GameObject CreateDropdownList(GameObject template)
        {
            return Instantiate(template);
        }
        
        /// <summary>
        /// Create a dropdown item based upon the item template.
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to obtain an option item.
        /// The option item should correspond to the provided template DropdownItem and its GameObject, equivalent to instantiating a copy of it.
        /// </remarks>
        /// <param name="itemTemplate">e template to create the option item from.</param>
        /// <returns>The created dropdown item component</returns>
        protected Item CreateItem(Item itemTemplate)
        {
            return Instantiate(itemTemplate);
        }

        /// <summary>
        /// Convenience method to explicitly destroy the previously generated blocker object
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to dispose of a blocker GameObject that blocks clicks to other controls while the dropdown list is open.
        /// </remarks>
        /// <param name="blocker">The blocker object to destroy.</param>
        protected void DestroyBlocker(GameObject blocker)
        {
            Destroy(blocker);
        }

        /// <summary>
        /// Convenience method to explicitly destroy the previously generated dropdown list
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to dispose of a dropdown list GameObject.
        /// </remarks>
        /// <param name="dropdownList">The dropdown list GameObject to destroy</param>
        protected void DestroyDropdownList(GameObject dropdownList)
        {
            Destroy(dropdownList);
        }

        /// <summary>
        ///  Convenience method to explicitly destroy the previously generated Items.
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to dispose of an option item.
        /// Likely no action needed since destroying the dropdown list destroys all contained items as well.
        /// </remarks>
        /// <param name="item">The Item to destroy.</param>
        protected void DestroyItem(Item item)
        {
        }

        public void Show()
        {
            if (!IsActive() || !IsInteractable() || dropdown != null) return;

            // Get root Canvas.
            var list = ListPoolContainer<Canvas>.Take();
            gameObject.GetComponentsInParent(false, list);
            if (list.Count == 0) return;
            Canvas rootCanvas = list[list.Count - 1];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].isRootCanvas)
                {
                    rootCanvas = list[i];
                    break;
                }
            }

            ListPoolContainer<Canvas>.Return(list);
            if (!validTemplate)
            {
                SetupTemplate();
                if (!validTemplate) return;
            }

            template.gameObject.SetActive(true);

            // popupCanvas used to assume the root canvas had the default sorting Layer, next line fixes (case 958281 - [UI] Dropdown list does not copy the parent canvas layer when the panel is opened)
            template.GetComponent<Canvas>().sortingLayerID = rootCanvas.sortingLayerID;

            // Instantiate the drop-down template
            dropdown = CreateDropdownList(template.gameObject);
            dropdown.name = "Dropdown List";
            dropdown.SetActive(true);

            // Make drop-down RectTransform have same values as original.
            RectTransform dropdownRectTransform = (RectTransform)dropdown.transform;
            dropdownRectTransform.SetParent(template.transform.parent, false);

            // Instantiate the drop-down list items

            // Find the dropdown item and disable it.
            Item itemTemplate = dropdown.GetComponentInChildren<Item>();
            GameObject content = itemTemplate.RectTransform.parent.gameObject;
            RectTransform contentRectTransform = (RectTransform)content.transform;
            itemTemplate.RectTransform.gameObject.SetActive(true);

            // Get the rects of the dropdown and item
            Rect dropdownContentRect = contentRectTransform.rect;
            Rect itemTemplateRect = itemTemplate.RectTransform.rect;

            // Calculate the visual offset between the item's edges and the background's edges
            Vector2 offsetMin = itemTemplateRect.min - dropdownContentRect.min + (Vector2)itemTemplate.RectTransform.localPosition;
            Vector2 offsetMax = itemTemplateRect.max - dropdownContentRect.max + (Vector2)itemTemplate.RectTransform.localPosition;
            Vector2 itemSize = itemTemplateRect.size;
            items.Clear();
            Toggle prev = null;
            for (int i = 0; i < Options.Count; ++i)
            {
                OptionData data = Options[i];
                Item item = AddItem(data, Value == i, itemTemplate, items);
                if (item == null) continue;

                // Automatically set up a toggle state change listener
                item.Toggle.isOn = Value == i;
                item.Toggle.onValueChanged.AddListener(x => OnSelectItem(item.Toggle));

                // Select current option
                if (item.Toggle.isOn && explicitNavigation)
                    item.Toggle.Select();

                // Automatically set up explicit navigation
                if (prev != null && explicitNavigation)
                {
                    Navigation prevNav = prev.navigation;
                    Navigation toggleNav = item.Toggle.navigation;
                    prevNav.mode = Navigation.Mode.Explicit;
                    toggleNav.mode = Navigation.Mode.Explicit;
                    prevNav.selectOnDown = item.Toggle;
                    prevNav.selectOnRight = item.Toggle;
                    toggleNav.selectOnLeft = prev;
                    toggleNav.selectOnUp = prev;
                    prev.navigation = prevNav;
                    item.Toggle.navigation = toggleNav;
                }

                prev = item.Toggle;
            }

            // Reposition all items now that all of them have been added
            Vector2 sizeDelta = contentRectTransform.sizeDelta;
            sizeDelta.y = itemSize.y * items.Count + offsetMin.y - offsetMax.y;
            contentRectTransform.sizeDelta = sizeDelta;
            float extraSpace = dropdownRectTransform.rect.height - contentRectTransform.rect.height;
            if (extraSpace > 0) dropdownRectTransform.sizeDelta = new Vector2(dropdownRectTransform.sizeDelta.x, dropdownRectTransform.sizeDelta.y - extraSpace);

            // Invert anchoring and position if dropdown is partially or fully outside of canvas rect.
            // Typically this will have the effect of placing the dropdown above the button instead of below,
            // but it works as inversion regardless of initial setup.
            Vector3[] corners = new Vector3[4];
            dropdownRectTransform.GetWorldCorners(corners);
            RectTransform rootCanvasRectTransform = (RectTransform)rootCanvas.transform;
            Rect rootCanvasRect = rootCanvasRectTransform.rect;
            for (int axis = 0; axis < 2; axis++)
            {
                bool outside = false;
                for (int i = 0; i < 4; i++)
                {
                    Vector3 corner = rootCanvasRectTransform.InverseTransformPoint(corners[i]);
                    if ((corner[axis] < rootCanvasRect.min[axis] && !Mathf.Approximately(corner[axis], rootCanvasRect.min[axis])) ||
                        (corner[axis] > rootCanvasRect.max[axis] && !Mathf.Approximately(corner[axis], rootCanvasRect.max[axis])))
                    {
                        outside = true;
                        break;
                    }
                }

                if (outside) RectTransformUtility.FlipLayoutOnAxis(dropdownRectTransform, axis, false, false);
            }

            for (int i = 0; i < items.Count; i++)
            {
                RectTransform itemRect = items[i].RectTransform;
                itemRect.anchorMin = new Vector2(itemRect.anchorMin.x, 0);
                itemRect.anchorMax = new Vector2(itemRect.anchorMax.x, 0);
                itemRect.anchoredPosition = new Vector2(itemRect.anchoredPosition.x, offsetMin.y + itemSize.y * (items.Count - 1 - i) + itemSize.y * itemRect.pivot.y);
                itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemSize.y);
            }

            // Fade in the popup
            AlphaFadeList(0.15f, 0f, 1f);

            // Make drop-down template and item template inactive
            template.gameObject.SetActive(false);
            itemTemplate.gameObject.SetActive(false);
            blocker = CreateBlocker(rootCanvas);
        }

        public void Hide()
        {
            if (dropdown != null)
            {
                AlphaFadeList(0.15f, 0f);

                // User could have disabled the dropdown during the OnValueChanged call.
                if (IsActive()) StartCoroutine(DelayedDestroyDropdownList(0.15f));
            }

            if (blocker != null)
                DestroyBlocker(blocker);
            blocker = null;

            if (explicitNavigation)
                Select();
        }

        public void RefreshShownValue()
        {
            OptionData data = NoOptionData;
            if (Options.Count > 0) data = Options[Mathf.Clamp(value, 0, Options.Count - 1)];
            if (captionText)
            {
                if (data != null && data.Text != null) captionText.text = data.Text;
                else captionText.text = "";
            }

            if (captionImage)
            {
                if (data != null) captionImage.sprite = data.Image;
                else captionImage.sprite = null;
                captionImage.enabled = (captionImage.sprite != null);
            }
        }

        public void SetValueWithoutNotify(int input)
        {
            SetValue(input, false);
        }

        public void AddOptions(List<OptionData> options)
        {
            Options.AddRange(options);

            RefreshShownValue();
        }

        public void AddOptions(List<string> options)
        {
            for (int i = 0; i < options.Count; i++)
                Options.Add(new OptionData(options[i]));

            RefreshShownValue();
        }

        public void AddOptions(List<Sprite> options)
        {
            for (int i = 0; i < options.Count; i++)
                Options.Add(new OptionData(options[i]));

            RefreshShownValue();
        }

        public void ClearOptions()
        {
            Options.Clear();
            value = 0;
            RefreshShownValue();
        }

        /// <summary>
        /// Handling for when the dropdown is initially 'clicked'. Typically shows the dropdown
        /// </summary>
        /// <param name="eventData">The associated event data.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            Show();
        }

        /// <summary>
        /// Handling for when the dropdown is selected and a submit event is processed. Typically shows the dropdown
        /// </summary>
        /// <param name="eventData">The associated event data.</param>
        public void OnSubmit(BaseEventData eventData)
        {
            Show();
        }

        /// <summary>
        /// This will hide the dropdown list.
        /// </summary>
        /// <remarks>
        /// Called by a BaseInputModule when a Cancel event occurs.
        /// </remarks>
        /// <param name="eventData">The associated event data.</param>
        public void OnCancel(BaseEventData eventData)
        {
            Hide();
        }
       
        private Item AddItem(OptionData data, bool selected, Item itemTemplate, List<Item> items)
        {
            // Add a new item to the dropdown.
            Item item = CreateItem(itemTemplate);
            item.RectTransform.SetParent(itemTemplate.RectTransform.parent, false);
            item.gameObject.SetActive(true);
            item.gameObject.name = "Item " + items.Count + (data.Text != null ? ": " + data.Text : "");
            if (item.Toggle != null)
                item.Toggle.isOn = false;

            if (item.Text)
                item.Text.text = data.Text;

            if (item.Image)
            {
                item.Image.sprite = data.Image;
                item.Image.enabled = (item.Image.sprite != null);
            }

            items.Add(item);
            return item;
        }

        private void AlphaFadeList(float duration, float alpha)
        {
            CanvasGroup group = dropdown.GetComponent<CanvasGroup>();
            AlphaFadeList(duration, group.alpha, alpha);
        }

        private void AlphaFadeList(float duration, float start, float end)
        {
            if (end.Equals(start))
                return;

            FloatTween tween = new FloatTween {Duration = duration, StartValue = start, TargetValue = end};
            tween.AddOnChangedCallback(SetAlpha);
            tween.IgnoreTimeScale = true;
            alphaTweenRunner.StartTween(tween);
        }

        private void SetAlpha(float alpha)
        {
            if (!dropdown)
                return;

            CanvasGroup group = dropdown.GetComponent<CanvasGroup>();
            group.alpha = alpha;
        }

        private void SetValue(int value, bool sendCallback = true)
        {
            if (Application.isPlaying && (value == this.value && !notifyOnSameValue || Options.Count == 0))
                return;

            this.value = Mathf.Clamp(value, 0, Options.Count - 1);
            RefreshShownValue();

            if (sendCallback)
            {
                // Notify all listeners
                UISystemProfilerApi.AddMarker("Dropdown.value", this);
                onValueChanged.Invoke(this.value);
            }
        }

        private void SetupTemplate()
        {
            validTemplate = false;
            if (!template)
            {
                Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this);
                return;
            }

            GameObject templateGo = template.gameObject;
            templateGo.SetActive(true);
            Toggle itemToggle = template.GetComponentInChildren<Toggle>();
            validTemplate = true;
            if (!itemToggle || itemToggle.transform == Template)
            {
                validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", Template);
            }
            else if (!(itemToggle.transform.parent is RectTransform))
            {
                validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", Template);
            }
            else if (ItemText != null && !ItemText.transform.IsChildOf(itemToggle.transform))
            {
                validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", Template);
            }
            else if (ItemImage != null && !ItemImage.transform.IsChildOf(itemToggle.transform))
            {
                validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", Template);
            }

            if (!validTemplate)
            {
                templateGo.SetActive(false);
                return;
            }

            Item item = itemToggle.gameObject.AddComponent<Item>();
            item.Text = itemText;
            item.Image = itemImage;
            item.Toggle = itemToggle;
            item.RectTransform = (RectTransform)itemToggle.transform;
            Canvas popupCanvas = GetOrAddComponent<Canvas>(templateGo);
            popupCanvas.overrideSorting = true;
            popupCanvas.sortingOrder = 30000;
            GetOrAddComponent<GraphicRaycaster>(templateGo);
            GetOrAddComponent<CanvasGroup>(templateGo);
            templateGo.SetActive(false);
            validTemplate = true;
        }

        private IEnumerator DelayedDestroyDropdownList(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            ImmediateDestroyDropdownList();
        }

        private T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (!comp)
                comp = go.AddComponent<T>();

            return comp;
        }

        private void ImmediateDestroyDropdownList()
        {
            for (int i = 0; i < items.Count; i++)
                if (items[i] != null)
                    DestroyItem(items[i]);

            items.Clear();
            if (dropdown != null)
                DestroyDropdownList(dropdown);

            dropdown = null;
        }

        private void OnSelectItem(Toggle toggle)
        {
            if (!toggle.isOn)
                toggle.isOn = true;

            int selectedIndex = -1;
            Transform tr = toggle.transform;
            Transform parent = tr.parent;

            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i) == tr)
                {
                    // subtract one to account for template child.
                    selectedIndex = i - 1;
                    break;
                }
            }

            if (selectedIndex < 0)
                return;

            Value = selectedIndex;
            Hide();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!IsActive())
                return;

            RefreshShownValue();
        }
#endif
    }
}