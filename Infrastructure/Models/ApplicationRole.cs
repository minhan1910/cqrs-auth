using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class ApplicationRole : IdentityRole
    {
        [StringLength(256)]
        public string Description { get; set; }

    }
}
