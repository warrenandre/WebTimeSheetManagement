using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTimeSheetManagement.Models
{
    [Table("OverTimeMaster")]
    public class OverTimeMaster
    {
        [Key]
        public int OverTimeMasterID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? TotalHours { get; set; }
        public int UserID { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int OverTimeStatus { get; set; }
        public string Comment { get; set; }
     
    }
}
