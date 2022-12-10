interface IBasicHeaderInformation{
    contentId: string,
    authorUsername: string,
    authorProfilePicture: string,
    contentTitle: string,
    description: string,
    publicationDate: Date,
    timeForReadingOrPrepration: number,
    averageRate: number,
    contentPicture: string
    isContentCookingRecepie: boolean
    typeOfMeal: string
}
export default IBasicHeaderInformation;