import IAuthorAward from "./IAuthorAward";

interface IAuthor{
    username: string,
    name: string,
    lastname: string,
    biography: string,
    awards: IAuthorAward[],
    yearsOfExpiriance: number,
    profilePicture: string
}
export default IAuthor;