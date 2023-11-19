using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract;

public interface IAuthService
{
    Task<IResult> Login(string userName,string password);
    Task<IDataResult<AccessToken>> CreateAccessToken(string userName);
}
