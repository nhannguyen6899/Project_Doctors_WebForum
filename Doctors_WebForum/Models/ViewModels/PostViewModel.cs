using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doctors_WebForum.Models.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string PostName { get; set; }

        [AllowHtml]
        public string PostContent { get; set; }

        public string Description { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Views { get; set; }
        public bool StatusBlock { get; set; }
        public int Topic_ID { get; set; }
        public int Doctor_ID { get; set; }

        public string ImageDoctor { get; set; }
        public string DoctorName { get; set; }
    }
}