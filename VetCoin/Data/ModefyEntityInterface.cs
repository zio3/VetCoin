using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    interface ICreate
    {

        string CreateUser { get; set; }
        DateTimeOffset CreateDate { get; set; }


    }
    interface IUpdate
    {
        string UpdateUser { get; set; }
        DateTimeOffset UpdateDate { get; set; }
    }


    //interface ILogging
    //{
    //    string GetOrigenKey();
    //}
}
