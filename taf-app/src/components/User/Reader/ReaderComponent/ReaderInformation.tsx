import axios from "axios";
import { Avatar, Box, Container, Grid, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import IReader from "../ReaderInterface/IReader";
import IReaderUsername from "../ReaderInterface/IReaderUsername";

function ReaderInformation(prop: IReaderUsername) {
    const [readerInformation, setReaderInformation] = useState<IReader>();


    useEffect(() => {
        const fetchReader = async () => {
            const result = await axios.get<IReader>(`https://localhost:5001/api/User/GetUserPreview/${prop.username}/Reader`,
                {
                    headers: {
                        'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                    },
                });
            if (result.status === 200) {
                setReaderInformation(result.data);
            }
        }
        fetchReader();
    }, [])


    return (<>
        <Container sx={{ display: "flex", justifyContent: "center", flexDirection: "column" }} >
            <Box sx={{ mt: 1, display: "flex", justifyContent: "center", flexWrap: "wrap" }}>
                <Avatar sx={{ width: 120, height: 120, }}
                    src={readerInformation?.profilePicture != undefined ? `data:image/jpeg;base64,${readerInformation?.profilePicture}` : ""} />
                <Box sx={{ display: "flex", justifyContent: "center", flexDirection: "column", ml: 1 }}>
                    <Typography variant="h4">{readerInformation?.username}</Typography>
                    <Typography variant="h6">{readerInformation?.name} {readerInformation?.lastname}</Typography>
                </Box>
                
            </Box>
            <Box sx={{ display: "flex", justifyItems: "center", flexDirection: "column" }}>
                    <Typography align="center" variant="h5"> {readerInformation?.typeOfStudent ==="Student" ? "Student" : "Srednjškolac"}</Typography>
                    <Typography align="center" variant="h6">Naziv škole - {readerInformation?.nameOfSchool}</Typography>
                </Box>



        </Container>
    </>);
}
export default ReaderInformation;