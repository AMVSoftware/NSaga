using System;

namespace NSaga
{
    public interface ISagaRepository
    {
        //TSaga InitiateSaga<TSaga>() where TSaga : class;
        //TSaga Find<TSaga>(Guid correlationId) where TSaga : class;
        //void Save<TSaga>(TSaga saga) where TSaga : class;
        //void Conculde<TSaga>(TSaga saga) where TSaga : class;

        dynamic Find(Guid correlationId);


        dynamic InitiateSaga(Type sagaType);
        void Save<TSagaData>(ISaga<TSagaData> saga);
        //void Conculde<TSaga>(TSaga saga) where TSaga : class;

    }
}
