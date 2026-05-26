using System;
using System.Collections.Generic;
using BYOJoystick.Actions;
using BYOJoystick.Bindings;
using BYOJoystick.UI.Scripts;
using SharpDX.DirectInput;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Manager = BYOJoystick.Managers.Base.Manager;

namespace BYOJoystick.UI
{
    public class BYOJUI : MonoBehaviour
    {
        public GameObject       VehicleSelection;
        public VehicleSelector  VehicleSelectorPrefab;
        public GameObject       Devices;
        public GameObject       DeviceHeaderPrefab;
        public Button           RefreshDevicesButton;
        public GameObject       CategoryActionsList;
        public CategoryActions  CategoryActionsPrefab;
        public GameObject       BindingGrid;
        public BindingCell      BindingCellPrefab;
        public GameObject       BindingCellSpacerPrefab;
        public GameObject       BindingCellInvalidPrefab;
        public BYOJBindingModal BYOJBindingModal;

        private readonly List<VehicleSelector> _selectors = new List<VehicleSelector>();
        private          Manager               _selectedManager;
        private          int                   _currentPage;
        private          int                   _itemsPerPage = 0;
        private          Button                _prevPageButton;
        private          Button          _nextPageButton;
        private          TextMeshProUGUI _pageIndicator;
        public           bool            IsBinding => BYOJBindingModal.IsBinding;

        public void Initialise()
        {
            PopulateVehicleSelectors();
            PopulateDeviceHeaders();
            OnSelectVehicle(_selectors[0].ShortName);
            RefreshDevicesButton.onClick.AddListener(OnRefreshDevices);
        }

        private void PopulateVehicleSelectors()
        {
            foreach (Transform child in VehicleSelection.transform)
            {
                Destroy(child.gameObject);
            }

            _selectors.Clear();

            var managers = BYOJ.GetAllManagers();
            foreach (var manager in managers)
            {
                var selector = Instantiate(VehicleSelectorPrefab, VehicleSelection.transform);
                _selectors.Add(selector);
                selector.Initialise(manager.GameName, manager.ShortName, OnSelectVehicle, OnSaveVehicle, OnLoadVehicle, OnCopyFromVehicle);
            }

            // Create paging controls if needed
            CreatePagingControlsIfNeeded();
            // Recalculate items per page and update visible page
            CalculateItemsPerPage();
            _currentPage = 0;
            UpdatePage();
        }

        private void CreatePagingControlsIfNeeded()
        {
            // Parent for controls
            var parent = VehicleSelection.transform.parent as RectTransform;
            if (parent == null)
                return;

            // Get font from an existing text component in a prefab so our dynamic text renders properly
            TMP_FontAsset font = null;
            Material fontMaterial = null;
            if (VehicleSelectorPrefab != null && VehicleSelectorPrefab.SelectText != null)
            {
                font = VehicleSelectorPrefab.SelectText.font;
                fontMaterial = VehicleSelectorPrefab.SelectText.fontSharedMaterial;
            }
            
            if (_prevPageButton == null)
            {
                var prevGo = new GameObject("PrevPageButton", typeof(RectTransform), typeof(Button), typeof(Image));
                prevGo.transform.SetParent(parent, false);
                var rt = prevGo.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                // position slightly inset and size a bit larger for touchability
                rt.anchoredPosition = new Vector2(12f, -12f);
                rt.sizeDelta = new Vector2(40f, 34f);
                var img = prevGo.GetComponent<Image>();
                img.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
                img.raycastTarget = true;
                // add a thin outline for contrast
                var outlinePrev = prevGo.AddComponent<Outline>();
                outlinePrev.effectColor = new Color(0f, 0f, 0f, 0.9f);
                outlinePrev.effectDistance = new Vector2(1f, -1f);
                _prevPageButton = prevGo.GetComponent<Button>();
                _prevPageButton.onClick.AddListener(() => { if (_currentPage > 0) { _currentPage--; UpdatePage(); } });

                var txtGo = new GameObject("Text", typeof(RectTransform));
                txtGo.transform.SetParent(prevGo.transform, false);
                var txt = txtGo.AddComponent<TextMeshProUGUI>();
                if (font != null) txt.font = font;
                if (fontMaterial != null) txt.fontSharedMaterial = fontMaterial;
                txt.text = "<";
                txt.fontSize = 20;
                txt.color = Color.white;
                txt.alignment = TextAlignmentOptions.Center;
                txt.raycastTarget = false;
                txt.enableAutoSizing = false;
                txt.fontStyle = FontStyles.Bold;
                var tRt = txtGo.GetComponent<RectTransform>();
                tRt.anchorMin = Vector2.zero; tRt.anchorMax = Vector2.one; tRt.sizeDelta = Vector2.zero;
            }

            if (_nextPageButton == null)
            {
                var nextGo = new GameObject("NextPageButton", typeof(RectTransform), typeof(Button), typeof(Image));
                nextGo.transform.SetParent(parent, false);
                var rt = nextGo.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(1f, 1f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(1f, 1f);
                // move left from the right edge so it does not touch the edge and not overlap prev
                rt.anchoredPosition = new Vector2(-52f, -12f);
                rt.sizeDelta = new Vector2(40f, 34f);
                var img = nextGo.GetComponent<Image>();
                img.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
                img.raycastTarget = true;
                var outlineNext = nextGo.AddComponent<Outline>();
                outlineNext.effectColor = new Color(0f, 0f, 0f, 0.9f);
                outlineNext.effectDistance = new Vector2(1f, -1f);
                _nextPageButton = nextGo.GetComponent<Button>();
                _nextPageButton.onClick.AddListener(() => { _currentPage++; UpdatePage(); });

                var txtGo = new GameObject("Text", typeof(RectTransform));
                txtGo.transform.SetParent(nextGo.transform, false);
                var txt = txtGo.AddComponent<TextMeshProUGUI>();
                if (font != null) txt.font = font;
                if (fontMaterial != null) txt.fontSharedMaterial = fontMaterial;
                txt.text = ">";
                txt.fontSize = 20;
                txt.color = Color.white;
                txt.alignment = TextAlignmentOptions.Center;
                txt.raycastTarget = false;
                txt.enableAutoSizing = false;
                txt.fontStyle = FontStyles.Bold;
                var tRt = txtGo.GetComponent<RectTransform>();
                tRt.anchorMin = Vector2.zero; tRt.anchorMax = Vector2.one; tRt.sizeDelta = Vector2.zero;
            }

            if (_pageIndicator == null)
            {
                var pgGo = new GameObject("PageIndicator", typeof(RectTransform));
                pgGo.transform.SetParent(parent, false);
                var rt = pgGo.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(0f, -10f);
                rt.sizeDelta = new Vector2(110f, 30f);
                // background for indicator for readability
                var bg = pgGo.AddComponent<Image>();
                bg.color = new Color(0f, 0f, 0f, 0.45f);
                var outlinePg = pgGo.AddComponent<Outline>();
                outlinePg.effectColor = new Color(1f, 1f, 1f, 0.08f);
                outlinePg.effectDistance = new Vector2(1f, -1f);
                var pageTextGo = new GameObject("PageText", typeof(RectTransform));
                pageTextGo.transform.SetParent(pgGo.transform, false);
                _pageIndicator = pageTextGo.AddComponent<TextMeshProUGUI>();
                if (font != null) _pageIndicator.font = font;
                if (fontMaterial != null) _pageIndicator.fontSharedMaterial = fontMaterial;
                var textRt = _pageIndicator.GetComponent<RectTransform>();
                textRt.anchorMin = Vector2.zero; textRt.anchorMax = Vector2.one; textRt.sizeDelta = Vector2.zero;
                _pageIndicator.fontSize = 14;
                _pageIndicator.alignment = TextAlignmentOptions.Center;
                _pageIndicator.color = Color.white;
                _pageIndicator.raycastTarget = false;
                _pageIndicator.enableAutoSizing = false;
                _pageIndicator.fontStyle = FontStyles.Bold;
            }
        }

        private void CalculateItemsPerPage()
        {
            _itemsPerPage = 0;
            if (_selectors.Count == 0)
                return;

            var parentRt = VehicleSelection.transform.parent as RectTransform;
            if (parentRt == null)
            {
                _itemsPerPage = _selectors.Count;
                return;
            }

            float availableWidth = parentRt.rect.width;
            if (availableWidth <= 0)
                availableWidth = Screen.width * 0.8f;

            // estimate selector width from first selector RectTransform or fallback
            var firstRt = _selectors[0].GetComponent<RectTransform>();
            float selWidth = firstRt != null ? (firstRt.sizeDelta.x != 0 ? firstRt.sizeDelta.x : firstRt.rect.width) : 150f;
            if (selWidth <= 0) selWidth = 150f;

            _itemsPerPage = Math.Max(1, (int)(availableWidth / selWidth));
        }

        private void UpdatePage()
        {
            if (_itemsPerPage <= 0)
                CalculateItemsPerPage();

            int total = _selectors.Count;
            int totalPages = (int)Math.Ceiling(total / (double)_itemsPerPage);
            if (_currentPage < 0) _currentPage = 0;
            if (_currentPage >= totalPages) _currentPage = Math.Max(0, totalPages - 1);

            int start = _currentPage * _itemsPerPage;
            int end = Math.Min(start + _itemsPerPage, total);
            for (int i = 0; i < total; i++)
            {
                _selectors[i].gameObject.SetActive(i >= start && i < end);
            }

            if (_prevPageButton != null)
                _prevPageButton.interactable = _currentPage > 0;
            if (_nextPageButton != null)
                _nextPageButton.interactable = _currentPage < totalPages - 1;

            if (_pageIndicator != null)
                _pageIndicator.text = totalPages <= 1 ? string.Empty : $"Page {_currentPage + 1}/{totalPages}";
        }

        private void PopulateDeviceHeaders()
        {
            foreach (Transform child in Devices.transform)
                Destroy(child.gameObject);

            var keyboardHeader = Instantiate(DeviceHeaderPrefab, Devices.transform);
            keyboardHeader.GetComponentInChildren<TextMeshProUGUI>().text = "Keyboard";

            foreach (var device in BYOJ.GetAllJoysticks())
            {
                var header = Instantiate(DeviceHeaderPrefab, Devices.transform);
                header.GetComponentInChildren<TextMeshProUGUI>().text = device.Information.InstanceName;
            }
        }

        private void PopulateActions()
        {
            foreach (Transform child in CategoryActionsList.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (ActionCategory category in Enum.GetValues(typeof(ActionCategory)))
            {
                var actions = _selectedManager.ControlActionsByCategory[category];
                if (actions.Count == 0)
                    continue;
                var categoryActions = Instantiate(CategoryActionsPrefab, CategoryActionsList.transform);
                categoryActions.Initialise(category, actions);
            }
        }

        private void PopulateBindings()
        {
            foreach (Transform child in BindingGrid.transform)
            {
                Destroy(child.gameObject);
            }

            var joysticks = BYOJ.GetAllJoysticks();

            var gridLayout = BindingGrid.GetComponent<GridLayoutGroup>();
            gridLayout.constraintCount = 1 + joysticks.Count;

            var keyboardBindings = BYOJ.ConfigManager.GetKeyboardBindingStore(_selectedManager.ShortName);

            foreach (ActionCategory category in Enum.GetValues(typeof(ActionCategory)))
            {
                var actions = _selectedManager.ControlActionsByCategory[category];
                if (actions.Count == 0)
                    continue;

                Instantiate(BindingCellSpacerPrefab, BindingGrid.transform);

                foreach (var action in actions)
                {
                    var bindings = keyboardBindings.Get(action.Name);
                    if (category != ActionCategory.Modifier && action.Input == ActionInput.Button)
                    {
                        var cell = Instantiate(BindingCellPrefab, BindingGrid.transform);
                        cell.Initialise(OnBindAction, action, bindings, isKeyboard: true);
                    }
                    else
                        Instantiate(BindingCellInvalidPrefab, BindingGrid.transform);
                }
            }

            foreach (var device in joysticks)
            {
                var joystickBindings = BYOJ.ConfigManager.GetJoystickBindingStore(_selectedManager.ShortName, device.Information.InstanceGuid);

                foreach (ActionCategory category in Enum.GetValues(typeof(ActionCategory)))
                {
                    var actions = _selectedManager.ControlActionsByCategory[category];
                    if (actions.Count == 0)
                        continue;

                    Instantiate(BindingCellSpacerPrefab, BindingGrid.transform);

                    foreach (var action in actions)
                    {
                        var bindings = joystickBindings.Get(action.Name);
                        var cell     = Instantiate(BindingCellPrefab, BindingGrid.transform);
                        cell.Initialise(OnBindAction, action, bindings, device.Information.InstanceGuid);
                    }
                }
            }
        }

        private void OnSelectVehicle(string shortName)
        {
            foreach (var selector in _selectors)
            {
                selector.SetSelected(selector.ShortName == shortName);
            }

            _selectedManager = BYOJ.GetManager(shortName);
            PopulateActions();
            PopulateBindings();
        }

        private void OnSaveVehicle(string shortName)
        {
            BYOJ.ConfigManager.SaveBindings(shortName);
        }

        private void OnLoadVehicle(string shortName)
        {
            BYOJ.ConfigManager.LoadBindings(shortName);
            PopulateBindings();
            if (_selectedManager == BYOJ.ActiveManager)
                BYOJ.ReloadActiveManager();
        }

        private void OnCopyFromVehicle(string shortName)
        {
            var fromManager = BYOJ.GetManager(shortName);
            BYOJ.ConfigManager.CopyBindings(fromManager, _selectedManager);
            PopulateBindings();
            BYOJ.ReloadActiveManager();
        }

        private void OnBindAction(BindingCell cell, Guid joystickId, bool isKeyboard, ControlAction action)
        {
            BYOJBindingModal.gameObject.SetActive(true);
            if (isKeyboard)
            {
                var bindings = BYOJ.ConfigManager.GetKeyboardBindingStore(_selectedManager.ShortName).Get(action.Name);
                BYOJBindingModal.ShowBindingModal(cell, action, bindings, true, null, OnKeyboardBindingComplete, OnBindingCancelled);
            }
            else
            {
                var bindings = BYOJ.ConfigManager.GetJoystickBindingStore(_selectedManager.ShortName, joystickId).Get(action.Name);
                var joystick = BYOJ.GetConnectedJoystick(joystickId);
                if (joystick == null)
                {
                    BYOJBindingModal.gameObject.SetActive(false);
                    throw new InvalidOperationException($"No joystick found with ID {joystickId}");
                }

                BYOJBindingModal.ShowBindingModal(cell, action, bindings, false, joystick, OnJoystickBindingComplete, OnBindingCancelled);
            }
        }

        private void OnKeyboardBindingComplete(BindingCell   cell,
                                               ControlAction action,
                                               bool          isKeyboard,
                                               Joystick      joystick,
                                               List<Binding> newBindings)
        {
            BYOJBindingModal.gameObject.SetActive(false);

            var bindingStore = BYOJ.ConfigManager.GetKeyboardBindingStore(_selectedManager.ShortName);
            var bindings     = bindingStore.Get(action.Name);
            if (_selectedManager == BYOJ.ActiveManager)
            {
                foreach (var binding in bindings)
                {
                    BYOJ.StopListeningForKeyboardBinding(binding);
                }
            }

            bindings.Clear();

            foreach (var newBinding in newBindings)
            {
                bindings.Add(newBinding as KeyboardBinding);
                if (_selectedManager == BYOJ.ActiveManager)
                    BYOJ.StartListeningForKeyboardBinding(newBinding as KeyboardBinding);
            }

            cell.UpdateText();
        }

        private void OnJoystickBindingComplete(BindingCell   cell,
                                               ControlAction action,
                                               bool          isKeyboard,
                                               Joystick      joystick,
                                               List<Binding> newBindings)
        {
            BYOJBindingModal.gameObject.SetActive(false);

            var bindingStore = BYOJ.ConfigManager.GetJoystickBindingStore(_selectedManager.ShortName, joystick.Information.InstanceGuid);
            var bindings     = bindingStore.Get(action.Name);

            bool modifiersChanged = false;

            if (_selectedManager == BYOJ.ActiveManager)
            {
                foreach (var binding in bindings)
                {
                    BYOJ.StopListeningForJoystickBinding(binding);
                    if (binding.Target == "Modifier" || binding.RequiresModifier)
                        modifiersChanged = true;
                }
            }

            bindings.Clear();

            foreach (var newBinding in newBindings)
            {
                var binding = newBinding as JoystickBinding;
                bindings.Add(binding);
                if (_selectedManager != BYOJ.ActiveManager)
                    continue;
                if (binding!.Target == "Modifier" || binding.RequiresModifier)
                    modifiersChanged = true;
                BYOJ.StartListeningForJoystickBinding(binding);
                BYOJ.ListenForModifierPresses(binding);
            }
            cell.UpdateText();

            // Reload the active manager if anything to do with modifiers was changed to ensure correct behaviour
            if (modifiersChanged)
                BYOJ.ReloadActiveManager();
        }

        public void OnBindingCancelled()
        {
            BYOJBindingModal.gameObject.SetActive(false);
        }

        private void OnRefreshDevices()
        {
            BYOJ.UpdateConnectedJoysticks();
            PopulateDeviceHeaders();
            PopulateBindings();
        }

        // Reserved for future per-resolution UI tweaks. Currently a no-op to avoid impacting other mods.
        private void ApplyScreenFitLayout() { }
    }
}