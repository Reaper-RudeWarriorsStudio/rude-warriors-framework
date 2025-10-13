using System.Collections.Generic;
using UnityEngine;

namespace RudeWarriors.Framework.Core
{
    public interface IUIService
    {
        void Register(string name, GameObject panel);
        void Unregister(string name);
        void Show(string name);
        void Hide(string name);
        void Toggle(string name);
        GameObject GetPanel(string name);
        void HideAll();
    }

    /// <summary>
    /// Centralized UI controller for showing, hiding, and managing panels.
    /// Register this in the FrameworkBootstrapper.
    /// </summary>
    public class UIService : MonoBehaviour, IUIService
    {
        [Header("Pre-registered panels (optional)")]
        [SerializeField] private List<GameObject> initialPanels = new();

        private readonly Dictionary<string, GameObject> _panels = new();

        private void Awake()
        {
            foreach (var panel in initialPanels)
            {
                if (panel != null)
                    Register(panel.name, panel);
            }
        }

        public void Register(string name, GameObject panel)
        {
            if (string.IsNullOrEmpty(name) || panel == null)
                return;

            if (_panels.ContainsKey(name))
            {
                Debug.LogWarning($"[UIService] Panel '{name}' already registered. Replacing.");
                _panels[name] = panel;
            }
            else _panels.Add(name, panel);
        }

        public void Unregister(string name)
        {
            if (_panels.ContainsKey(name))
                _panels.Remove(name);
        }

        public void Show(string name)
        {
            if (!_panels.TryGetValue(name, out var panel))
            {
                Debug.LogWarning($"[UIService] Panel '{name}' not found.");
                return;
            }

            panel.SetActive(true);
        }

        public void Hide(string name)
        {
            if (_panels.TryGetValue(name, out var panel))
                panel.SetActive(false);
        }

        public void Toggle(string name)
        {
            if (_panels.TryGetValue(name, out var panel))
                panel.SetActive(!panel.activeSelf);
        }

        public GameObject GetPanel(string name)
        {
            _panels.TryGetValue(name, out var panel);
            return panel;
        }

        public void HideAll()
        {
            foreach (var panel in _panels.Values)
                if (panel != null)
                    panel.SetActive(false);
        }
    }
}
