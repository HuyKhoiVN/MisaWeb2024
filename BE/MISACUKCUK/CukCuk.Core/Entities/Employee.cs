﻿using CukCuk.Core.Enum;
using CukCuk.Core.MISAAttribute;
using CukCuk.Core.MISAResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Entities
{
    public class Employee
    {
        public Employee()
        {
            
        }

        public Guid EmployeeId { get; set; }

        [NotEmpty]
        [PropertyName("Mã nhân viên")]
        public string EmployeeCode { get; set; }
        
        [NotEmpty]
        [PropertyName("Tên nhân viên")]
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
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

        [NotMap]
        public string GenderName
        {
            get
            {
                switch (Gender)
                {
                    case Enum.Gender.Female:
                        return ResourceVN.Gender_Female;
                    case Enum.Gender.Male:
                        return ResourceVN.Gender_Male;
                    default:
                        return ResourceVN.Gender_Other;
                }
            }
        }
    }
}
