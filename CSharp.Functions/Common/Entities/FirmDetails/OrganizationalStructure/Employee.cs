namespace Common.Entities
{
    public class Employee: CustomerBase
    {
        public string? CustomerId { get; set; }
        public string? CompanyNo { get; set; }
        public string? Title { get; set; }
        public Employee? LineManager { get; set; }
        public Role? PrimaryRole { get; set; }
        public List<Role> OtherRoles { get; set; } = new List<Role>();
        public List<ProductType> ProductType { get; set; } = new();
        public string? EmploymentStatus { get; set; }
        public bool IsRoot { get; set; }
        public string? ImgId { get; set; }
        public List<EmployeeTask> Tasks { get; set; } = new();
        public string OnboardingType { get; set; } = OnboardingTypes.Employee.ToString();
    }
}