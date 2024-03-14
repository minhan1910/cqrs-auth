using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig
{
    public class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder
                .ToTable("Employees", SchemaName.HR)
                .HasIndex(e => e.FirstName)
                .HasDatabaseName("IX_Employee_FirstName");

            builder
                .HasIndex(e => e.LastName)
                .HasDatabaseName("IX_Employee_LastName");
        }
    }
}