namespace FirstResponse
{
    public readonly struct ScreenRequested
    {
        public readonly ScreenId Screen;

        public ScreenRequested(ScreenId screen)
        {
            Screen = screen;
        }
    }
}
