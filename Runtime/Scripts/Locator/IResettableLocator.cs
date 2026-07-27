namespace HelloDev.Utils.Locator.Locator
{
    // Marker interface so the resetter can find every locator SO
    // without knowing its generic type at compile time.
    public interface IResettableLocator
    {
        void ResetLocator();
    }
}