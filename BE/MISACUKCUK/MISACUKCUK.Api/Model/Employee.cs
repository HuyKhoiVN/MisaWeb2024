namespace MISACUKCUK.Api.Model
{
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityNumber { get; set; }
        public string Address { get; set; }
        public DateTime? IndentityDate { get; set; }
        public string IdentityPlace { get; set; }
        public string LanelineNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankNumber { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string GenderName
        {
            get
            {
                switch (Gender)
                {
                    case 0:
                        return "Nữ";
                    case 1:
                        return "Name";
                    default:
                        return "Không xác định";                
                }
            }
        }
    }
}
