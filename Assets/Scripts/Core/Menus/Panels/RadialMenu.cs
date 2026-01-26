using System.Collections.Generic;
using UnityEngine;

namespace OverBang.ExoWorld.Core
{
    public class RadialMenu : MonoBehaviour
    {
        [SerializeField] private float radius = 250f;
        [SerializeField] private float startAngle; 
        [SerializeField] private GameObject itemPrefab; 

        private List<GameObject> menuItems;


        private void Start()
        {
            menuItems  = new List<GameObject>();
        }

        public void TestFunction()
        {
            AddItem(itemPrefab);
            AddItem(itemPrefab);
            AddItem(itemPrefab);
            AddItem(itemPrefab);
            AddItem(itemPrefab);
            AddItem(itemPrefab);
        }

        public void AddItem(GameObject item)
        {
            GameObject newItem = Instantiate(item, transform);
        
            menuItems.Add(newItem);
            RepositionItems();
        }

        private void RepositionItems()
        {
            int itemCount = menuItems.Count;
            if (itemCount == 0) return;

            float angleStep = 360f / itemCount;

            for (int i = 0; i < itemCount; i++)
            {
                float angle = (startAngle + angleStep * i) * Mathf.Deg2Rad;
            
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                menuItems[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < menuItems.Count)
            {
                Destroy(menuItems[index]);
                menuItems.RemoveAt(index);
                RepositionItems();
            }
        }

        public void ClearMenu()
        {
            foreach (GameObject item in menuItems)
            {
                Destroy(item);
            }
            menuItems.Clear();
        }
    }
}