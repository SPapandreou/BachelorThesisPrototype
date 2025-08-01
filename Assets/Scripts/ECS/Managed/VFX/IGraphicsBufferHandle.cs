namespace ECS.Managed.VFX
{
    public interface IGraphicsBufferHandle
    {
        public void Unlock();
        public void Upload();
    }
}