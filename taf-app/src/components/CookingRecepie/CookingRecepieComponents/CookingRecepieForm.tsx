import { BlockTwoTone } from "@mui/icons-material";
import { Alert, AlertTitle, Box, Button, Container, CssBaseline, FormControl, Grid, InputLabel, MenuItem, Select, SelectChangeEvent, TextField, Typography } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";
import { Link, Navigate, useLocation } from "react-router-dom";
import ICookingRecepie from "../CookingRecepieInterface/ICookingRecepie";
import ICookingRecepieUpdate from "../CookingRecepieInterface/ICookingRecepieUpdate";
import IIngredient from "../CookingRecepieInterface/IIngredient";
import IStepInFoodPreparation from "../CookingRecepieInterface/IStepInFoodPreparation";
import IngrediantForm from "./IngrediantForm";
import StepInFoodPreparationForm from "./StepInFoodPreparationForm";
import { TbPoint } from "react-icons/tb";

function CookingRecepieForm(prop: ICookingRecepie | any) {
    const [cookingRecepieTitle, setCookingRecepieTitle] = useState("");
    const [description, setCookingRecepieDescription] = useState("");
    const [preparationTime, setCookingRecepiePreparationTime] = useState<string>("1");
    const [cookingRecepiePicturePreview, setCookingRecepiePicturePreview] = useState<Blob>(new Blob);
    const [fetchedPictureForUpdate, setFetchedPictureForUpdate] = useState("");
    const [stepsInFoodPreparation, setStepInFoodPreparation] = useState<IStepInFoodPreparation[]>([]);
    const [ingredients, setIngredientsForFoodPreparation] = useState<IIngredient[]>([]);
    const [typeOfMeal, setTypeOfMeal] = useState("Dorucak");
    const [formData, setFormData] = useState<FormData>();
    const [recepieCreated, setRecepieCreatedFlag] = useState(false);
    const [areFieldsEmpthy, setAreFieldsEmpthyFlag] = useState(false);

    function validateFields(): boolean {
        if (cookingRecepieTitle === "" || description === "" || preparationTime ==="" || parseInt(preparationTime)=== NaN
        || parseInt(preparationTime) <= 0 || formData === undefined) {
            setAreFieldsEmpthyFlag(true);
            return true;
        }
        else {
            setAreFieldsEmpthyFlag(false);
            return false;
        }
    }
    const handleCookingRecepieTitleChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setCookingRecepieTitle(e.currentTarget.value);

    }
    const handleCookingRecepieDescriptionChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setCookingRecepieDescription(e.currentTarget.value);

    }
    const handleCookingRecepiePrepratinTimeChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        const number = parseInt(e.currentTarget.value); 
        if (number != NaN) {
            setCookingRecepiePreparationTime(e.currentTarget.value);
        }
        else{
            setCookingRecepiePreparationTime("");
        }
    }

    const handleChangePicture: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        // e.preventDefault();
        if (e.target.files !== null) {
            setCookingRecepiePicturePreview(e.target.files[0]);
            let form = new FormData();
            for (let i = 0; i < e.target.files.length; i++) {
                let element = e.target.files[i];
                form.append('cookingRecepiePicture', element);
            }
            setFormData(form);
        }

    }

    function pushStepInFoodPreparationToArray(step: IStepInFoodPreparation): void {
        if (step.ordinalNumberOfStep && step.stepDescription !== "")
            setStepInFoodPreparation([...stepsInFoodPreparation, step]);
    }

    function pushIngredientToArray(ingredient: IIngredient): void {
        if (ingredient.ordinalNumberOfIngredient > 0 && ingredient.ingredient !== "")
            setIngredientsForFoodPreparation([...ingredients, ingredient]);
    }

    const buttonHandlerRemoveIngredient = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        removeIngredientForFoodPreparationFromArray(parseInt(event.currentTarget.id, 10));
    }

    const buttonHandlerRemoveStep = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        removeStepInFoodPreparationFromArray(parseInt(event.currentTarget.id, 10));
    }

    function removeStepInFoodPreparationFromArray(stepIndex: number): void {
        setStepInFoodPreparation((steps) => steps.filter((step, ind) => ind !== stepIndex));
    }

    function removeIngredientForFoodPreparationFromArray(ingredientIndex: number): void {
        setIngredientsForFoodPreparation((ingredients) => ingredients.filter((ingredient, ind) => ind !== ingredientIndex));
    }


    const handleSelectTypeOfMealChange = (event: SelectChangeEvent) => {
        setTypeOfMeal(event.target.value as string);
    };

    const handleCreateUpdateRecepieClick: React.MouseEventHandler<HTMLButtonElement> = (e) => {
        console.log(preparationTime);
        if (validateFields() === false) {
            let form = new FormData();
            if (formData != undefined) {
                form = formData;
            }
            form.append('cookingRecepieTitle', cookingRecepieTitle);
            form.append('description', description);
            form.append('preparationTime', preparationTime.toString());
            form.append('typeOfMeal', typeOfMeal);

            stepsInFoodPreparation.forEach((step, ind) => {
                form.append(`stepsInFoodPreparation[${ind}].ordinalNumberOfStep`, step.ordinalNumberOfStep.toString());
                form.append(`stepsInFoodPreparation[${ind}].stepDescription`, step.stepDescription);
            })

            ingredients.forEach((ingredient, ind) => {
                form.append(`ingredients[${ind}].ordinalNumberOfIngredient`, ingredient.ordinalNumberOfIngredient.toString());
                form.append(`ingredients[${ind}].Ingredient`, ingredient.ingredient);
            })

            if (prop.cookingRecepieUpdate === true) {
                axios.put(`https://localhost:5001/api/CookingRecepie/UpdateCookingRecepie/${prop.cookingRecepieId}`, formData,
                    {
                        headers: {
                            'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                        },
                    })
                    .then((response) => {
                        if (response.status === 200) {
                            setRecepieCreatedFlag(true);
                        }
                    })
                    .catch(() => { });

            }
            else {
                axios.post(`https://localhost:5001/api/CookingRecepie/CreateCookingRecepie/${sessionStorage.getItem('username')}`, formData,
                    {
                        headers: {
                            'Authorization': 'Bearer ' + window.sessionStorage.getItem("token")
                        },
                    })
                    .then((response) => {
                        if (response.status === 200) {
                            setRecepieCreatedFlag(true);
                        }
                    })
                    .catch(() => { });
            }
        }

    }

    useEffect(() => {
        if (prop.cookingRecepieUpdate != undefined && prop.cookingRecepieId != undefined) {
            const fetchCookingRecepie = async () => {
                const result = await axios.get<ICookingRecepie>(`https://localhost:5001/api/CookingRecepie/GetCookingRecepie/${prop.cookingRecepieId}`);
                if (result.status === 200) {
                    setCookingRecepieTitle(result.data.cookingRecepieTitle);
                    setCookingRecepieDescription(result.data.description);
                    setCookingRecepiePreparationTime(result.data.preparationTime.toString());
                    setTypeOfMeal(result.data.typeOfMeal);
                    setStepInFoodPreparation(result.data.stepsInFoodPreparation);
                    if(result.data.ingredients != null){
                    setIngredientsForFoodPreparation(result.data.ingredients);}
                    setFetchedPictureForUpdate(result.data.cookingRecepiePicture);
                    let form = new FormData();
                    form.append('cookingRecepiePicture', result.data.cookingRecepiePicture);

                    setFormData(form);
                }
            }

            fetchCookingRecepie();
        }
    }, []);

    useEffect(()=>{
        if (prop.cookingRecepieUpdate != undefined && prop.cookingRecepieId != undefined) {
            document.title = "Ažuriranje recepta";
        }
        else{
            document.title = "Kreiranje recepta";
        }
    },[]);


    const configs: string[][] = [["Naslov recepta", "cookingRecepieTitle", "naslovRecepta"], ["Opis recepta", "description", "opisRecepta"]];
    const preparationTimeConfigs: string[] = ["Vreme spremanja", "preparationTime", "vremeSpremanja", "number"]
    const handlersForSecondConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleCookingRecepieTitleChange, handleCookingRecepieDescriptionChange];
    return (
        <>
            {recepieCreated === true ? <Navigate to='/homepage' /> : ""}
            <Container component="main" maxWidth="md">
                <CssBaseline />
                <Box
                    sx={{
                        marginTop: 8,
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'flex-start',
                    }}
                >
                    <Typography component="h1" variant="h4"> {prop.cookingRecepieUpdate != undefined ? "Ažuriranje recepta" : "Kreiranje recepta"}   </Typography>
                </Box>


                <Box component="form" noValidate sx={{ mt: 3 }}>

                    <Grid container spacing={2}>
                        {configs.map((conf, ind) => (
                            <Grid item xs={12} key={conf[2]}>
                                <TextField
                                    label={conf[0]}
                                    name={conf[1]}
                                    id={conf[2]}
                                    type="text"
                                    multiline={conf[2] === "opisRecepta" ? true : false}
                                    value={conf[2] === "opisRecepta" ? description : cookingRecepieTitle}
                                    rows={6}
                                    required
                                    fullWidth
                                    onChange={handlersForSecondConfig[ind]}
                                />
                            </Grid>))}

                        <Grid item xs={12} sm={6}>
                            <TextField
                                label={preparationTimeConfigs[0]}
                                name={preparationTimeConfigs[1]}
                                id={preparationTimeConfigs[2]}
                                required
                                fullWidth
                                value = {preparationTime}
                                type={preparationTimeConfigs[3]}
                                onChange={handleCookingRecepiePrepratinTimeChange}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>


                            <FormControl fullWidth>
                                <InputLabel id="demo-simple-select-label" shrink={true}>Za spremanje</InputLabel>
                                <Select
                                    labelId="demo-simple-select-label"
                                    id="demo-simple-select"
                                    value={typeOfMeal}
                                    label="Za spremanje"
                                    onChange={handleSelectTypeOfMealChange}
                                >
                                    <MenuItem value={"Dorucak"}>{"Doručak"}</MenuItem>
                                    <MenuItem value={"Rucak"}>{"Ručak"}</MenuItem>
                                    <MenuItem value={"Vecera"}>{"Večera"}</MenuItem>
                                    <MenuItem value={"Uzina"}>{"Užina"}</MenuItem>
                                </Select>
                            </FormControl>
                        </Grid>
                    </Grid>
                </Box>

                <Box >
                    <Typography variant="h6" sx={{ paddingTop: "15px" }} align="center">Navedite sastojke potrebne za spremanje hrane:</Typography>
                    <IngrediantForm pushIngredientToArray={pushIngredientToArray} />
                    <Grid >
                        <Typography sx={{ marginLeft: "30px", marginTop: "5px" }} variant="body1" position="relative">Sastojci:</Typography>
                        {ingredients.map((ingredient, ind) => (
                            <Box maxWidth="800px" sx={{ justifyContent: "space-between", paddingLeft: "25px" }} key={ind}>
                                <Typography sx={{ display: "flex", justifyItems: "flex-start" }} maxWidth="800px" paddingLeft={1}>
                                    <TbPoint size={20} /> {ingredient.ingredient}
                                </Typography>
                                <Box>
                                    <Button id={ind.toString()}
                                        size="small"
                                        variant="contained"
                                        sx={{
                                            background: '#e01f2a', ':hover': {
                                                bgcolor: '#ed7c83',
                                                color: 'FFFFFF',
                                            }, marginLeft: "5px"
                                        }}
                                        onClick={buttonHandlerRemoveIngredient}>
                                        Obriši sastojak
                                    </Button>

                                </Box>
                            </Box>))}
                    </Grid>




                    <Typography variant="h6" sx={{ paddingTop: "20px" }} align="center">Navedite korake za spremanje hrane:</Typography>
                    <StepInFoodPreparationForm pushStepInFoodPreparationToArray={pushStepInFoodPreparationToArray} />
                    <Grid >
                        <Typography sx={{ marginLeft: "30px", marginTop: "5px" }} variant="body1" position="relative">Koraci:</Typography>

                        {stepsInFoodPreparation.sort((step1, step2) => step1.ordinalNumberOfStep -step2.ordinalNumberOfStep).map((step, ind) => (
                            <Box maxWidth="800px" sx={{ justifyContent: "space-between", paddingLeft: "25px" }} key={ind}>
                                <Typography sx={{ display: "flex", justifyItems: "flex-start" }} maxWidth="800px" paddingLeft={1}>
                                <TbPoint size={20} />Korak {step.ordinalNumberOfStep} - {step.stepDescription}
                                </Typography>
                                <Box>
                                    <Button id={ind.toString()}
                                        size="small"
                                        variant="contained"
                                        sx={{
                                            background: '#e01f2a', ':hover': {
                                                bgcolor: '#ed7c83',
                                                color: 'FFFFFF',
                                            }, marginLeft: "5px"
                                        }}
                                        onClick={buttonHandlerRemoveStep}>
                                        Obriši korak
                                    </Button>

                                </Box>
                            </Box>))}
                    </Grid>

                </Box >



                <Box sx={{ display: "flex", justifyContent: "flex-start", flexDirection: "column" }}>
                    <Box>
                        <Button
                            variant="contained"
                            component="label"
                            sx={{
                                mt: 8, mb: 2, background: '#2f9a27', ':hover': {
                                    bgcolor: '#27699A',
                                    color: 'FFFFFF',
                                },
                            }}
                        >
                            Izaberi sliku recepta
                            <input
                                type="file"
                                hidden
                                onChange={handleChangePicture}
                            />
                        </Button>
                    </Box>
                    {cookingRecepiePicturePreview != undefined && cookingRecepiePicturePreview.size > 0 ?
                        <Box
                            component="img"
                            sx={{
                                height: 233,
                                width: 350,
                                maxHeight: { xs: 233, md: 167 },
                                maxWidth: { xs: 350, md: 250 },
                            }}

                            src={URL.createObjectURL(cookingRecepiePicturePreview)}
                        /> :
                        <Box
                            component="img"
                            sx={{
                                height: 150,
                                width: 200,
                                maxHeight: { xs: 233, md: 167 },
                                maxWidth: { xs: 350, md: 250 },
                            }}
                            src={fetchedPictureForUpdate.length > 0 ?`data:image/jpeg;base64,${fetchedPictureForUpdate}` : '/placeholderImage.jpg'}
                        />}

                </Box>

                {areFieldsEmpthy === true ?
                    <Alert severity="warning" sx={{ mt: 2 }}>
                        <AlertTitle>Upozorenje</AlertTitle>
                        Morate popuniti sva relevantna polja i uneti sliku!
                    </Alert>
                    : ""}

                <Box style={{ display: "flex", justifyContent: "center" }}>
                    <Button
                        type="submit"
                        onClick={handleCreateUpdateRecepieClick}
                        variant="contained"
                        sx={{
                            mt: 3, mb: 2, background: '#2f9a27', ':hover': {
                                bgcolor: '#27699A',
                                color: 'FFFFFF',
                            },

                        }}
                    >
                        {prop.cookingRecepieUpdate === true ? "Azuriraj recept" : "Kreiraj recept"}
                    </Button>
                </Box>
            </Container>
        </>
    );
}
export default CookingRecepieForm;