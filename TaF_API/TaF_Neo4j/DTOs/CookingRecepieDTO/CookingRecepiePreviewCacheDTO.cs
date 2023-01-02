using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.CookingRecepieDTO
{
    public class CookingRecepiePreviewCacheDTO
    {
        #region Fields

        public string AuthorUsername { get; set; }

        public string AuthorProfilePicture_FilePath { get; set; }

        public Guid CookingRecepieId { get; set; }

        public string CookingRecepieTitle { get; set; }

        public string Description { get; set; }

        public string TypeOfMeal { get; set; }

        public DateTime PublicationDate { get; set; }

        public int PreparationTime { get; set; }

        public float AverageRate { get; set; }

        public string CookingRecepiePicture_FilePath { get; set; }

        #endregion Fields

        #region Constructor

        public CookingRecepiePreviewCacheDTO() { }

        public CookingRecepiePreviewCacheDTO(string authorUsername, string profilePicture_FilePath, Guid cookingRecepieId, string cookingRecepieTitle, string description, string typeOfMeal,
                                        DateTime publicatioDate, int preparationTime, float averageRate, string cookingRecepiePicture_FilePath)
        {
            this.AuthorUsername = authorUsername;
            this.AuthorProfilePicture_FilePath = profilePicture_FilePath;
            this.CookingRecepieId = cookingRecepieId;
            this.CookingRecepieTitle = cookingRecepieTitle;
            this.Description = description;
            this.TypeOfMeal = typeOfMeal;
            this.PublicationDate = publicatioDate;
            this.PreparationTime = preparationTime;
            this.AverageRate = averageRate;
            this.CookingRecepiePicture_FilePath = cookingRecepiePicture_FilePath;
        }

        #endregion Constructor
    }
}
