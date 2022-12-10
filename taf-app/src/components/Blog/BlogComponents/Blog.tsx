import { Box, Button, Checkbox, CssBaseline, Grid, Typography } from "@mui/material";
import { Container } from "@mui/system";
import axios from "axios";
import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import BasicHeaderInformation from "../../ContentMutalComponents/BasicHeaderInformation";
import Comment from "../../ContentMutalComponents/Comment";
import IBasicHeaderInformation from "../../ContentMutalComponents/IBasicHeaderInformation";
import IComment from "../../ContentMutalComponents/IComment";
import IBlog from "../BlogInterfaces/IBlog";
import IBlogId from "../BlogInterfaces/IBlogId";



function Blog() {
    const location = useLocation().state as IBlogId;
    const [blog, setBlog] = useState<IBlog>();
    const [isUserLoggedIn, setUserLoggedInFlag] = useState(false);


    useEffect(() => {
        const fetchBlog = async () => {
            const result = await axios.get<IBlog>(`https://localhost:5001/api/Blog/GetBlog/${location.blogId}`);
            if (result.status === 200) {
                setBlog(result.data);
            }
        }
        fetchBlog();
    }, [])

    useEffect(() => {
        if (sessionStorage.getItem('token') != null) {
            setUserLoggedInFlag(true);
        }
    }, [])

    useEffect(() => {
        document.title = "Blog";  
      }, []);


    return (
        <>
            <Container component="main" maxWidth="lg">
                <CssBaseline />
                <Box
                    sx={{

                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        component: 'form'
                    }}
                >

                    {blog !== undefined ?
                        <BasicHeaderInformation authorUsername={blog.authorUsername} authorProfilePicture={blog.authorProfilePicture}
                            contentTitle={blog.blogTitle} description={blog.content} publicationDate={blog.publicationDate}
                            timeForReadingOrPrepration={blog.readingTime} averageRate={blog.averageRate} contentPicture={blog.blogPicture}
                            isContentCookingRecepie={false} typeOfMeal={""} contentId={blog.blogId} /> : ""}
                    
                    <Comment isContentCookingRecepie={false} contentId={location.blogId} />
                </Box>
            </Container>
        </>
    );
}
export default Blog;