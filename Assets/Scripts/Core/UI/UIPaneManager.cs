using System.Collections.Generic;
using DilmerGames.UI;
using DilmerGames.Core.Singletons;
using UnityEngine;

namespace DilmerGames.Managers
{
    public class UIPaneManager : Singleton<UIPaneManager>
    {
        private Dictionary<string, UIPane> panels = new Dictionary<string, UIPane>();

        private void Awake()
        {
            var temp = Resources.FindObjectsOfTypeAll<UIPane>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (!temp[i].accessableToPanesClass)
                    continue;

                if (temp[i].identity == "")
                    continue;

                if (panels.ContainsKey(temp[i].identity))
                {
                    Debug.LogWarning("Multiple Panes Keys Detected: " + temp[i].identity, temp[i].gameObject);
                    continue;
                }

                panels.Add(temp[i].identity, temp[i]);
            }
        }

        private void ShowPanel(string id)
        {
            if (panels.ContainsKey(id))
                panels[id].Show();
            else
                Debug.LogWarning("Could not find panel id: " + id);
        }

        private void HidePanel(string id)
        {
            if (panels.ContainsKey(id))
                panels[id].Hide();
            else
                Debug.LogWarning("Could not find panel id: " + id);
        }

        private void ShowPanel(string id, float speed)
        {
            if (panels.ContainsKey(id))
                panels[id].Show(speed);
            else
                Debug.LogWarning("Could not find panel id: " + id);
        }

        private void HidePanel(string id, float speed)
        {
            if (panels.ContainsKey(id))
                panels[id].Hide(speed);
            else
                Debug.LogWarning("Could not find panel id: " + id);
        }

        private void ShowPanelFast(string id)
        {
            if (panels.ContainsKey(id))
                panels[id].ShowFast();
            else
                Debug.LogWarning("Could not find panel id: " + id);
        }

        private bool IsPanelOpen(string id)
        {
            if (panels.ContainsKey(id))
                return panels[id].IsShowing;
            else
                return false;
        }


        private void HidePanelFast(string id)
        {
            if (panels.ContainsKey(id))
                panels[id].HideFast();
            else
                Debug.LogWarning("Could not find panel id: " + id);
        }

        #region Static Methods
        
        public static bool IsOpen(string id)
        {
            return Instance.IsPanelOpen(id);
        }

        public static void Show(string id, float speed)
        {
            Instance.ShowPanel(id, speed);
        }

        public static void Show(string id)
        {
            Instance.ShowPanel(id);
        }

        public static void ShowFast(string id)
        {
            Instance.ShowPanelFast(id);
        }

        public static void Hide(string id, float speed)
        {
            Instance.HidePanel(id, speed);
        }

        public static void Hide(string id)
        {
            Instance.HidePanel(id);
        }

        public static void HideFast(string id)
        {
            Instance.HidePanelFast(id);
        }
        #endregion
    }
}