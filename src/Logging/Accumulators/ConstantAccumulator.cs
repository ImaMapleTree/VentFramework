namespace VentLib.Logging.Accumulators;

public class ConstantAccumulator: ILogAccumulator
{
    private string text;
    
    public ConstantAccumulator(string text = "")
    {
        this.text = text;
    }
    
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        return composite.Append(text);
    }
}