using Microsoft.AspNetCore.Http;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Services.User.CommonUserMethods
{
    public static class CommonUserMethods
    {
        public static byte[] EncryptPassword(string password, byte[] passwordSalt)
        {
            byte[] hashedPassword;
            byte[] originalPasswordInBytes;
            if (string.IsNullOrEmpty(password) || passwordSalt.Length == 0)
                return hashedPassword = new byte[0];

            originalPasswordInBytes = Encoding.ASCII.GetBytes(password);

            int totalLengthCombined = (originalPasswordInBytes.Length + passwordSalt.Length);

            hashedPassword = new byte[totalLengthCombined];
            int i;
            for (i = 0; i < passwordSalt.Length - 1; i++)
            {
                hashedPassword[i] = passwordSalt[i];
            }
            for (int j = 0; j < originalPasswordInBytes.Length - 1; j++)
            {
                hashedPassword[i] = originalPasswordInBytes[j];
                i++;
            }
            return HashPassword(hashedPassword);
        }

        public static byte[] HashPassword(byte[] password)
        {
            SHA256 _sha256 = SHA256.Create();
            return _sha256.ComputeHash(password);
        }

        public static async Task<bool> CheckIfUserExists(IGraphClient client, string userName)
        {
            var author = (await client.Cypher.Match("(author: Author)")
                                      .Where((TaF_Neo4j.Models.Author author) => author.Username == userName)
                                      .Return(author => author.As<TaF_Neo4j.Models.Author>())
                                      .ResultsAsync).FirstOrDefault();
            if (author != null)
                return true;
            else
            {
                var reader = (await client.Cypher.Match("(reader: Reader)")
                                      .Where((TaF_Neo4j.Models.Reader reader) => reader.Username == userName)
                                      .Return(reader => reader.As<TaF_Neo4j.Models.Reader>())
                                      .ResultsAsync).FirstOrDefault();
                if (reader != null)
                    return true;
                else
                    return false;
            }

        }

        public static string GetFolderPathByUserType(bool author)
        {
            string folderPath = "";
            if (author)
                folderPath = "C:\\Users\\DusanSotirov\\Desktop\\TaF_Pictures\\UserProfilePicture\\Author\\";
            else
                folderPath = "C:\\Users\\DusanSotirov\\Desktop\\TaF_Pictures\\UserProfilePicture\\Reader\\";
            return folderPath;
        }
        public static string WriteImageToFolder(IFormFile ImageFile, string username, bool author)
        {
            byte[] imageBinary = null;
            string folderPath = GetFolderPathByUserType(author);

            string imageFilePath = "";
            if (ImageFile.Length > 0)
            {
                using (var binaryReader = new BinaryReader(ImageFile.OpenReadStream()))
                {
                    imageBinary = binaryReader.ReadBytes((int)ImageFile.Length);
                }
                imageFilePath = folderPath + username;
                File.WriteAllBytes(imageFilePath, imageBinary);
            }
            return imageFilePath;
        }

        public static byte[] ReadImageFromFile(string username, bool author)
        {
            try
            {
                string folderPath = GetFolderPathByUserType(author);
                string imageFilePath = folderPath + username;
                byte[] image = File.ReadAllBytes(imageFilePath);
                if (image.Length > 0)
                    return image;
                else
                    return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                return Array.Empty<byte>();
            }
        }

        public static void DeleteImageFromFolder(string username, bool author)
        {
            string folderPath = GetFolderPathByUserType(author);
            string imageFilePath = folderPath + username;
            File.Delete(imageFilePath);
        }

       
    }
}
