import { Box, Button, Container, TextField, Typography } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";
import CommentsPreview from "./CommentsPreview";
import IComment from "./IComment";
import ICommentCreateInformation from "./ICommentCreateInformation";
import IContentCommentInformation from "./IContentCommentInformation";

function Comment(prop: IContentCommentInformation) {
    const [comments, setComments] = useState<IComment[]>([]);
    const [numberOfCommentsToGet, setNumberOfCommnetsToGet] = useState(4);
    const [renderAddCommentForm, setAddCommentFormFlag] = useState(false);
    const [commentContent, setCommentContent] = useState("");

    const fetchComments = async () => {
        let httpReq = "";
        if (prop.isContentCookingRecepie) {
            httpReq = `https://localhost:5001/api/CookingRecepie/GetCommentsOfCookingRecepie/${prop.contentId}/${numberOfCommentsToGet}`;
        }
        else {
            httpReq = `https://localhost:5001/api/Blog/GetCommentsOfBlog/${prop.contentId}/${numberOfCommentsToGet}`;
        }
        const result = await axios.get<IComment[]>(httpReq);
        if (result.status === 200) {
            setComments(result.data);
        }
    }

    const handleGetMoreComments: React.MouseEventHandler<HTMLLabelElement> = (e) => {
        setNumberOfCommnetsToGet(numberOfCommentsToGet + 4);
        fetchComments();
    }

    const handleAddCommentClicked: React.MouseEventHandler<HTMLLabelElement> = (e) => {
        setAddCommentFormFlag(true);
    }

    const handleSaveCommentClicked: React.MouseEventHandler<HTMLLabelElement> = (e) => {
        if (commentContent != "") {
            setAddCommentFormFlag(false);
            let TypeOfUser: string = sessionStorage.getItem('typeOfUser') === "Author" ? "Author" : "Reader";
            let Username = sessionStorage.getItem('username');
            let username: string = "";
            if (Username !== null) {
                username = Username;
            }

            let httpReq: string = "";
            if (prop.isContentCookingRecepie) {
                httpReq = `https://localhost:5001/api/CookingRecepie/AddCommentToTheCookingRecepie`;
            }
            else {
                httpReq = `https://localhost:5001/api/Blog/AddCommentToTheBlog`;
            }

            const addComment = async () => {
                const comment: ICommentCreateInformation = {
                    contentId: prop.contentId, username: username, typeOfUser: TypeOfUser
                    , commentContent: commentContent
                }
                const result = await axios.post(httpReq, comment,
                    {
                        headers: {
                            'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                        },
                    });

                if (result.status === 200) {
                    setAddCommentFormFlag(false);
                    fetchComments();
                }
            }
            addComment();
        }
    }

    const handleAddCommentChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setCommentContent(e.currentTarget.value);
    }

    useEffect(() => {
        fetchComments();
    }, [])

    useEffect(() => {
    }, [numberOfCommentsToGet])
    return (
        <>
            <Container sx={{ mb: 4 }} >

                <Typography variant="h5" paddingBottom="15px">Komentari</Typography>
                {sessionStorage.getItem('token') == null ? "" : (
                <Box>
                    {renderAddCommentForm === false ?
                        <Button variant="contained"
                            component="label"
                            sx={{
                                mb: 2, background: '#2f9a27', ':hover': {
                                    bgcolor: '#27699A',
                                    color: 'FFFFFF',
                                },
                            }}
                            onClick={handleAddCommentClicked}>Dodaj komentar</Button> : ""}

                    {renderAddCommentForm ? <Box>
                        <TextField
                            label={"Unesite komentar"}
                            name={"unesiteKomentar"}
                            id={"UnesiteKomentar"}
                            required
                            fullWidth
                            autoFocus={true}
                            onChange={handleAddCommentChange}
                        />

                        <Button variant="contained"
                            component="label"
                            sx={{
                                mt: 1, mb: 1, background: '#2f9a27', ':hover': {
                                    bgcolor: '#27699A',
                                    color: 'FFFFFF',
                                },
                            }}
                            onClick={handleSaveCommentClicked}>Sačuvaj komentar</Button>
                    </Box> : ""}
                </Box> )}

                {comments.map((comment, ind) => (
                    <CommentsPreview authorOfComment={comment.authorOfComment} authorOfCommentProfilePicture={comment.authorOfCommentProfilePicture}
                        publicationDate={comment.publicationDate} commentContent={comment.commentContent} typeOfUser={comment.typeOfUser} key={ind} />
                ))}

                <Box sx={{ display: "flex", justifyContent: "center" }} mt={4}>
                    {comments.length > 0 ?
                    <Button variant="contained"
                        component="label"
                        sx={{
                            mb: 2, background: '#2f9a27', ':hover': {
                                bgcolor: '#27699A',
                                color: 'FFFFFF',
                            },
                        }}
                        onClick={handleGetMoreComments}>
                        Prikaži vise
                    </Button>
                    : 
                    <Typography align="center" color="GrayText">Nema komentara.</Typography>}
                </Box>
            </Container>
        </>
    );
}
export default Comment;