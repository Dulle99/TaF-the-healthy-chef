import { Box, Button, ToggleButton, ToggleButtonGroup, Typography } from "@mui/material";
import { Container } from "@mui/system";
import axios from "axios";
import { useEffect, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import BlogPreview from "../../../Blog/BlogComponents/BlogPreview";
import IBlogPreviewProp from "../../../Blog/BlogInterfaces/IBlogPreviewProp";
import CookingRecepiePreview from "../../../CookingRecepie/CookingRecepieComponents/CookingRecepiePreview";
import ICookingRecepiePreviewProp from "../../../CookingRecepie/CookingRecepieInterface/ICookingRecepiePreviewProps";
import DeleteSavedContent from "../../MutalUserComponents/Components/DeleteSavedContent";
import IReaderUsername from "../ReaderInterface/IReaderUsername";
import ReaderInformation from "./ReaderInformation";
import { BsFillPencilFill } from "react-icons/bs";

function ReaderProfile() {
    const location = useLocation().state as IReaderUsername;
    const [typeOfContentToDisplay, setTypeOfContentToDisplay] = useState('savedBlogs');
    const [readersSavedBlogs, setReadersSavedBlogs] = useState<IBlogPreviewProp[]>([]);
    const [readersSavedCookingRecepies, setReadersSavedCookingRecepies] = useState<ICookingRecepiePreviewProp[]>([]);
    const [numberOfSavedBlogsToGet, setNumberOfSavedBlogsToGet] = useState(5);
    const [numberOfSavedCookingRecepiesToGet, setnumberOfSavedCookingRecepiesToGet] = useState(5);

    const handleChange = (event: React.MouseEvent<HTMLElement>, newAlignment: string,) => {
        if(newAlignment !=null)
            setTypeOfContentToDisplay(newAlignment);
    };

    const handleClickLoadMoreSavedCookingRecepies = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        fetchSavedCookingRecepies(numberOfSavedCookingRecepiesToGet + 5);
        setnumberOfSavedCookingRecepiesToGet(numberOfSavedCookingRecepiesToGet + 5);
    }

    const handleClickLoadMoreSavedBlogs = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        fetchSavedBlogs(numberOfSavedBlogsToGet + 5);
        setNumberOfSavedBlogsToGet(numberOfSavedBlogsToGet + 5);
    }

    async function removeContentFromReadLater(isCookingRecepie: boolean, contentId: string) {
        let username = sessionStorage.getItem('username');
        let typeOfUser = sessionStorage.getItem('typeOfUser');
        if (username != null || typeOfUser != null) {
            let url: string = isCookingRecepie ?
                `https://localhost:5001/api/User/RemoveCookingRecepieForReadLater/${contentId}/${username}/${typeOfUser}`
                :
                `https://localhost:5001/api/User/RemoveBlogForReadLater/${contentId}/${username}/${typeOfUser}`;

            await axios.delete(url, {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            });

            if (isCookingRecepie) {
                fetchSavedCookingRecepies(numberOfSavedCookingRecepiesToGet);
            }
            else {
                fetchSavedBlogs(numberOfSavedBlogsToGet);
            }

        }
    }

    async function fetchSavedBlogs(numberOfSavedBlogsToGet: number) {
        const result = await axios.get<IBlogPreviewProp[]>(`https://localhost:5001/api/User/GetSavedBlogs/${location.username}/Reader/${numberOfSavedBlogsToGet}`,
            {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            });
        if (result.status === 200) {
            setReadersSavedBlogs(result.data);
        }
    }

    async function fetchSavedCookingRecepies(numberOfSavedRecepiesToGet: number) {
        const result = await axios.get<ICookingRecepiePreviewProp[]>(`https://localhost:5001/api/User/GetSavedCookingRecepie/${location.username}/Reader/${numberOfSavedRecepiesToGet}`,
            {
                headers: {
                    'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                },
            });
        if (result.status === 200) {
            setReadersSavedCookingRecepies(result.data);
        }
    }

    useEffect(() => {
        fetchSavedCookingRecepies(5);
        fetchSavedBlogs(5);
    }, []);

    useEffect(()=>{
        document.title = "Moj profil";
    },[]);

    return (<>
        <Container>
            <ReaderInformation username={location.username} />
            <Link to='/updateReader' state={{ username: location.username }} style={{ textDecoration: 'none', display:"flex", justifyContent:"center" }}>
                    <Button
                        size='small' sx={{
                            background: '#ebb315', margin: '3px', ':hover': {
                                bgcolor: '#f1c959',
                                color: 'FFFFFF',
                            },
                            marginTop:"10px",
                            marginLeft: "15px",
                            marginBottom: "10px"
                        }}
                    >
                        <Typography color="white" >Izmeni profil <BsFillPencilFill color="white" size={15} />
                        </Typography>
                    </Button>
                </Link> 
            <Box sx={{ display: "flex", justifyContent: "center", mt: 5 }}>
                <ToggleButtonGroup
                    color="secondary"
                    value={typeOfContentToDisplay}
                    exclusive
                    onChange={handleChange}
                >
                    <ToggleButton value="savedBlogs" key={"savedBlogs"}>Sačuvani blogovi</ToggleButton>
                    <ToggleButton value="savedCookingRecepies" key={"savedcookingRecepies"}>Sačuvani recepti</ToggleButton>
                </ToggleButtonGroup>
            </Box>
        </Container>

        <Container sx={{ display: "flex", justifyContent: "space-between", flexWrap: "wrap", }}>
            {typeOfContentToDisplay === "savedCookingRecepies" ? 
            <Container>
                <Typography variant="h5">Sačuvani Recepti</Typography>
                {readersSavedCookingRecepies.map((recepie, ind) => (
                    <Box sx={{ display: "flex", flexDirection: "column", mt: 2 }} key={recepie.cookingRecepieId + ind}>
                        <Box sx={{ display: "flex", flexDirection: "row", justifyContent: "flex-end", mt: 1, mb: 1 }}>
                            <DeleteSavedContent typeOfContent="savedCookingRecepies" contentId={recepie.cookingRecepieId}
                                removeContentFromReadLater={removeContentFromReadLater} />
                        </Box>
                        <CookingRecepiePreview {...recepie} key={ind} />
                    </Box>
                ))}

                <Box style={{ display: "flex", justifyContent: "center" }}>
                    {readersSavedCookingRecepies != null && readersSavedCookingRecepies.length > 0 ? 
                    <Button
                        onClick={handleClickLoadMoreSavedCookingRecepies}
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
                    <Typography align="center" color="gray">Nema sačuvanih recepta.</Typography>}
                </Box>
            </Container>
             :
             <Container>
             <Typography variant="h5">Sacuvani Blogovi</Typography>
             {readersSavedBlogs.map((blog, ind) => (
                 <Box sx={{ display: "flex", flexDirection: "column", mt: 2 }} key={blog.blogId + ind}>
                     <Box sx={{ display: "flex", flexDirection: "row", justifyContent: "flex-end", mt: 1, mb: 1 }}>
                         <DeleteSavedContent typeOfContent="savedBlogs" contentId={blog.blogId}
                             removeContentFromReadLater={removeContentFromReadLater} />
                     </Box>
                     <BlogPreview {...blog} key={blog.blogId} />
                 </Box>
             ))}
             {readersSavedBlogs != null && readersSavedBlogs.length> 0 ?
             <Box style={{ display: "flex", justifyContent: "center" }}>

                 <Button
                     onClick={handleClickLoadMoreSavedBlogs}
                     size='small' sx={{
                         background: '#92279A', margin: '3px', ':hover': {
                             bgcolor: '#b731c1',
                             color: 'FFFFFF',
                         },
                     }}
                 >
                     <Typography color='#FFFFFF'>Učitaj još</Typography>
                 </Button>
             </Box>
              : 
             <Typography align="center" color="gray">Nema sacuvanih blogova.</Typography>}
         </Container>}

        </Container>
    </>);
}
export default ReaderProfile;