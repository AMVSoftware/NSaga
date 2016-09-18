namespace NSaga
{
    public interface ISagaMediator
    {
        OperationResult Consume(IInitiatingSagaMessage initiatingMessage);
        OperationResult Consume(ISagaMessage sagaMessage);
    }
}