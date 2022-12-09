using Microsoft.AspNetCore.Http;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Services.MutalMethodsForContent
{
    public static class MutalMethodsForContent
    {
        public static string GetFolderPathByContentType(bool cookingRecipe)
        {
            string folderPath = "";
            if (cookingRecipe)
                folderPath = "C:\\Users\\DusanSotirov\\Desktop\\TaF_Pictures\\CookingRecepiesPictures\\";
            else
                folderPath = "C:\\Users\\DusanSotirov\\Desktop\\TaF_Pictures\\BlogPictures\\";
            return folderPath;
        }
        public static string WriteImageToFolder(IFormFile ImageFile, Guid contentId, bool cookingRecipe)
        {
            byte[] imageBinary = null;
            string folderPath = GetFolderPathByContentType(cookingRecipe);

            string imageFilePath = "";
            if (ImageFile.Length > 0)
            {
                using (var binaryReader = new BinaryReader(ImageFile.OpenReadStream()))
                {
                    imageBinary = binaryReader.ReadBytes((int)ImageFile.Length);
                }
                imageFilePath = folderPath + contentId.ToString();
                File.WriteAllBytes(imageFilePath, imageBinary);
            }
            return imageFilePath;
        }

        public static byte[] ReadImageFromFile(Guid contentId, bool cookingRecipe)
        {
            try
            {
                string folderPath = GetFolderPathByContentType(cookingRecipe);
                string imageFilePath = folderPath + contentId.ToString();
                var image = File.ReadAllBytes(imageFilePath);
                return image;
            }
            catch(Exception ex)
            {
                return File.ReadAllBytes("C:\\Users\\DusanSotirov\\Desktop\\TaF_Pictures\\DefaultImages\\default.jpg");
            }
        }

        public static void DeleteImageFromFolder(Guid contentId, bool cookingRecipe)
        {
            string folderPath = GetFolderPathByContentType(cookingRecipe);
            string imageFilePath = folderPath + contentId.ToString();
            File.Delete(imageFilePath);
        }

        public static async Task UpdateUserRate(IGraphClient client, Guid rateId, float newRateValue)
        {
            await client.Cypher.Match("(r: Rate)")
                               .Where((Models.Rate r) => r.RateId == rateId)
                               .Set("r.RateValue = $newRate")
                               .WithParam("newRate", newRateValue).ExecuteWithoutResultsAsync();
        }
    }
}
