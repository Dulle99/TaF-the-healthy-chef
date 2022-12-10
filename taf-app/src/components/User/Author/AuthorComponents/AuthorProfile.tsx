import { Box, Button, ToggleButton, ToggleButtonGroup, Typography } from "@mui/material";
import { Container } from "@mui/system";
import axios from "axios";
import { useEffect, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import IAuthorUsername from "../AuthorInterface/IAuthorUsername";
import AuthorInformation from "./AuthorInformation";
import AuthorPersonalAndSavedContent from "./AuthorPersonalAndSavedContent";
import { BsFillPencilFill } from "react-icons/bs";

function AuthorProfile() {
    const location = useLocation().state as IAuthorUsername;
    const [typeOfContentToDisplay, setTypeOfContentToDisplay] = useState('cookingRecepies');

    const handleChange = (event: React.MouseEvent<HTMLElement>, newAlignment: string,) => {
        if(newAlignment != null)
            setTypeOfContentToDisplay(newAlignment);
    };

    useEffect(()=>{
        document.title = "Moj profil";
    },[]);

    return (
        <>
            <Container>
                <AuthorInformation username={location.username} />
                    <Link to='/updateAuthor' state={{ username: location.username }} style={{ textDecoration: 'none' }}>
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
                <Box sx={{ display: "flex", justifyContent: "center", }}>
                    <ToggleButtonGroup
                        color="secondary"
                        value={typeOfContentToDisplay}
                        exclusive
                        onChange={handleChange}
                    >
                        <ToggleButton value="cookingRecepies" key={"cookingRecepies"}>Recepti</ToggleButton>
                        <ToggleButton value="blogs" key={"blogs"}>Blogovi</ToggleButton>
                        <ToggleButton value="savedBlogs" key={"savedBlogs"}>Sačuvani blogovi</ToggleButton>
                        <ToggleButton value="savedCookingRecepies" key={"savedcookingRecepies"}>Sačuvani recepti</ToggleButton>
                    </ToggleButtonGroup>
                </Box>

                <Container sx={{ display: "flex", justifyContent: "space-between", flexWrap: "wrap", }}>
                    <AuthorPersonalAndSavedContent authorUsername={location.username} contentType={typeOfContentToDisplay} />

                </Container>
            </Container>
        </>
    );
}
export default AuthorProfile;