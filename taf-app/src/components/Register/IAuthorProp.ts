import IAward from "./IAward";

interface IAuthorProp{
    handleBiographyChange :React.ChangeEventHandler<HTMLInputElement>,
    handleYearsOfExpirianceChange :React.ChangeEventHandler<HTMLInputElement>,
    pushAwardToArray(award: IAward): void,
    removeAwardFromArray(awardIndex: number):void,
    awards: IAward[]
}
export default IAuthorProp;