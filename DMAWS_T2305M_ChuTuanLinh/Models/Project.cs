﻿using DMAWS_T2305M_ChuTuanLinh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMAWS_T2305M_ChuTuanLinh.Models
{
    public class Project
    {
        public int ProjectId { get; set; } // Id thể loại
        public string ProjectName { get; set; } // Tên thể loại
        public DateTime ProjectStartDate { get; set; } // Ngày bắt đầu
        public DateTime? ProjectEndDate { get; set; } // Ngày kết thúc (Nullable)

        public virtual ICollection<ProjectEmployee>? ProjectEmployees { get; set; } // Có thể để null
    }
}
