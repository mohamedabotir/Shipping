namespace Common.Utils;

public interface IServiceProviderFactory
{
    IServiceProvider CreateScope();
}