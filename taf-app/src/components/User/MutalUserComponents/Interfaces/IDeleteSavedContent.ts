interface IDeleteSavedContent{
    typeOfContent: string,
    contentId: string,
    removeContentFromReadLater(isCookingRecepie: boolean, contentId: string): Promise<void>
}
export default IDeleteSavedContent;