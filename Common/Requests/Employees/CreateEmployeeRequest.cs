﻿using System.ComponentModel.DataAnnotations;

namespace Common.Requests.Employees
{
    public class CreateEmployeeRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public decimal Salary { get; set; }
    }
}