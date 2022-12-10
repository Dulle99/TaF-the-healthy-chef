import { Avatar, Button, Card, CardActionArea, CardContent, CardMedia, Container, Grid, Rating, Typography } from "@mui/material";
import { Box } from "@mui/system";
import React, { useState } from "react";
import { Link, Navigate } from "react-router-dom";
import IBlogPreviewProp from "../BlogInterfaces/IBlogPreviewProp";
import { GiRead } from "react-icons/gi";
import FormatDateFunction from "../../ContentMutalComponents/FormatDateFunction";



function BlogPreview(props: IBlogPreviewProp) {
    const [authorUsernameClicked, setAuthorUsernameClickedFlag] = useState(false);
    const handleClickUserName: React.MouseEventHandler<HTMLButtonElement> = (e) => {
        if (sessionStorage.getItem('token') != null && window.location.pathname != '/author') {
            setAuthorUsernameClickedFlag(true);
        }
    }
    const handleClickReadMore: React.MouseEventHandler<HTMLButtonElement> = (e) => {
        console.log(e.currentTarget.value);
    }

    return (
        <>
            {authorUsernameClicked ? <Navigate to={props.authorUsername === sessionStorage.getItem('username') ? "/authorProfile" : "/author"}
                                                     state={{ username: props.authorUsername }} /> : ""}

            <Grid item xs={12} md={20} key={props.blogId} style={{ display: 'flex', flexWrap: 'wrap', flexDirection: 'column', maxWidth: "max", marginTop: "5px" }}>
                <Card >
                    <CardContent sx={{ flex: 1, }}>
                        <Container style={{ display: "flex", justifyContent: "space-between", padding: "0px" }}>
                            <Box sx={{ display: 'flex', flexDirection: 'row', alignItems: "flex-start" }} >
                                <Avatar src={`data:image/jpeg;base64,${props.authorProfilePicture}`} />
                                <Button variant='text' value={props.authorUsername} onClick={handleClickUserName}>
                                    <Typography color='#92279A'>{props.authorUsername}</Typography>
                                </Button>
                            </Box>
                            <Box>
                                <Typography component="h2" variant="h5" style={{ textAlign: 'center' }}>
                                    {props.blogTitle}
                                </Typography>
                            </Box>
                            <Box>
                                <Grid sx={{display:"flex",flexDirection:"column", justifyContent:"right"}}>
                                    <Box sx={{display: "flex", justifyContent:"center" }}>
                                     <GiRead size={22}/>
                                    <Typography variant="subtitle2" color="text.secondary" paddingLeft={1} >
                                        {props.readingTime} min
                                    </Typography>
                                    </Box>
                                    <Typography variant="subtitle1" color="text.secondary" align="center" >{FormatDateFunction(props.publicationDate)}</Typography>
                                    <Rating
                                        name="average-rate"
                                        value={props.averageRate}
                                        readOnly
                                    />
                                </Grid>
                            </Box>
                        </Container>

                        <Box sx={{ display: "flex", justifyContent: "space-between"}}>

                            <Box>

                            <Typography variant="subtitle1" paragraph style={{ paddingTop: "10px" }} >
                                {props.partOfBlogContent}
                            </Typography>
                            </Box>

                            <CardMedia
                                component="img"
                                sx={{ maxWidth: 160, display: {  xs: 'block', sm: 'block' } }}
                                src={`data:image/jpeg;base64,${props.blogPicture}`}
                            />
                        </Box>
                        <Box>
                            <Link to='/blog' state={{ blogId: props.blogId }} style={{ textDecoration: 'none' }}>

                                <Button value={props.blogId} type='submit' style={{
                                    borderRadius: 40,
                                }} key={props.blogId} onClick={handleClickReadMore} sx={{
                                    mt: 3, mb: 2, background: '#92279A', ':hover': {
                                        bgcolor: '#b731c1',
                                        color: 'FFFFFF',
                                    },
                                }}>
                                    <Typography color="common.white">Nastavi da čitas</Typography>
                                </Button>
                            </Link>
                        </Box>
                    </CardContent>

                </Card>
            </Grid>
        </>
    );
}
export default BlogPreview;