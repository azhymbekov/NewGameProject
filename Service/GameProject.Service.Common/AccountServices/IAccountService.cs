using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.Service.Common.AccountServices
{
    public interface IAccountService
    {
        Task<OperationResult> Login(string userName, string password);

        Task Logout();
    }
}
