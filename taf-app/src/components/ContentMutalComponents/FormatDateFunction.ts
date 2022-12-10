function FormatDate(date: Date): string {

    let newdate: Date = new Date(date);
    let formatedDate: string = newdate.getDate().toString() + "." + (newdate.getMonth() + 1).toString() + "." + newdate.getFullYear().toString() + ".";
    return formatedDate;
}
export default FormatDate;