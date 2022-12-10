import { Box, Button, ToggleButton, ToggleButtonGroup, Typography } from "@mui/material";
import { Container } from "@mui/system";
import axios from "axios";
import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import BlogPreview from "../../../Blog/BlogComponents/BlogPreview";
import IBlogPreviewProp from "../../../Blog/BlogInterfaces/IBlogPreviewProp";
import CookingRecepiePreview from "../../../CookingRecepie/CookingRecepieComponents/CookingRecepiePreview";
import ICookingRecepiePreviewProp from "../../../CookingRecepie/CookingRecepieInterface/ICookingRecepiePreviewProps";
import IAuthor from "../AuthorInterface/IAuthor";
import IAuthorUsername from "../AuthorInterface/IAuthorUsername";
import AuthorInformation from "./AuthorInformation";

function AuthorPreview() {
    const location = useLocation().state as IAuthorUsername;
    const [typeOfContentToDisplay, setTypeOfContentToDisplay] = useState('cookingRecepies');
    const [authorsBlogs, setAuthorsBlogs] = useState<IBlogPreviewProp[]>([]);
    const [authorsCookingRecepies, setAuthorsCookingRecepies] = useState<ICookingRecepiePreviewProp[]>([]);
    const [numberOfBlogsToGet, setNumberOfBlogsToGet] = useState(5);
    const [numberOfCookingRecepiesToGet, setnumberOfCookingRecepiesToGet] = useState(5);

    async function fetchBlogs(numberOfBlogsToGet: number) {
        const result = await axios.get<IBlogPreviewProp[]>(`https://localhost:5001/api/Blog/GetBlogsByAuthor/${location.username}/${numberOfBlogsToGet.toString()}`,
            {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            })
        if (result.status === 200) {
            setAuthorsBlogs(result.data);
        }
    }

    async function fetchCookingRecepies(numberOfBlogsToGet: number) {
        const result = await axios.get<ICookingRecepiePreviewProp[]>(`https://localhost:5001/api/CookingRecepie/GetCookingRecepiesByAuthor/${location.username}/${numberOfBlogsToGet.toString()}`,
            {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            });
        if (result.status === 200) {
            setAuthorsCookingRecepies(result.data);
        }
    }
    const handleClickLoadMoreCookingRecepies = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        fetchCookingRecepies(numberOfCookingRecepiesToGet + 5);
        setnumberOfCookingRecepiesToGet(numberOfCookingRecepiesToGet + 5);
    }

    const handleClickLoadMoreBlogs = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        fetchBlogs(numberOfBlogsToGet + 5);
        setNumberOfBlogsToGet(numberOfBlogsToGet + 5);
    }

    const handleChange = (event: React.MouseEvent<HTMLElement>, newAlignment: string,) => {
        if (newAlignment != null) {
            setTypeOfContentToDisplay(newAlignment);
            setNumberOfBlogsToGet(5);
            setnumberOfCookingRecepiesToGet(5);
            if (newAlignment === "cookingRecepies") {
                fetchCookingRecepies(5);
            }
            else
                fetchBlogs(5);
        }

    };

    useEffect(() => {
        fetchCookingRecepies(5);
        fetchBlogs(5);
    }, []);

    useEffect(()=>{
        document.title = "Pregled profila";
    },[]);

    return (
        <>
            <Container>
                <AuthorInformation username={location.username} />
                <Box sx={{ display: "flex", justifyContent: "center", }}>
                    <ToggleButtonGroup
                        color="secondary"
                        value={typeOfContentToDisplay}
                        exclusive
                        onChange={handleChange}
                    >
                        <ToggleButton value="cookingRecepies">Recepti</ToggleButton>
                        <ToggleButton value="blogs">Blogovi</ToggleButton>
                    </ToggleButtonGroup>
                </Box>

                <Container sx={{ display: "flex", justifyContent: "space-between", flexWrap: "wrap", }}>
                    {typeOfContentToDisplay === "cookingRecepies" ? (
                        <Container>
                            <Typography variant="h5">Recepti</Typography>
                            {authorsCookingRecepies.map((recepie, ind) => (
                                <CookingRecepiePreview {...recepie} key={ind} />
                            ))}

                            <Box style={{ display: "flex", justifyContent: "center" }}>
                                {authorsCookingRecepies != null && authorsCookingRecepies.length > 0 ?
                                    <Button
                                        onClick={handleClickLoadMoreCookingRecepies}
                                        size='small' sx={{
                                            background: '#92279A', margin: '3px', ':hover': {
                                                bgcolor: '#b731c1',
                                                color: 'FFFFFF',
                                            },
                                        }}
                                    >
                                        <Typography color='#FFFFFF'>Učitaj još</Typography>
                                    </Button>
                                    :
                                    <Typography color="gray">Autor nema publikovane recepte.</Typography>}
                            </Box>
                        </Container>

                    )
                        : (

                            <Container>
                                <Typography variant="h5">Blogovi</Typography>
                                {authorsBlogs.map((blog, ind) => (
                                    <BlogPreview {...blog} key={ind} />
                                ))}
                                <Box style={{ display: "flex", justifyContent: "center" }}>
                                    {authorsBlogs != null && authorsBlogs.length > 0 ?
                                        <Button
                                            onClick={handleClickLoadMoreBlogs}
                                            size='small' sx={{
                                                background: '#92279A', margin: '3px', ':hover': {
                                                    bgcolor: '#b731c1',
                                                    color: 'FFFFFF',
                                                },
                                            }}
                                        >
                                            <Typography color='#FFFFFF'>Učitaj još</Typography>
                                        </Button>
                                        :
                                        <Typography color="gray">Nema publikovanih blogova.</Typography>}
                                </Box>
                            </Container>)}
                </Container>
            </Container>
        </>
    );
}
export default AuthorPreview;