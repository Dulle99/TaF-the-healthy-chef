import { Grid, Typography } from "@mui/material";
import { Container } from "@mui/system";
import axios from "axios";
import React, { useEffect, useState } from "react";
import BlogPreview from "../Blog/BlogComponents/BlogPreview";
import IBlogPreviewProp from "../Blog/BlogInterfaces/IBlogPreviewProp";
import CookingRecepiePreview from "../CookingRecepie/CookingRecepieComponents/CookingRecepiePreview";
import ICookingRecepiePreviewProp from "../CookingRecepie/CookingRecepieInterface/ICookingRecepiePreviewProps";
import Header from "../Header/Header";

function Homepage() {
    const [recommendedBlogs, setRecommendedBlogs] = useState<IBlogPreviewProp[]>([]);
    const [recommendedCookingRecepies, setRecommendedCookingRecepies] = useState<ICookingRecepiePreviewProp[]>([]);

    useEffect(() => {

        const fetchedRecommendedCookingRecepies = async () => {
            const result = await axios.get<ICookingRecepiePreviewProp[]>('https://localhost:5001/api/CookingRecepie/GetRecommendedCookingRecepies');
            setRecommendedCookingRecepies(result.data); 
            console.log(result.data);   
        }
        const fetchedRecommendedBlogs = async () => {
            const result = await axios.get<IBlogPreviewProp[]>('https://localhost:5001/api/Blog/GetRecommendedBlogs');
            setRecommendedBlogs(result.data);
        }

        fetchedRecommendedBlogs();
        fetchedRecommendedCookingRecepies();
    }, []);
    useEffect(() => {
        document.title = "The healthy Chef";
    },[])
    return (
        
        <main>
                <Container>
                    <Typography component="h1"
                        variant="h2"
                        align="left"
                        color="text.primary">Recepti koje preporučujemo
                    </Typography>
                    <Grid>
                        {recommendedCookingRecepies.map((cookingRecepie) => (<CookingRecepiePreview {...cookingRecepie} key={cookingRecepie.cookingRecepieId} />))}
                    </Grid>
                    <Typography component="h1"
                        variant="h2"
                        align="left"
                        color="text.primary">Blogovi koje preporučujemo
                    </Typography>
                    <Grid>
                        {recommendedBlogs.map((blog) => (<BlogPreview {...blog} key={blog.blogId}/>))}
                    </Grid>
                </Container>

            </main>
        
    );
}
export default Homepage;