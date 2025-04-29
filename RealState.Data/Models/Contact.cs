using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealState.Data.Models
{
   

    public class Contact
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public bool? EmailSend { get; set; }

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

       
    }
}
