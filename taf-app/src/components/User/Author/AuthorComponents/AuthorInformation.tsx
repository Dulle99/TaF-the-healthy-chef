import { Avatar, Box, Button, Grid, Link, Typography } from "@mui/material";
import { Container } from "@mui/system";
import axios from "axios";
import { useEffect, useState } from "react";
import FormatDate from "../../../ContentMutalComponents/FormatDateFunction";
import IAuthor from "../AuthorInterface/IAuthor";
import IAuthorUsername from "../AuthorInterface/IAuthorUsername";
import { FaAward } from "react-icons/fa";
import { BsFillPencilFill } from "react-icons/bs";

function AuthorInformation(prop: IAuthorUsername) {
    const [authorInformation, setAuthorInformation] = useState<IAuthor>();


    useEffect(() => {
        const fetchAuthor = async () => {
            const result = await axios.get<IAuthor>(`https://localhost:5001/api/User/GetUserPreview/${prop.username}/Author`,
                {
                    headers: {
                        'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                    },
                });
            if (result.status === 200) {
                setAuthorInformation(result.data);
            }
        }

        fetchAuthor();
    }, [])
    return (
        <>
            <Container >
                <Box sx={{ mt: 1, display: "flex", justifyContent: "left", flexWrap: "wrap" }}>
                    <Avatar sx={{ width: 120, height: 120, }}
                        src={authorInformation?.profilePicture != undefined ? `data:image/jpeg;base64,${authorInformation?.profilePicture}` : ""} />
                    <Box sx={{ display: "flex", justifyContent: "center", flexDirection: "column", ml: 1 }}>
                        <Typography variant="h4">{authorInformation?.username}</Typography>
                        <Typography>{authorInformation?.name} {authorInformation?.lastname}</Typography>
                    </Box>
                    
                    
                </Box>
                <Box sx={{ display: "flex",flexDirection: "column", mt:1}}>
                    <Typography >Godine iskustva {authorInformation?.yearsOfExpiriance}</Typography>
                    <Typography >Biografija</Typography>
                    <Box maxWidth="1000px" sx={{ paddingLeft: "25px" }}>
                        <Typography>{authorInformation?.biography}</Typography>
                    </Box>
                    <Typography sx={{ mt: 1 }}>Nagrade:</Typography>
                    {authorInformation?.awards.map((award, ind) => (
                        <Box maxWidth="400px" sx={{ display: "flex", paddingLeft: "25px", padding:"5px" }} key={award.awardId}>
                            <FaAward size={25}/>
                            <Typography paddingLeft={1} > {award.awardName + " - " + FormatDate(award.dateOfReceivingAward) } 
                            </Typography>
                        </Box>))}
                </Box>


            </Container>
        </>
    );
}
export default AuthorInformation;