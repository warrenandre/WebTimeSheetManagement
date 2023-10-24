using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTimeSheetManagement.Models
{
    public class OverTimeExportModel
    {
        public int OverTimeMasterID { get; set; }
        public int TotalHours { get; set; }
        public string Name { get; set; }
    }

    public class GetOverTimePeriods
    {
        public string Period { get; set; }
    }

    public class GetOverTimeProjectNames
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
    }

    public class GetOverTimeHours
    {
        public int Hours { get; set; }
    }

    public class OverTimeExcelExportModel
    {
        [Display(Name = "OverTime From Date")]
        [Required(ErrorMessage = "Please Choose From Date")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "OverTime To Date")]
        [Required(ErrorMessage = "Please Choose To Date")]
        public DateTime? ToDate { get; set; }
    }

    public class ExpenseOverTimeExcelExportModel
    {
        [Display(Name = "Expense From Date")]
        [Required(ErrorMessage = "Please Choose From Date")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "Expense To Date")]
        [Required(ErrorMessage = "Please Choose To Date")]
        public DateTime? ToDate { get; set; }
    }

    public class OverTimeExportUserModel
    {
        [Display(Name = "OverTime From Date")]
        [Required(ErrorMessage = "Please Choose From Date")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "OverTime To Date")]
        [Required(ErrorMessage = "Please Choose To Date")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Employee Name")]
        [Required(ErrorMessage = "Please Select Employee Name")]
        public int RegistrationID { get; set; }
    }

}
