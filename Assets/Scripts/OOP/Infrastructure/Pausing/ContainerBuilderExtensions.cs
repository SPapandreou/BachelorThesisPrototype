using VContainer;
using VContainer.Unity;

namespace OOP.Infrastructure.Pausing
{
    public static class ContainerBuilderExtensions
    {
        public static IContainerBuilder InstallPausing(this IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<PausableTickController>();
            
            return builder;
        }
    }
}