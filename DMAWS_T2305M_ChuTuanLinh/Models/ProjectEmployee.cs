using DMAWS_T2305M_ChuTuanLinh.Models;
using System.ComponentModel.DataAnnotations;

namespace DMAWS_T2305M_ChuTuanLinh.Models
{
    public class ProjectEmployee
    {
        public int EmployeeId { get; set; } // Id nhân viên
        public int ProjectId { get; set; } // Id dự án

        [Required]
        public string Tasks { get; set; } // Nhiệm vụ trong dự án

        public virtual Employee Employees { get; set; } // Nhân viên
        public virtual Project Projects { get; set; } // Dự án
    }
}
