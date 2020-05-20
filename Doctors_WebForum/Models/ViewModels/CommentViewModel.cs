using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Doctors_WebForum.Models.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string CommentContent { get; set; }
        public Nullable<System.DateTime> CommentDate { get; set; }
        public int Post_ID { get; set; }
        public int Doctor_ID { get; set; }

        public string ImageDoctor { get; set; }
    }
}