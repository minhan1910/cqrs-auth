using System.ComponentModel.DataAnnotations;

namespace Common.Requests.Employees
{
    public class UpdateEmployeeRequest
    {
        [Required]
        public int Id { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public decimal Salary { get; set; }
    }
}