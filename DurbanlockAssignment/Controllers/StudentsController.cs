using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DurbanlockAssignment.Models;
using OfficeOpenXml;
using System.Drawing;

namespace StudentAdmission.Controllers
{
    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
        
            var items = await DocumentDBRepository<Item>.GetItemesAsync();
            return View(items);
        }

        [ActionName("search")]
        public async Task<ActionResult> SearchAsync(string search , string SearchBy)
        {
            var items = await DocumentDBRepository<Item>.SearchAsync(search);
            return View("Index",items);
        }
        [ActionName("UnActive")]

        public async Task<ActionResult> UnActiveAsync()
        {
            var items = await DocumentDBRepository<Item>.GetItemsAsync(d => !d.isActive);
            return View("Index",items);
        }
        [ActionName("Active")]
        public async Task<ActionResult> ActiveAsync()
        {
            var items = await DocumentDBRepository<Item>.GetItemsAsync(d => d.isActive);
            return View("Index",items);
        }

#pragma warning disable 1998
        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync()
        {
            return View();
        }
#pragma warning restore 1998
         string result="";
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "Id,StudentNum,FirstName,LastName,EmailAddress,PhoneNumber,Telephone,isActive")] Item item, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
              //await  Checkuser(item.StudentNum);
              // if (result=="true")
              //  {
                    BlobManager BlobManagerObj = new BlobManager("picture");
                    item.StudentImage = BlobManagerObj.UploadFile(upload);
                    await DocumentDBRepository<Item>.CreateItemAsync(item);
                    return RedirectToAction("Index");
                }
                else if (result == "true")
                {
                    ViewBag.status = "Student Already Exist";
                    return View(item);
                }

              
           // }

            return View(item);
        }
       public async Task  Checkuser(string stud)
        {
            var items = await DocumentDBRepository<Item>.Search2Async(stud);
             var item =  items.ToList();
            if (item == null)
            {
                result = "true";
            }
            else
            {
                result = "false";
            }
        }
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind(Include = "Id,StudentNum,FirstName,LastName,EmailAddress,PhoneNumber,Telephone,isActive")] Item item)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Item>.UpdateItemAsync(item.StudentNum, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind(Include = "Id")] string id)
        {
            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
           if(item.StudentImage!=null)
            {
                BlobManager BlobManagerObj = new BlobManager("picture");
                BlobManagerObj.DeleteBlob(item.StudentImage);
            }
            await DocumentDBRepository<Item>.DeleteItemAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            return View(item);
        }
        [ActionName("Email")]
        public async Task<ActionResult> EmailAsync(string id)
        {
            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            exporttoexcel(item);
            return RedirectToAction("Index");
        }

        public void exporttoexcel(Item item)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("report");

            ws.Cells["a1"].Value = "studentno";
            ws.Cells["B1"].Value = "Name";
            ws.Cells["C1"].Value = "Surname";
            ws.Cells["D1"].Value = "Email";
            ws.Cells["E1"].Value = "Phone Number";
            ws.Cells["F1"].Value = "Telephone";
            ws.Cells["G1"].Value = "Is ACtive";



            ws.Row(2).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Row(2).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("pink")));


            int rowStart = 2;

            ws.Cells[string.Format("A{0}", rowStart)].Value = item.StudentNum;
            ws.Cells[string.Format("B{0}", rowStart)].Value = item.FirstName;
            ws.Cells[string.Format("C{0}", rowStart)].Value = item.LastName;
            ws.Cells[string.Format("D{0}", rowStart)].Value = item.EmailAddress;
            ws.Cells[string.Format("E{0}", rowStart)].Value = item.PhoneNumber;
            ws.Cells[string.Format("F{0}", rowStart)].Value = item.Telephone;
            ws.Cells[string.Format("G{0}", rowStart)].Value = item.isActive.ToString();



            ws.Cells["A:AZ"].AutoFitColumns();
            Byte[] filea = pck.GetAsByteArray();
            gmail mail = new gmail();
            mail.To = item.EmailAddress;
            mail.Attachment = filea;
            mail.Subject = "ProjectX21 Student Information";
            mail.Body = "Dear " + item.FirstName + " " + item.LastName + " " + "We are happy to inform you that you are a registered student on our School  ";
            mail.sendmail();
        }
    }
}
