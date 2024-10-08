using DMAWS_T2305M_ChuTuanLinh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMAWS_T2305M_ChuTuanLinh.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; } // Id nhân viên
        public string EmployeeName { get; set; } // Tên nhân viên
        public DateTime EmployeeDOB { get; set; } // Ngày tháng năm sinh
        public string EmployeeDepartment { get; set; } // Bộ phận

        public virtual ICollection<ProjectEmployee>? ProjectEmployees { get; set; } // Có thể để null
    }
}
