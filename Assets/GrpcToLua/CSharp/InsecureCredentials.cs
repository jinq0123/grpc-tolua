namespace GrpcToLua
{
    // Same as Grpc.Core.InsecureCredentials
    public sealed class InsecureCredentials : Grpc.Core.ChannelCredentials
    {
        public override void InternalPopulateConfiguration(Grpc.Core.ChannelCredentialsConfiguratorBase configurator, object state)
        {
            configurator.SetInsecureCredentials(state);
        }
    }
}
