using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjRentalManagement.Helpers
{
    public static class BuildingImageHelper
    {
        static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        const int MaxBytes = 5 * 1024 * 1024;

        /// <summary>Saves upload under ~/Images/Buildings/. Returns relative URL (~/Images/Buildings/...) or null if no file.</summary>
        public static string TrySaveBuildingImage(HttpPostedFileBase file, ModelStateDictionary modelState, string fieldName = "photoFile")
        {
            if (file == null || file.ContentLength == 0)
                return null;

            if (file.ContentLength > MaxBytes)
            {
                modelState.AddModelError(fieldName, "Image must be 5 MB or smaller.");
                return null;
            }

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext))
            {
                modelState.AddModelError(fieldName, "Allowed image types: JPG, PNG, GIF, WebP.");
                return null;
            }

            var folder = HttpContext.Current.Server.MapPath("~/Images/Buildings");
            Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid().ToString("N") + ext;
            var physical = Path.Combine(folder, fileName);
            file.SaveAs(physical);
            return "~/Images/Buildings/" + fileName;
        }

        public static void DeleteIfExists(string imagePathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(imagePathOrUrl) || !imagePathOrUrl.StartsWith("~/", StringComparison.Ordinal))
                return;
            try
            {
                var physical = HttpContext.Current.Server.MapPath(imagePathOrUrl);
                if (File.Exists(physical))
                    File.Delete(physical);
            }
            catch
            {
                /* ignore cleanup failures */
            }
        }
    }
}
