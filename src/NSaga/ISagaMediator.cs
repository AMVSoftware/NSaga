namespace NSaga
{
    public interface ISagaMediator
    {
        OperationResult Consume(ISagaMessage sagaMessage);
        OperationResult Consume(IInitiatingSagaMessage initiatingMessage);
    }
}