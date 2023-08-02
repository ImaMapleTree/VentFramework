namespace VentLib.Logging.Accumulators;

public interface IWrappingAccumulator: ILogAccumulator
{
    public ILogAccumulator[] WrappedAccumulators { get; }
}