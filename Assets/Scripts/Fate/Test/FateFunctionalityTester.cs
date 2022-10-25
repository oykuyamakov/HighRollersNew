using Events;
using Fate.EventImplementations;
using Fate.ShopKeeper.EventImplementations;
using UnityEngine;
using UnityEngine.UI;
using Utility.Extensions;

public class FateFunctionalityTester : MonoBehaviour
{
    public Button FateButton;
    public Button InventoryButton;
    public Button ShopKeeperButton;

    public CanvasGroup CanvasGroup;
    
    private void OnEnable()
    {
        FateButton.onClick.AddListener(OpenFate);
//        InventoryButton.onClick.AddListener(OpenInventory);
        ShopKeeperButton.onClick.AddListener(OpenShopKeeper);
        
        GEM.AddListener<ToggleFateEditorUIEvent>(OnToggleFate);
        GEM.AddListener<ToggleShopKeeperUIEvent>(OnToggleShop);
    }

    private void OnToggleFate(ToggleFateEditorUIEvent evt)
    {
        if(evt.Visible)
            return;

        CanvasGroup.Toggle(true, 0.25f);
    }
    
    private void OnToggleShop(ToggleShopKeeperUIEvent evt)
    {
        if(evt.Visible)
            return;

        CanvasGroup.Toggle(true, 0.25f);
    }

    public void OpenFate()
    {
        using var evt = ToggleFateEditorUIEvent.Get(true).SendGlobal();

        CanvasGroup.Toggle(false, 0.25f);
    }

    public void OpenInventory()
    {
        
    }

    public void OpenShopKeeper()
    {
        using var evt = ToggleShopKeeperUIEvent.Get(true).SendGlobal();
        
        CanvasGroup.Toggle(false, 0.25f);
    }
}
