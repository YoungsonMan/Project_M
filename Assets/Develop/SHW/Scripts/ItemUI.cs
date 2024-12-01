using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    private Inventory inventory;

    [Header("Items")]
    [SerializeField] Image niddle;
    [SerializeField] Image dart;

    private void Awake()
    {
        inventory = gameObject.GetComponent<Inventory>();

        if (inventory == null)
        {
            Debug.LogError("Inventory 스크립트를 찾을 수 없습니다.");
        }
    }

    private void OnEnable()
    {
        // 인벤토리 이벤트 등록
        // inventory.OnItemChanged += UpdateUI;
    }

    private void OnDisable()
    {
        // 이벤트 해제
        // inventory.OnItemChanged -= UpdateUI;
    }

    private void UpdateUI(bool hasItem, ItemBase item)
    {
        // 모든 아이템 아이콘 초기화
        ItemUIOFF();

        if (hasItem && item != null)
        {
            // 아이템 이름에 따라 아이콘 활성화
            switch (item.itemName)
            {
                case "바늘 아이템":
                    niddle.gameObject.SetActive(true);
                    break;

                case "다트 아이템":
                    dart.gameObject.SetActive(true);
                    break;

                default:
                    Debug.LogWarning($"알 수 없는 아이템: {item.itemName}");
                    break;
            }
        }
    }

    public void ItemUIOFF()
    {
        niddle.gameObject.SetActive(false);
        dart.gameObject.SetActive(false);
    }
}
