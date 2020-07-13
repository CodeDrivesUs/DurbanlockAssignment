using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace DurbanlockAssignment.Models
{
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "StudentNum")]
        [RegularExpression(@"^(\d{8})$", ErrorMessage = "Enter a valid Student Number")]
        [StringLength(maximumLength: 8, ErrorMessage = "Student number must be 8 digits long", MinimumLength = 8)]
        public string StudentNum { get; set; }
        [JsonProperty(PropertyName = "FirstName")]
        [Required(ErrorMessage = "Please enter your first name.")]
        [DisplayName("First Name")]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "LastName")]
        [Required(ErrorMessage = "Please enter your last name.")]
        [DisplayName("Last Name")]
        [MaxLength(50)]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "EmailAddress")]
        [Required(ErrorMessage = "Please enter your e-mail address")]
        [DisplayName("Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(60)]
        public string EmailAddress { get; set; }
        [JsonProperty(PropertyName = "mobile")]
        [MinLength(10),MaxLength(10)]
        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(pattern: @"^\(?([0]{1})\)?[-. ]?([1-9]{1})[-. ]?([0-9]{8})$", ErrorMessage = "Entered Mobile Number Format Is Not Valid.")]
        [StringLength(maximumLength: 10, ErrorMessage = "SA Contact Number must be exactly 10 digits long", MinimumLength = 10)]
        [DisplayName("Mobile Number")]
        public string PhoneNumber { get; set; }
        [DisplayName("Telephone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(pattern: @"^\(?([0]{1})\)?[-. ]?([1-9]{1})[-. ]?([0-9]{8})$", ErrorMessage = "Entered Mobile Number Format Is Not Valid.")]
        [StringLength(maximumLength: 10, ErrorMessage = "Your Telephone Number must be 9 digit long.", MinimumLength = 10)]
        [JsonProperty(PropertyName = "TelePhone")]
        
        public string Telephone { get; set; }
        [DisplayName("Image")]
        [DataType(DataType.ImageUrl)]
        public string StudentImage { get; set; }
        [DisplayName("Are You Active? ")]
        [JsonProperty(PropertyName = "Is Active")]
        public bool isActive { get; set; }
    }

    public class gmail
    {
        public string To { get; set; }
        public string from { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public Byte[] Attachment { get; set; }

        public void sendmail()
        {


            MailMessage mc = new MailMessage("Maxwellzweli@gmail.com", To);
            mc.Subject = Subject;
            mc.Body = Body;
            MemoryStream ms = new MemoryStream(Attachment);


            mc.Attachments.Add(new Attachment(ms, "Student.xls"));
            mc.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            NetworkCredential nc = new NetworkCredential("Maxwellzweli@gmail.com", "Zweli@nkuna2");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(mc);
        }

    }
}