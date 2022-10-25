using Events;
using Fate.ShopKeeper.EventImplementations;
using GameStages.Hub;
using UnityEngine;

namespace Fate.ShopKeeper
{
    public class ShopKeeperInteractionPoint : InteractionPoint
    {
        private void OnEnable()
        {
            GEM.AddListener<ToggleShopKeeperUIEvent>(OnShopKeeperToggle);
        }

        public override void OnTriggerEnter(Collider other)
        {
            if(Entered)
                return;

            using var evt = ToggleShopKeeperUIEvent.Get(true).SendGlobal();
        }

        public override void OnTriggerExit(Collider other)
        {
            if(!Entered)
                return;

            using var evt = ToggleShopKeeperUIEvent.Get(false).SendGlobal();
        }
        
        private void OnShopKeeperToggle(ToggleShopKeeperUIEvent evt)
        {
            Entered = evt.Visible;
        }
        
        private void OnDisable()
        {
            GEM.RemoveListener<ToggleShopKeeperUIEvent>(OnShopKeeperToggle);
        }
    }
}