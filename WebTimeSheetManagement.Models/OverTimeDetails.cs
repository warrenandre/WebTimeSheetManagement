using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTimeSheetManagement.Models
{
    [Table("OverTimeDetails")]
    public class OverTimeDetails
    {
        [Key]
        public int OverTimeID { get; set; }
        public string DaysofWeek { get; set; }
        public int? Hours { get; set; }
        public DateTime? Period { get; set; }
        public int? ProjectID { get; set; }
        public int UserID { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int OverTimeMasterID { get; set; }
       
    }
}
