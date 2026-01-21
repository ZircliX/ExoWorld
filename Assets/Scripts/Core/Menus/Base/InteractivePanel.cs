namespace OverBang.ExoWorld.Core
{
    public abstract class InteractivePanel : NavigablePanel
    {
        protected override void Awake()
        {
            base.Awake();
            SetupNavigation();
        }

        protected virtual void SetupNavigation() { }
    }
}