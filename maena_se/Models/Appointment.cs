using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace maena_se.Models
{
    public class Appointment
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }
}