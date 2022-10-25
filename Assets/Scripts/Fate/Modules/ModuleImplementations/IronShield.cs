using CharImplementations;

namespace Fate.Modules
{
    public class IronShield : Module
    {
        public override void OnActiveSkill(ModuleRuntimeData runtimeData)
        {
            PlayerExtensions.GetPlayer().GetImmune(runtimeData.GetDurationData());
        }
    }
}