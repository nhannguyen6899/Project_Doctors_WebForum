using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doctors_WebForum.Models.ViewModels
{
    public class SpecializeViewModel
    {
        public int Id { get; set; }
        public string SpecializeName { get; set; }
        public string Description { get; set; }

        public string TopicName { get; set; }
        public string ToppicDescription { get; set; }
    }
}