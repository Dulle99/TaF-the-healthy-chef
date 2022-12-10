import { Avatar, Box, Button, Card, CardContent, Container, Grid, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import FormatDate from "./FormatDateFunction";
import IComment from "./IComment";

function CommentsPreview(props: IComment) {

    return (
        <>
            <Grid item xs={12} md={20} style={{ display: 'flex', flexWrap: 'wrap', flexDirection: 'column', maxWidth: "max", marginTop: "5px" }}>
                <Card >
                    <CardContent sx={{ flex: 1, }}>
                        <Container style={{ display: "flex", justifyContent: "space-between", padding: "0px", flexDirection: "column" }}>
                            <Box sx={{ display: 'flex', flexDirection: 'row', alignItems: "flex-start", justifyContent: "space-between" }} >
                                <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: "flex-start" }} >
                                    {sessionStorage.getItem('token') != null ?
                                        <Link to={props.typeOfUser === "Author" ? '/author' : '/reader'}
                                            state={{ username: props.authorOfComment }}
                                            style={{ textDecoration: 'none' }}>

                                            <Typography color='#92279A'>{props.authorOfComment}</Typography>
                                            <Avatar src={`data:image/jpeg;base64,${props.authorOfCommentProfilePicture}`} />
                                        </Link>
                                        :
                                        <Box>
                                            <Typography color='#92279A'>{props.authorOfComment}</Typography>
                                            <Avatar src={`data:image/jpeg;base64,${props.authorOfCommentProfilePicture}`} />
                                        </Box>}

                                </Box>
                                <Typography variant="subtitle1" color="GrayText">{FormatDate(props.publicationDate)}</Typography>
                            </Box>
                            <Box sx={{ display: 'flex', justifyContent: "center" }}>
                                <Typography color='#92279A'>{props.commentContent}</Typography>
                            </Box>
                        </Container>
                    </CardContent>
                </Card>
            </Grid>
        </>
    );
}
export default CommentsPreview;