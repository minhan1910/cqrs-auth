using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        [StringLength(256)]
        public string Descrption { get; set; }
        public string Group { get; set; }
    }
}