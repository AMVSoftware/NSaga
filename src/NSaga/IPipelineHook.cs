using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSaga
{
    public interface IMediatorPipelineHook
    {
        void BeforeInitialisation(IInitiatingSagaMessage message);
        void AfterInitialisation(IInitiatingSagaMessage message);
        void BeforeConsuming(ISagaMessage message);
        void AfterConsuming(ISagaMessage message);
    }
}
