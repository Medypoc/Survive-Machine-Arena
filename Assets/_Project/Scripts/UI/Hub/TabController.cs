using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabController : MonoBehaviour
{
    [System.Serializable]
    public struct TabGroup
    {
        public string name;      // Название для удобства в инспекторе
        public Button tabButton; // Кнопка вкладки
        public GameObject panel; // Панель, которая открывается
    }

    [Header("Настройки вкладок")]
    [SerializeField] private List<TabGroup> tabs;
    [SerializeField] private int defaultTabIndex = 0;

    [Header("Визуал (Опционально)")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.7f, 0.7f, 0.7f);

    void Start()
    {
        // Инициализация кнопок
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i; // Локальная переменная для замыкания
            if (tabs[i].tabButton != null)
            {
                tabs[i].tabButton.onClick.AddListener(() => OpenTab(index));
            }
        }

        // Открываем вкладку по умолчанию
        OpenTab(defaultTabIndex);
    }

    public void OpenTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        for (int i = 0; i < tabs.Count; i++)
        {
            bool isActive = (i == index);

            // Включаем/выключаем панель
            if (tabs[i].panel != null)
                tabs[i].panel.SetActive(isActive);

            // Меняем цвет кнопки (если нужно)
            if (tabs[i].tabButton != null)
            {
                Image btnImage = tabs[i].tabButton.GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.color = isActive ? activeColor : inactiveColor;
                }
            }
        }
    }

    // Метод для закрытия всех вкладок (например, при закрытии всего каталога)
    public void CloseAll()
    {
        foreach (var tab in tabs)
        {
            if (tab.panel != null) tab.panel.SetActive(false);
        }
    }
}