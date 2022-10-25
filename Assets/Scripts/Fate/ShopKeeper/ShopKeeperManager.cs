using Events;
using Fate.EventImplementations;
using Fate.ShopKeeper.EventImplementations;
using UnityEngine;
using UnityEngine.UI;

namespace Fate.ShopKeeper
{
    public class ShopKeeperManager : MonoBehaviour
    {
        public ModuleShopController Controller;

        public Button ExitButton;

        private void OnEnable()
        {
            if (!Controller.FillInventory())
            {
                GEM.AddListener<ModulesLoadedEvent>(OnModulesLoaded);
            }
            GEM.AddListener<ToggleShopKeeperUIEvent>(OnToggleShopKeeperUI);

            // TODO: might move to another place
            ExitButton.onClick.AddListener(OnExit);
        }

        private void OnModulesLoaded(ModulesLoadedEvent evt)
        {
            Controller.FillInventory();
        }

        private void OnToggleShopKeeperUI(ToggleShopKeeperUIEvent evt)
        {
            Controller.ShopInventory.ToggleInventoryUI(evt.Visible);
        }

        private void OnExit()
        {
            using var evt = ToggleShopKeeperUIEvent.Get(false).SendGlobal();
        }

        private void OnDisable()
        {
            GEM.RemoveListener<ModulesLoadedEvent>(OnModulesLoaded);
            GEM.RemoveListener<ToggleShopKeeperUIEvent>(OnToggleShopKeeperUI);
            ExitButton.onClick.RemoveAllListeners();
        }
    }
}