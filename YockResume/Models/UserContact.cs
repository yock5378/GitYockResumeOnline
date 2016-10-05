using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace YockResume.Models
{
    public class UserContact
    {

        [DisplayName("ID")]
        [Key]
        public Int32 ID { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "此欄必填")]
        [MaxLength(50, ErrorMessage = "長度不可超過 50個字元")]
        public string Name { get; set; }

        [StringLength(200)]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$", ErrorMessage = "請填入正確的Email格式")]
        [MaxLength(200, ErrorMessage = "長度不可超過 200個字元")]
        public string Email { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^[\d]+(\-[\d]+)*$", ErrorMessage = "請填入正確的電話號碼格式")]
        [MaxLength(20, ErrorMessage = "長度不可超過 20個字元")]
        public string Phone { get; set; }

        [StringLength(500)]
        [MaxLength(500, ErrorMessage = "長度不可超過 500個字元")]
        public string Message { get; set; }

        public DateTime CreateOn { get; set; }

    }
}