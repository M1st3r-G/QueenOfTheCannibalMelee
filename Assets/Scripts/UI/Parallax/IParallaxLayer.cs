namespace UI.Parallax
{
    public interface IParallaxLayer
    {
        public void FixOrder(int layer);
        public void Move(float delta);
    }
}