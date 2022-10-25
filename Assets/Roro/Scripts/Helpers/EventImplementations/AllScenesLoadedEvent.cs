using Events;

namespace Based2.EventImplementations
{
	public class AllScenesLoadedEvent : Event<AllScenesLoadedEvent>
	{

		public static AllScenesLoadedEvent Get()
		{
			var evt = GetPooledInternal();
			return evt;
		}
	}
}
