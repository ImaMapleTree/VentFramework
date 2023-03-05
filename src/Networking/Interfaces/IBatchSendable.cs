using VentLib.Networking.Batches;

namespace VentLib.Networking.Interfaces;

public interface IBatchSendable<out T> : IBatchSendable where T: IBatchSendable<T>
{
    new BatchEnd Write(BatchWriter batchWriter);

    new T Read(BatchReader batchReader);

    object IBatchSendable.Read(BatchReader batchReader)
    {
        return Read(batchReader);
    }

    BatchEnd IBatchSendable.Write(BatchWriter batchWriter)
    {
        return Write(batchWriter);
    }
}

public interface IBatchSendable
{
    BatchEnd Write(BatchWriter batchWriter);
    
    object Read(BatchReader reader);
}