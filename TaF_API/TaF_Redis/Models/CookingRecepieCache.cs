using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Redis.Models
{
    public class CookingRecepieCache
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

    # endregion Fields

        public CookingRecepieCache()
        { }
    }
}
