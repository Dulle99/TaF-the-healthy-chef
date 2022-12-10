import { Rating, Typography } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";
import IRateContent from "./IRateContent";
import IRateDTO from "./IRateDTO";

function RateContent(props: IRateContent) {
    const [averageRate, setAverageRate] = useState(1);


    const handleRateContent = (event: React.SyntheticEvent<Element, Event>, value: number | null) => {
        if (sessionStorage.getItem('token') != null) {

            if (value != null) {
                console.log(value);
                let rateValue = value != null ? value : 1;
                let typeOfUser = sessionStorage.getItem('typeOfUser') == "Author" ? "Author" : "Reader";
                let Username = sessionStorage.getItem('username');
                let username: string = "";
                if (Username !== null) {
                    username = Username;
                }
                let rate: IRateDTO = { contentId: props.contentId, value: rateValue, typeOfUser: typeOfUser, username: username }
                let url: string = "";
                if (props.isContentCookingRecepie) {
                    url = `https://localhost:5001/api/CookingRecepie/AddRateToTheCookingRecepie`;
                }
                else {
                    url = `https://localhost:5001/api/Blog/AddRateToTheBlog`;
                }
                const AddRate = async () => {
                    const result = await axios.post<number>(url, rate,
                        {
                            headers: {
                                'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                            },
                        });
                    if (result.status === 200) {
                        setAverageRate(result.data);
                    }
                }

                AddRate();
            }
        }

    }

    useEffect(() => {
    }, [averageRate]);

    useEffect(() => {
        setAverageRate(props.averageRate);
    }, []);
    return (
        <>
            <Typography variant="subtitle1" color="text.secondary">Ocena</Typography>
            <Rating
                name="average-rate"
                value={averageRate}
                onChange={handleRateContent}
            />
        </>
    );
}
export default RateContent;