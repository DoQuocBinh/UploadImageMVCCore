using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UploadImage.Models;


namespace UploadImage.Controllers
{
    public class HomeController : Controller
    {
       
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> SaveToDB(IFormFile postedFile)
        {
            TblFile myFile = new TblFile();
            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Files\\");
            bool basePathExists = System.IO.Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);
            var fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
            var filePath = Path.Combine(basePath, postedFile.FileName);
            var extension = Path.GetExtension(postedFile.FileName);
            using (var dataStream = new MemoryStream())
            {
                await postedFile.CopyToAsync(dataStream);
                myFile.Data = dataStream.ToArray();
            }
            myFile.ContentType = extension;
            myFile.Name = fileName;
            ImageStoreContext db = new ImageStoreContext();
            db.TblFiles.Add(myFile);
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        public IActionResult Display()
        {
            ImageStoreContext db = new ImageStoreContext();
            return View(db.TblFiles.ToList());
        }
        public IActionResult Upload()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
