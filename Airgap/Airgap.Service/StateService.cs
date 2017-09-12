using System;
using System.Collections.Generic;
using System.Text;
using Airgap.Entity.Entities;
using Airgap.Entity;
using System.Linq;

namespace Airgap.Service
{
    public class StateService : IStateService
    {
        private IRepository<State> repoState;

        public StateService(IRepository<State> repoState)
        {
            this.repoState = repoState;
        }

        public List<State> GetAllState()
        {
            return repoState.GetAll().ToList();
        }

        public State GetStateById(long id)
        {
            return repoState.Find(x => x.Id == id);
        }

        public State GetStateByNameOrCode(string name)
        {
            return repoState.Find(x => x.Name.ToLower() == name.ToLower() || x.Code.ToLower() == name.ToLower());
        }

        public State GetStateByCode(string Code)
        {
            return repoState.Find(x => x.Code.ToLower() == Code.ToLower());
        }
    }
}
