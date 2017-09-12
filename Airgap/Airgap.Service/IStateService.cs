using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Service
{
    public interface IStateService
    {
        List<State> GetAllState();

        State GetStateById(long id);

        State GetStateByNameOrCode(string name);

        State GetStateByCode(string name);
    }
}
