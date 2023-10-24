using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTimeSheetManagement.Models
{
    [Table("OverTimeAuditTB")]
    public class OverTimeAuditTB
    {
        [Key]
        public int ApprovalOverTimeLogID { get; set; }
        public int ApprovalUser { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Comment { get; set; }
        public int Status { get; set; }
        public int OverTimeID { get; set; }
        public int UserID { get; set; }
    }

    



}
