using System;
using System.Collections.Generic;
using System.Text;

namespace SimplEnteiner.Core.RegistrationService.Factory
{
    internal class RegistryFactory : IRegistryFactory
    {
        public IRegistry CreateRegistry()
        {
            return new Registry();
        }
    }
}
