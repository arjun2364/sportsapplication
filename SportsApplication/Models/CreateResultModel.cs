using Microsoft.AspNetCore.Identity;
using Sports_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsApplication.Models
{
    public class CreateResultModel: Result
    {
        public List<Athelete> AtheleteList;
    }
}
