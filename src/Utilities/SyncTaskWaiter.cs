using System.Threading.Tasks;

namespace VentLib.Utilities;

internal class SyncTaskWaiter<T>
{
    internal readonly Task<T> Response;
    internal bool Finished;

    public SyncTaskWaiter(Task<T> response)
    {
        Response = response;
        Response.ContinueWith(_ => Finished = true);
    }
}