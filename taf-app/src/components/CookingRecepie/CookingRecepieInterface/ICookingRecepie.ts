import IIngredient from "./IIngredient";
import IStepInFoodPreparation from "./IStepInFoodPreparation";

interface ICookingRecepie{
    cookingRecepieId: string,
    authorUsername: string,
    authorProfilePicture: string,
    cookingRecepieTitle: string,
    description: string,
    typeOfMeal: string,
    publicationDate: Date,
    preparationTime: number,
    averageRate: number,
    cookingRecepiePicture: string,
    stepsInFoodPreparation: IStepInFoodPreparation[],
    ingredients: IIngredient []
}
export default ICookingRecepie;