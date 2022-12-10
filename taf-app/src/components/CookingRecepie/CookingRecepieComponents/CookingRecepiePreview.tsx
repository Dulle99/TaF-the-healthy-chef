import { Avatar, Button, Card, CardActionArea, CardContent, CardMedia, Chip, Container, Grid, Rating, Typography } from "@mui/material";
import { Box } from "@mui/system";
import React, { useState } from "react";
import { Link, Navigate } from "react-router-dom";
import ICookingRecepiePreviewProp from "../CookingRecepieInterface/ICookingRecepiePreviewProps";
import { GiCookingGlove, GiCookingPot, GiCampCookingPot } from "react-icons/gi";
import FormatDateFunction from "../../ContentMutalComponents/FormatDateFunction";


function CookingRecepiePreview(props: ICookingRecepiePreviewProp) {
    const [authorUsernameClicked, setAuthorUsernameClickedFlag] = useState(false);
    const handleClickUserName: React.MouseEventHandler<HTMLButtonElement> = (e) => {
        if (sessionStorage.getItem('token') != null && window.location.pathname != '/author') {
            setAuthorUsernameClickedFlag(true);
        }
    }

    return (
        <>
            {authorUsernameClicked ? <Navigate to={props.authorUsername === sessionStorage.getItem('username') ? "/authorProfile" : "/author"}
                state={{ username: props.authorUsername }} /> : ""}
            <Grid item xs={12} md={20} key={props.cookingRecepieId} style={{ display: 'flex', flexWrap: 'wrap', flexDirection: 'column', maxWidth: "max", marginTop: "5px" }}>
                <Card >
                    <CardContent sx={{ flex: 1, }}>
                        <Container style={{ display: "flex", justifyContent: "space-between", padding: "0px" }}>
                            <Box sx={{ display: 'flex', flexDirection: 'row', alignItems: "flex-start" }} >
                                <Avatar src={`data:image/jpeg;base64,${props.authorProfilePicture}`} />
                                <Button variant='text' value={props.authorUsername} onClick={handleClickUserName}   >
                                    <Typography color='#92279A'>{props.authorUsername}</Typography>
                                </Button>
                            </Box>
                            <Box>
                                <Typography component="h2" variant="h5" style={{ textAlign: 'center' }}>
                                    {props.cookingRecepieTitle}
                                </Typography>
                            </Box>
                            <Box>
                                <Grid sx={{ display: "flex", flexDirection: "column", justifyContent: "right" }}>
                                    <Box sx={{ display: "flex", }}>
                                        <GiCookingGlove size={22} />
                                        <GiCampCookingPot size={22} />
                                        <Box paddingLeft={1} >
                                            <Typography variant="subtitle1" color="text.secondary" align="center" >
                                                {props.preparationTime} min
                                            </Typography>
                                        </Box>
                                    </Box>
                                    <Typography variant="subtitle1" color="text.secondary" align="center" >{FormatDateFunction(props.publicationDate)}</Typography>
                                    <Chip label={props.typeOfMeal} color='primary' style={{ backgroundColor: '#92279A' }} />
                                    <Rating
                                        name="average-rate"
                                        value={props.averageRate}
                                        readOnly
                                    />
                                </Grid>

                            </Box>
                        </Container>

                        <Box sx={{ display: "flex", justifyContent: "space-between" }}>

                            <Box >
                                <Typography variant="subtitle1" paragraph style={{ paddingTop: "10px" }} >
                                    {props.description}

                                </Typography>
                            </Box>

                            <CardMedia
                                component="img"
                                sx={{ width: 160, display: { xs: 'block', sm: 'block' } }}
                                src={`data:image/jpeg;base64,${props.cookingRecepiePicture}`}

                            />

                        </Box>

                        <Link to='/cookingRecepie' state={{ cookingRecepieId: props.cookingRecepieId }} style={{ textDecoration: 'none' }}>
                            <Button value={props.cookingRecepieId} style={{
                                borderRadius: 40,
                            }} key={props.cookingRecepieId} sx={{
                                mt: 3, mb: 2, background: '#92279A', ':hover': {
                                    bgcolor: '#b731c1',
                                    color: 'FFFFFF',
                                },
                            }}>
                                <Typography color="common.white">Nastavi da čitas</Typography>
                            </Button>
                        </Link>
                    </CardContent>
                </Card>
            </Grid>
        </>
    );
}
export default CookingRecepiePreview;