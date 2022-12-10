import IAward from "./IAward";

interface PushPopAwardProp{
    pushAwardToArray(award: IAward): void,
    popAwardFromArray(awardIndex: number):void
}
export default PushPopAwardProp;