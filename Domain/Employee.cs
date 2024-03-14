using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Employee
    {        
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(60)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public decimal Salary { get; set; }
    }
}