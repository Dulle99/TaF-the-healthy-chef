import { Box, Button, Checkbox, CssBaseline, Grid, Typography } from "@mui/material";
import { Container } from "@mui/system";
import axios from "axios";
import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import BasicHeaderInformation from "../../ContentMutalComponents/BasicHeaderInformation";
import Comment from "../../ContentMutalComponents/Comment";
import IBasicHeaderInformation from "../../ContentMutalComponents/IBasicHeaderInformation";
import IComment from "../../ContentMutalComponents/IComment";
import ICookingRecepie from "../CookingRecepieInterface/ICookingRecepie";
import ICookingRecepieId from "../CookingRecepieInterface/ICookingRecepieId";
import { TbPoint } from "react-icons/tb";



function CookingRecepie() {
    const location = useLocation().state as ICookingRecepieId;
    const [cookingRecepie, setCookingRecepie] = useState<ICookingRecepie>();
    const [isUserLoggedIn, setUserLoggedInFlag] = useState(false);

    const label = { inputProps: { 'aria-label': 'Checkbox demo' } };

    const handleCheckBoxClicked: React.MouseEventHandler<HTMLButtonElement>= (e) => {
        
    }


    useEffect(() => {
        const fetchCookingRecepie = async () => {
            const result = await axios.get<ICookingRecepie>(`https://localhost:5001/api/CookingRecepie/GetCookingRecepie/${location.cookingRecepieId}`);
            if (result.status === 200) {
                setCookingRecepie(result.data);
            }
        }

        fetchCookingRecepie();
    }, [])

    useEffect(() => {
        if (sessionStorage.getItem('token') != null) {
            setUserLoggedInFlag(true);
        }
    }, []);

    useEffect(() => {
        document.title = "Recept";  
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

                    {cookingRecepie !== undefined ?
                        <BasicHeaderInformation authorUsername={cookingRecepie.authorUsername} authorProfilePicture={cookingRecepie.authorProfilePicture}
                            contentTitle={cookingRecepie.cookingRecepieTitle} description={cookingRecepie.description} publicationDate={cookingRecepie.publicationDate}
                            timeForReadingOrPrepration={cookingRecepie.preparationTime} averageRate={cookingRecepie.averageRate} contentPicture={cookingRecepie.cookingRecepiePicture}
                            isContentCookingRecepie={true} typeOfMeal={cookingRecepie.typeOfMeal} contentId={cookingRecepie.cookingRecepieId} /> : ""}


                    <Container >
                        {cookingRecepie !== undefined && cookingRecepie.ingredients != null && cookingRecepie?.ingredients.length > 0 ?
                            <Typography variant="h5" paddingBottom="10px">Potrebni sastojci</Typography> : ""}
                        {cookingRecepie !== undefined && cookingRecepie.ingredients != null ? cookingRecepie.ingredients.map((ingredient, ind) => (
                            <Grid sx={{ display: "flex", justifyContent: "space-between" }} padding="5px" key={ind} >
                                <Box sx={{display: "flex"}}>
                                    <TbPoint size={25} />
                                    <Typography>{ingredient.ingredient}</Typography>
                                </Box>
                            </Grid>
                        )) : ""}
                    </Container>


                    <Container >
                    {cookingRecepie !== undefined && cookingRecepie.stepsInFoodPreparation != null && cookingRecepie.stepsInFoodPreparation.length > 0 ?
                        <Typography variant="h5" paddingBottom="10px">Priprema</Typography> : "" }
                        {cookingRecepie !== undefined && cookingRecepie.stepsInFoodPreparation != null ? cookingRecepie.stepsInFoodPreparation.map((step, ind) => (
                            <Grid sx={{ display: "flex", justifyContent: "space-between" }} padding="5px" key={ind} >
                                <Box>
                                    <Typography variant='h6'>Korak {step.ordinalNumberOfStep}</Typography>
                                    <Typography>{step.stepDescription}</Typography>
                                </Box>
                                {isUserLoggedIn ?
                                    <Box>
                                        <Checkbox {...label} color="secondary" key={ind} id={"1"} onClick={handleCheckBoxClicked} />
                                    </Box> : ""}
                            </Grid>
                        )) : ""}
                    </Container>

                    <Comment isContentCookingRecepie={true} contentId={location.cookingRecepieId} />
                </Box>
            </Container>
        </>
    );
}
export default CookingRecepie;