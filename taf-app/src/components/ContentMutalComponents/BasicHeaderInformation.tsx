import { ModeComment } from "@mui/icons-material";
import { Avatar, Box, Button, CardMedia, Chip, Container, createTheme, Grid, Rating, ThemeProvider, Typography } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import ResponsiveTitleTheme from "../ResponsiveTitleTheme";
import IBasicHeaderInformation from "./IBasicHeaderInformation";
import RateContent from "./RateContent";

function BasicHeaderInformation(props: IBasicHeaderInformation) {
    const [publicationDate, setPublicationDate] = useState("");
    const [authorUsernameClicked, setAuthorUsernameClickedFlag] = useState(false);
    const [displayAddToReadLaterButton, setDisplayAddToReadLater] = useState(true);
    const [savedToReadLater, setSavedToReadLaterFlag] = useState(false);

    async function IsContentAlreadySavedToReadLater() {
        let url: string = "";
        let typeOfUser = sessionStorage.getItem('typeOfUser') === "Author" ? "Author" : "Reader";
        if (props.isContentCookingRecepie) {
            url = `https://localhost:5001/api/User/CheckIfCookingRecepieIsSavedToReadLater/${sessionStorage.getItem('username')}/${typeOfUser}/${props.contentId}`
        }
        else {
            url = `https://localhost:5001/api/User/CheckBlogIsSavedToReadLater/${sessionStorage.getItem('username')}/${typeOfUser}/${props.contentId}`
        }

        const result = await axios.get<Boolean>(url,
            {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            });

        if (result.data === true) {
            setSavedToReadLaterFlag(true);
        }
        else {
            setSavedToReadLaterFlag(false);
        }
    }

    async function SaveToReadLater() {
        if (sessionStorage.getItem('token') != null) {
            let url: string = "";
            let typeOfUser = sessionStorage.getItem('typeOfUser') === "Author" ? "Author" : "Reader";
            if (props.isContentCookingRecepie) {
                url = `https://localhost:5001/api/User/AddCookingRecepieToReadLater/${props.contentId}/${sessionStorage.getItem('username')}/${typeOfUser}`;
            }
            else {
                url = `https://localhost:5001/api/User/AddBlogToReadLater/${props.contentId}/${sessionStorage.getItem('username')}/${typeOfUser}`;
            }

            const result = await axios.post(url, {},
                {
                    headers: {
                        'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                    },
                });

            if (result.status === 200) {
                setSavedToReadLaterFlag(true);
            }
        }

    }

    const handleAddToReadLater: React.FormEventHandler<HTMLButtonElement> = (e) => {
        if (savedToReadLater === false) {
            SaveToReadLater();
        }
    }

    const handleClickUserName: React.MouseEventHandler<HTMLButtonElement> = (e) => {
        if (sessionStorage.getItem('token') != null) {
            setAuthorUsernameClickedFlag(true);
        }
    }

    useEffect(() => {
        let date: Date = new Date(props.publicationDate);
        let formatedDate: string = date.getDate().toString() + "." + (date.getMonth() + 1).toString() + "." + date.getFullYear().toString() + ".";

        setPublicationDate(formatedDate);
    }, [])

    useEffect(() => {
        if (sessionStorage.getItem('token') === null) {
            setDisplayAddToReadLater(false);
        }
        else if (props.authorUsername === sessionStorage.getItem('username')) {
            setDisplayAddToReadLater(false);
        }
        else {
            setDisplayAddToReadLater(true);
            IsContentAlreadySavedToReadLater();

        }
    }, [])
    return (
        <>
            {authorUsernameClicked ? <Navigate to={props.authorUsername === sessionStorage.getItem('username') ? "/authorProfile" : "/author"}
                                                   state={{ username: props.authorUsername }} /> : ""}
            <Container style={{ display: "flex", justifyContent: "space-between", padding: "50px", flexWrap: "wrap" }}>
                <Box sx={{ display: 'flex', flexDirection: 'row', alignItems: "flex-start" }} >
                    <Avatar src={`data:image/jpeg;base64,${props.authorProfilePicture}`} />
                    <Button variant='text' value={props.authorUsername} onClick={handleClickUserName}   >
                        <Typography color='#92279A'>{props.authorUsername}</Typography>
                    </Button>

                </Box>
                <Box>
                    <ThemeProvider theme={ResponsiveTitleTheme()}>
                    <Typography variant="h4" style={{ textAlign: 'center' }}>
                        {props.contentTitle}
                    </Typography>
                    </ThemeProvider>
                </Box>
                <Box sx={{display:"flex", justifyContent:"right"}}  >
                    <Grid style={{ alignContent: "start" }}>
                        <Typography variant="subtitle1" color="text.secondary">
                            {publicationDate}
                        </Typography>
                        {props.isContentCookingRecepie ?
                            <Chip label={props.typeOfMeal} color='primary' style={{ backgroundColor: '#92279A' }} /> : ""}

                        {props.isContentCookingRecepie ?
                            <Typography variant="subtitle1" color="text.secondary" >
                                Vreme spremanja {props.timeForReadingOrPrepration} min
                            </Typography>
                            :
                            <Typography variant="subtitle1" color="text.secondary" >
                                Vreme citanja {props.timeForReadingOrPrepration} min
                            </Typography>}
                        <Box>
                            <RateContent contentId={props.contentId} averageRate={props.averageRate} isContentCookingRecepie={props.isContentCookingRecepie} />
                        </Box>

                        {displayAddToReadLaterButton ? (
                            <Box>
                                <Button variant="contained"
                                    sx={{
                                        mb: 2, background: '#2f9a27', ':hover': {
                                            bgcolor: '#27699A',
                                            color: 'FFFFFF',
                                        },
                                    }}
                                    onClick={handleAddToReadLater}
                                >
                                    {savedToReadLater ? "Sacuvano" : "Sacuvaj"}
                                </Button>
                            </Box>) : ""}
                    </Grid>
                </Box>

            </Container>
            <Container>
                <Box sx={{ display: "flex", justifyContent: "center" }}>
                    <Box
                        component="img"
                        sx={{
                            height: 433,
                            width: 850,
                            maxHeight: { xs: 633, md: 467 },
                            maxWidth: { xs: 850, md: 650 },
                        }}

                        src={`data:image/jpeg;base64,${props.contentPicture}`}
                    />
                </Box>

                <Box >
                    {props.isContentCookingRecepie ? <Typography variant="h5" paragraph style={{ paddingTop: "5px" }} >
                        Opis
                    </Typography> : ""}
                    <Typography variant="subtitle1" paragraph style={{ paddingTop: "2px" }} >
                        {props.description}
                    </Typography>
                </Box>

            </Container>
        </>
    );
}
export default BasicHeaderInformation;