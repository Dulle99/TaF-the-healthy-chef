interface IUpdateAndDeleteButtonProp{
    typeOfContent: string,
    contentId: string,
    deleteContent(isCookingRecepie: boolean, contentId: string): Promise<void>
}
export default IUpdateAndDeleteButtonProp;