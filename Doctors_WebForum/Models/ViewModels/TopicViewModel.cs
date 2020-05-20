using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doctors_WebForum.Models.ViewModels
{
    public class TopicViewModel
    {
        public int Id { get; set; }
        public string TopicName { get; set; }
        public string Description { get; set; }
        public int Specialize_ID { get; set; }
    }
}