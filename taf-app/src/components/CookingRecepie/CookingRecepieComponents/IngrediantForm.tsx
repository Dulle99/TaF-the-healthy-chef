import { Box, Button, Grid, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import IIngredientToArray from "../CookingRecepieInterface/IPushIngrediantToArray";
import IPushStepInArray from "../CookingRecepieInterface/IPushStepInArray";

function IngrediantForm(prop: IIngredientToArray) {
    const [ordinalNumber, setOrdinalNumber] = useState(1);
    const [ingredient, setIngredient] = useState("");
    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        console.log(ordinalNumber);
        console.log(ingredient);
        if (ordinalNumber > 0 && ingredient != "") {
            prop.pushIngredientToArray({ ordinalNumberOfIngredient: ordinalNumber, ingredient: ingredient });
            setOrdinalNumber(ordinalNumber + 1);
            setIngredient("");
        }
    }

    const handleIngredientChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setIngredient(e.currentTarget.value);
    }

    useEffect(()=>{
        setOrdinalNumber(1);
    },[]);
    const handlersConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleIngredientChange]
    const configs: string[][] = [["Naziv sastojka", "Naziv sastojka", "nazivSastojka", "text"]];
    return (<>
        <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
            <Grid container spacing={2}>
                {configs.map((conf, ind) => (
                    <Grid item xs={12} sm={6} key={conf[0]}>
                        <TextField
                            label={conf[0]}
                            name={conf[1]}
                            id={conf[2]}
                            type={conf[3]}
                            required
                            fullWidth
                            autoFocus={conf[1] === "Ime" ? true : false}
                            onChange={handlersConfig[ind]}
                            value={ingredient}
                        />
                    </Grid>
                ))}
                <Button
                    size="small"
                    type="submit"
                    variant="contained"
                    sx={{
                        mt: 3, mb: 2, background: '#2f9a27', ':hover': {
                            bgcolor: '#27699A',
                            color: 'FFFFFF',
                        },
                        marginLeft: "25px"
                    }}
                >
                    Dodaj sastojak
                </Button>
            </Grid>

        </Box>
    </>);

}
export default IngrediantForm;